using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.DTO.V1
{
    public class PaymentCardRequestDto
    {
        [Required]
        public string CardNumber { get; init; }
        [Required]
        public string CardHolderName { get; init; }
        [Required]
        public int ExpirationMonth { get; init; }
        [Required]
        public int ExpirationYear { get; init; }
        [Required]
        public string Cvv { get; init; }
    }
}
