using MongoDB.Driver;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Services.Users
{
    public interface IUserServices
    {
        Task<UserDto> CreateUserAsync(UserDto user);
        bool UserExists(string username, string email);
        Task<UserDto> FindUserByIdAsync(Guid id);
        Task<UserDto> FindUserByUsernameAsync(string username);
        Task<UserDto> FindUserByEmailAsync(string email);
        Task<UserDto> UpdateUserAsync(UserDto updatedUser);
        Task<UserDto> DeleteUserAsync(Guid id);
        // ----------------------------------------------------------------------------
        Task<bool> RevokeTokenAsync(string username);
        Task<UserTokenDto> AuthenticadeUserAsync(UserCredentialsDto user);
        Task<UserTokenDto> ValidateCredentialsAsync(UserTokenDto token);
    }
}
