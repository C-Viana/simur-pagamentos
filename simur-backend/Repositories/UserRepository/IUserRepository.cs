using MongoDB.Driver;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;

namespace simur_backend.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(IClientSessionHandle session, User user);
        Task<User> FindByIdAsync(Guid id);
        bool UserExistsAsync(string username, string email);
        Task<User> FindByUsernameAsync(string username);
        Task<User> FindByEmailAsync(string email);
        Task<User> UpdateAsync(User updatedUser);
        Task<User> UpdateAsync(IClientSessionHandle session, User updatedUser);
        Task<User> DeleteAsync(Guid id);
    }
}
