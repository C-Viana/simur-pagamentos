using MongoDB.Bson.Serialization.Attributes;

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
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration {  get; set; }
        public List<string> UserRoles { get; set; }
    }
}
