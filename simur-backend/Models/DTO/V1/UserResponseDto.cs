using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.DTO.V1
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; } = "**********";
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
