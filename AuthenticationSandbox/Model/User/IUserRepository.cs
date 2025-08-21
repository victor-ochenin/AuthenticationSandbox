namespace AuthenticationSandbox.Model.User
{
    // IUserRepository - хранилище пользователей
    public interface IUserRepository
    {
        // получение пользователя по логину
        // возвращает null если пользователь не найден
        Task<User?> GetByLogin(string login);

        // вставка нового пользователя
        Task Insert(User user);

        // получение всех пользователей
        Task<List<User>> SelectAll();

        // количество администраторов
        Task<int> CountAdmins();

        // выставить/снять флаг администратора у пользователя
        Task SetAdmin(string login, bool isAdmin);
    }
}
