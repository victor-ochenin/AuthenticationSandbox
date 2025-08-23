using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AuthenticationSandbox.Model.User
{
    public class MongoUserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public MongoUserRepository(IConfiguration configuration)
        {
            string? connectionString = configuration["Mongo:ConnectionString"] ?? "mongodb://localhost:27017";
            string databaseName = configuration["Mongo:Database"] ?? "AuthenticationSandbox";

            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);
            _users = database.GetCollection<User>("users");

            EnsureIndexes();
        }

        private void EnsureIndexes()
        {
            // unique index on Login
            IndexKeysDefinition<User> keys = Builders<User>.IndexKeys.Ascending(u => u.Login);
            CreateIndexModel<User> model = new CreateIndexModel<User>(keys, new CreateIndexOptions { Unique = true });
            _users.Indexes.CreateOne(model);
        }

        public async Task<User?> GetByLogin(string login)
        {
            return await _users.Find(u => u.Login == login).FirstOrDefaultAsync();
        }

        public async Task Insert(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<List<User>> SelectAll()
        {
            return await _users.Find(Builders<User>.Filter.Empty).ToListAsync();
        }

        public async Task<int> CountAdmins()
        {
            long count = await _users.CountDocumentsAsync(u => u.IsAdmin);
            return (int)count;
        }

        public async Task SetAdmin(string login, bool isAdmin)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(u => u.Login, login);
            UpdateDefinition<User> update = Builders<User>.Update.Set(u => u.IsAdmin, isAdmin);
            await _users.UpdateOneAsync(filter, update);
        }
    }
}


