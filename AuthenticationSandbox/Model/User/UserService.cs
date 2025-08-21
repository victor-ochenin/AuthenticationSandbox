using AuthenticationSandbox.Model.Encoder;
using System.Text.RegularExpressions;

namespace AuthenticationSandbox.Model.User
{
    // UserService - сервис для работы с пользователями
    public class UserService
    {
        string loginPattern = @"^[a-zA-Z0-9_]{3,16}$"; // regexp для валидации логина

        private readonly IUserRepository _repository;
        private readonly IEncoder _encoder;

        public UserService(IUserRepository repository, IEncoder encoder)
        {
            _repository = repository;
            _encoder = encoder;
        }

        // Register - регистрация пользователя
        // вход: строка логина пользователя
        // выход: api-ключ для данного пользователя
        // исключения: InvalidLoginException, DuplicatedLoginException
        public async Task<string> Register(string login, bool isAdmin = false)
        {
            // валидация логина
            if (string.IsNullOrEmpty(login))
            {
                throw new InvalidLoginException(login, "login is empty");
            }
            if (!Regex.IsMatch(login, loginPattern))
            {
                throw new InvalidLoginException(login);
            }

            // проверить дублирование логина
            bool isDuplicated = await _repository.GetByLogin(login) != null;
            if (isDuplicated)
            {
                throw new DuplicatedLoginException(login);
            }

            // выполнить регистрацию
            User user = new User()
            {
                UUID = Guid.NewGuid(),
                Login = login,
                RegisteredAt = DateTime.UtcNow,
                IsAdmin = isAdmin,
            };
            string apiKey = _encoder.Encode(user.ToString());
            await _repository.Insert(user);
            return apiKey;
        }

        // Get - получение данных о пользователе по ключу
        // вход: api-ключ пользователя
        // выход: объект с информацией о пользователе
        // иключения: UserNotFoundException 
        public async Task<User> Get(string apiKey)
        {
            foreach (User user in await _repository.SelectAll())
            {
                if (_encoder.Encode(user.ToString()) == apiKey)
                {
                    return user;
                }
            }
            throw new UserNotFoundException();
        }

        // GetAll - получение списка всех пользователей
        // вход: -
        // выход: список всех пользователей
        public async Task<List<User>> GetAll()
        {
            return await _repository.SelectAll();
        }

        // SetAdmin - выставление/снятие флага администратора с защитой от удаления последнего админа
        public async Task SetAdmin(string login, bool isAdmin)
        {
            User? user = await _repository.GetByLogin(login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (!isAdmin && user.IsAdmin)
            {
                int admins = await _repository.CountAdmins();
                if (admins <= 1)
                {
                    throw new LastAdminRemovalException();
                }
            }

            await _repository.SetAdmin(login, isAdmin);
        }
    }
}
