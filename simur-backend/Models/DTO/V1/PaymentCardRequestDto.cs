using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.DTO.V1
{
    public class PaymentCardRequestDto
    {
        [Required]
        [NotNull]
        public string CardNumber { get; init; }
        [Required]
        [NotNull]
        public string CardHolderName { get; init; }
        [Required]
        [NotNull]
        public int ExpirationMonth { get; init; }
        [Required]
        [NotNull]
        public int ExpirationYear { get; init; }
        [Required]
        [NotNull]
        public string Cvv { get; init; }
    }
}
