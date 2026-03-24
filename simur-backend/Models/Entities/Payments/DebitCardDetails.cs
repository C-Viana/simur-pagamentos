using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("DebitCardDetails")]
    public class DebitCardDetails : IPaymentDetails
    {
        [Required]
        public string CardToken { get; init; }

        public DebitCardDetails()
        {
            base.PaymentType = PaymentType.DEBIT_CARD;
        }
    }
}
