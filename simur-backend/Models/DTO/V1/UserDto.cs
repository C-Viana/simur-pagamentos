using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
