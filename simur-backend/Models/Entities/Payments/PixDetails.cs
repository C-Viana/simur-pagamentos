using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("PixDetails")]
    public class PixDetails : IPaymentDetails
    {
        [Required]
        public string PixKey { get; set; }
        [Required]
        public string QrCodeBase64 { get; set; }
        [Required]
        public string PaymentUri { get; set; }
        [Required]
        public DateTime? ExpiresAt { get; set; }

        public PixDetails() {
            base.PaymentType = Constants.PaymentType.PIX;
        }
    }
}
