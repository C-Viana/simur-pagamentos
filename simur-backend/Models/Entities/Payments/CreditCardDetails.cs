using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using simur_backend.Models.DTO.V1;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("CreditCardDetails")]
    public class CreditCardDetails : IPaymentDetails
    {
        [Required]
        public int Installments { get; init; }
        [Required]
        public string CardToken { get; init; }

        public CreditCardDetails()
        {
            base.PaymentType = PaymentType.CREDIT_CARD;
        }
    }
}
