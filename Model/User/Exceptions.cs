namespace AuthenticationSandbox.Model.User
{
    // DuplicatedLoginException - исключение дублирующегося логина
    public class DuplicatedLoginException : Exception
    {
        public DuplicatedLoginException(string login) : base($"login '{login}' is duplicated")
        {
        }
    }

    // InvalidLoginException - исключение невалидного логина
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException(string login) : base($"login '{login}' is invalid")
        {

        }

        public InvalidLoginException(string login, string details) : base($"login '{login}' is invalid: {details}")
        {

        }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("user not found")
        {
        }
    }

    public class LastAdminRemovalException : Exception
    {
        public LastAdminRemovalException() : base("cannot remove the last admin")
        {
        }
    }
}
