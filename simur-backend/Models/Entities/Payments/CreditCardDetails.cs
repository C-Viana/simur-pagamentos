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
        public string LastFourDigits { get; set; }
        [Required]
        public string CardHolderName { get; set; }
        [Required]
        public DateOnly ExpirationDate { get; set; }
        [Required]
        [JsonConverter(typeof(GetPaymentBrandFromString))]
        public PaymentBrand Brand { get; set; }
        [Required]
        public int Installments { get; set; }
        [Required]
        public decimal InterestAmount { get; set; }

        public CreditCardDetails()
        {
            base.PaymentType = Constants.PaymentType.CREDIT_CARD;
        }
    }
}
