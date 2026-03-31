using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class UserDto
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        [Length(8, 30)]
        public string Username { get; set; }

        [Required]
        [JsonIgnore]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Length(5, 80)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
