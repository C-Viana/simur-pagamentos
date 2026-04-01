using Microsoft.IdentityModel.JsonWebTokens;
using MongoDB.Driver;
using simur_backend.Auth.Config;
using simur_backend.Auth.Contract;
using simur_backend.Mappers;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;
using simur_backend.Repositories.UserRepository;
using simur_backend.Utilities;
using System.Security.Claims;

namespace simur_backend.Services.Users
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _repository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly TokenConfiguration _tokenConfiguration;

        private readonly IMongoClient _mongoClient;
        private readonly ILogger<UserServices> _logger;
        private readonly UserConverter _mapper;

        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public UserServices(IUserRepository repository, ILogger<UserServices> logger, IMongoClient mongoClient, ITokenGenerator tokenGenerator, TokenConfiguration tokenConfiguration)
        {
            _repository = repository;
            _tokenGenerator = tokenGenerator;
            _tokenConfiguration = tokenConfiguration;
            _mongoClient = mongoClient;
            _logger = logger;
            _mapper = new UserConverter();
        }

        public async Task<UserResponseDto> CreateUserAsync(UserDto subscribingUser)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();

            try
            {
                subscribingUser.Password = AlgorithmSha256.HashString(subscribingUser.Password);
                User savedUser = await _repository.CreateAsync(_sessionHandle, _mapper.Parse(subscribingUser));
                await _sessionHandle.CommitTransactionAsync();
                return _mapper.Parse(savedUser);
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "User {username} creation failed", subscribingUser.Username);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(UserDto modifiedUser)
        {
            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                User updatedUser = await _repository.UpdateAsync(_sessionHandle, _mapper.Parse(modifiedUser));
                await _sessionHandle.CommitTransactionAsync();
                return _mapper.Parse(updatedUser);
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Updating user {username} failed", modifiedUser.Username);
                throw;
            }
        }

        public bool UserExists(string username, string email)
        {
            return _repository.UserExistsAsync(username, email);
        }

        public async Task<UserResponseDto> DeleteUserAsync(Guid id)
        {
            User deletedUser = await _repository.DeleteAsync(id);
            return _mapper.Parse(deletedUser);
        }

        public async Task<UserResponseDto> FindUserByEmailAsync(string email)
        {
            User foundUser = await _repository.FindByEmailAsync(email);
            return _mapper.Parse(foundUser);
        }

        public async Task<UserResponseDto> FindUserByIdAsync(Guid id)
        {
            User foundUser = await _repository.FindByIdAsync(id);
            return _mapper.Parse(foundUser);
        }

        public async Task<UserResponseDto> FindUserByUsernameAsync(string username)
        {
            User foundUser = await _repository.FindByUsernameAsync(username);
            return _mapper.Parse(foundUser);
        }

        public async Task<bool> RevokeTokenAsync(string username)
        {

            User foundUser = await _repository.FindByUsernameAsync(username);
            if (foundUser == null) return false;

            using IClientSessionHandle _sessionHandle = await _mongoClient.StartSessionAsync();
            _sessionHandle.StartTransaction();
            try
            {
                foundUser.RefreshToken = null;
                foundUser.RefreshTokenExpiration = DateTime.MinValue;
                await _repository.UpdateAsync(_sessionHandle, foundUser);
                await _sessionHandle.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _sessionHandle.AbortTransactionAsync();
                _logger.LogError(ex, "Revoking access of user {username} failed", username);
                throw;
            }
        }

        public async Task<UserTokenDto> AuthenticadeUserAsync(UserCredentialsDto credentials)
        {
            User foundUser = await _repository.FindByUsernameAsync(credentials.Username);
            if(foundUser == null) return null;
            if(!AlgorithmSha256.Compare(credentials.Password, foundUser.Password)) return null;
            return await GenerateTokenAsync(foundUser, null);
        }

        public async Task<UserTokenDto> ValidateCredentialsAsync(UserTokenDto token)
        {
            ClaimsPrincipal principal = _tokenGenerator.GerPrincipalFromExpiredToken(token.AccessToken);
            string username = principal.Identity.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var user = await _repository.FindByUsernameAsync(username);
            if( user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiration < DateTime.Now) return null;

            UserTokenDto renewedToken = await GenerateTokenAsync(user, principal.Claims);
            return renewedToken;
        }

        private async Task<UserTokenDto> GenerateTokenAsync(User user, IEnumerable<Claim> userClaims)
        {
            var claims = new List<Claim>() {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")), //Remove dashes from GUID
                new(JwtRegisteredClaimNames.UniqueName, user.Username)
            };

            if(user.UserRoles != null && user.UserRoles.Count > 0)
            {
                foreach (var role in user.UserRoles)
                {
                    claims.Add(new (ClaimTypes.Role, role));
                }
            }

            var accessToken = _tokenGenerator.GenerateAccessToken(claims);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            DateTime creationTime = DateTimeOffset.Now.DateTime;
            DateTime expirationTime = creationTime.AddMinutes(_tokenConfiguration.RefreshLifetimeMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = expirationTime;
            await _repository.UpdateAsync(user);

            _logger.LogInformation("Access token created for user {username}", user.Username);

            return new UserTokenDto(
                authenticated: true,
                created: creationTime.ToString(DATE_FORMAT),
                expiration: expirationTime.ToString(DATE_FORMAT),
                accessToken: accessToken,
                refreshToken: refreshToken
            );
        }
    }
}
