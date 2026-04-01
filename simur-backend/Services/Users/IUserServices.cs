using MongoDB.Driver;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Services.Users
{
    public interface IUserServices
    {
        Task<UserResponseDto> CreateUserAsync(UserDto user);
        bool UserExists(string username, string email);
        Task<UserResponseDto> FindUserByIdAsync(Guid id);
        Task<UserResponseDto> FindUserByUsernameAsync(string username);
        Task<UserResponseDto> FindUserByEmailAsync(string email);
        Task<UserResponseDto> UpdateUserAsync(UserDto updatedUser);
        Task<UserResponseDto> DeleteUserAsync(Guid id);
        // ----------------------------------------------------------------------------
        Task<bool> RevokeTokenAsync(string username);
        Task<UserTokenDto> AuthenticadeUserAsync(UserCredentialsDto user);
        Task<UserTokenDto> ValidateCredentialsAsync(UserTokenDto token);
    }
}
