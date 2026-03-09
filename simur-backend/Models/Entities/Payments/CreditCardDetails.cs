using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using simur_backend.Models.Deserealizers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("CreditCardDetails")]
    public class CreditCardDetails : IPaymentDetails
    {
        [Required]
        [JsonPropertyName("last_four_digits")]
        public string LastFourDigits { get; set; }
        [Required]
        [JsonPropertyName("card_holder_name")]
        public string CardHolderName { get; set; }
        [Required]
        [JsonPropertyName("expiration_date")]
        public DateOnly ExpirationDate { get; set; }
        [Required]
        [JsonPropertyName("brand")]
        [JsonConverter(typeof(GetPaymentBrandFromString))]
        public PaymentBrand Brand { get; set; }
        [Required]
        [JsonPropertyName("installments")]
        public int Installments { get; set; }
        [Required]
        [JsonPropertyName("interest_amount")]
        public decimal InterestAmount { get; set; }
    }
}
