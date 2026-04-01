using rest_with_asp_net_10_cviana.Data.Converter.Contract;
using simur_backend.Models.DTO.V1;
using simur_backend.Models.Entities;

namespace simur_backend.Mappers
{
    public class UserConverter : IParser<UserDto, User>, IParser<User, UserResponseDto>
    {
        public User Parse(UserDto dto)
        {
            return new User(
                dto.Id,
                dto.Username,
                dto.Password,
                dto.FullName,
                dto.Email,
                dto.RefreshToken,
                dto.RefreshTokenExpiration
            );
        }

        //public UserDto Parse(User origin)
        //{
        //    return new UserDto()
        //    {
        //        Id = origin.Id,
        //        Username = origin.Username,
        //        Password = origin.Password,
        //        FullName = origin.FullName,
        //        Email = origin.Email,
        //        RefreshToken = origin.RefreshToken,
        //        RefreshTokenExpiration = origin.RefreshTokenExpiration
        //    };
        //}

        public UserResponseDto Parse(User origin)
        {
            return new UserResponseDto()
            {
                Id = origin.Id,
                Username = origin.Username,
                FullName = origin.FullName,
                Email = origin.Email
            };
        }

        public List<User> ParseList(List<UserDto> origin)
        {
            return [.. origin.Select(Parse)];
        }

        //public List<UserDto> ParseList(List<User> origin)
        //{
        //    return [.. origin.Select(Parse)];
        //}

        public List<UserResponseDto> ParseList(List<User> origin)
        {
            return [.. origin.Select(Parse)];
        }
    }
}
