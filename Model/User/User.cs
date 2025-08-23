using MongoDB.Bson.Serialization.Attributes;

namespace AuthenticationSandbox.Model.User
{
    // User - сущность пользователя
    [BsonIgnoreExtraElements]
    public class User
    {
        // уникальный идентификатор пользователя, выдается системой
        public Guid UUID { get; set; }
        // логин пользователя
        public required string Login { get; set; }
        // время регистрации
        public DateTime RegisteredAt { get; set; }
        // флаг админа
        public bool IsAdmin { get; set; }
        public User() { }

        public override string ToString()
        {
            return $"{UUID} - {Login} - {RegisteredAt}";
        }
    }
}
