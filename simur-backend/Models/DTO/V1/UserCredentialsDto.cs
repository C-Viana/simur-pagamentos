using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class UserCredentialsDto
    {
        public UserCredentialsDto(){}

        [Required]
        [NotNull]
        public string Username { get; set; }

        [Required]
        [NotNull]
        public string Password { get; set; }
    }
}
