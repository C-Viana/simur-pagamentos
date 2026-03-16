using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.DTO.V1
{
    public class PaymentCardResponseDto
    {
        [Required]
        public string Token { get; init; }

        [Required]
        public string LastFourDigits { get; init; }

        [Required]
        //[JsonConverter(typeof(GetPaymentBrandFromString))]
        public string Brand { get; init; }
    }
}
