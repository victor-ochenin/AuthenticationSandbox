using AuthenticationSandbox.Model.User;

namespace AuthenticationSandbox.Stub
{
    public class UserRepositoryStub : IUserRepository
    {
        public Dictionary<string, User> users = new Dictionary<string, User> ();

        public async Task<User?> GetByLogin(string login)
        {
            return users.TryGetValue(login, out User? user) ? user : null;
        }

        public async Task Insert(User user)
        {
            users.Add(user.Login, user);
        }

        public async Task<List<User>> SelectAll()
        {
            return users.Values.ToList();
        }

        public async Task<int> CountAdmins()
        {
            return users.Values.Count(u => u.IsAdmin);
        }

        public async Task SetAdmin(string login, bool isAdmin)
        {
            if (users.TryGetValue(login, out User? user))
            {
                user.IsAdmin = isAdmin;
            }
        }
    }
}
