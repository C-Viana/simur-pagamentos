using MongoDB.Driver;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<User>("users");
        }

        public async Task<User> CreateAsync(IClientSessionHandle session, User user)
        {
            await _collection.InsertOneAsync(session, user);
            return await _collection.Find(session, entity => entity.Username == user.Username).FirstOrDefaultAsync();
        }

        public async Task<User> DeleteAsync(Guid id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Id, id);
            return await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Email, email);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> FindByIdAsync(Guid id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Username, username);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> UpdateAsync(User updatedUser)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Id, updatedUser.Id);
            await _collection.FindOneAndReplaceAsync(filter, updatedUser);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> UpdateAsync(IClientSessionHandle session, User updatedUser)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Id, updatedUser.Id);
            await _collection.FindOneAndReplaceAsync(session, filter, updatedUser);
            return await _collection.Find(session, filter).FirstOrDefaultAsync();
        }

        /*
         * Returns true if username or user e-mail exists
         */
        public bool UserExistsAsync(string username, string email)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(entity => entity.Username, username) | Builders<User>.Filter.Eq(entity => entity.Email, email);
            return _collection.Find(filter).FirstOrDefaultAsync().GetAwaiter().GetResult() != null;
        }
    }
}
