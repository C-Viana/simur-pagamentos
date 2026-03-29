using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.Entities
{
    public class User
    {
        public User(Guid id, string username, string password, string fullName, string email, string refreshToken, DateTime refreshTokenExpiration)
        {
            Id = id;
            Username = username;
            Password = password;
            FullName = fullName;
            Email = email;
            RefreshToken = refreshToken;
            RefreshTokenExpiration = refreshTokenExpiration;
        }

        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Length(8,30)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Length(5, 80)]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration {  get; set; }


    }
}
