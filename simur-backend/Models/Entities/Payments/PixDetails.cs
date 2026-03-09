using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("PixDetails")]
    public class PixDetails : IPaymentDetails
    {
        [Required]
        [JsonPropertyName("pix_key")]
        public string PixKey { get; set; }
        [Required]
        [JsonPropertyName("qr_code")]
        public string QrCodeBase64 { get; set; }
        [Required]
        [JsonPropertyName("payment_uri")]
        public string PaymentLink { get; set; }
        [Required]
        [JsonPropertyName("expiration")]
        public DateTime? ExpiresAt { get; set; }

        public PixDetails() { }

        public PixDetails(string pixKey, string qrCodeBase64, string paymentLink, DateTime? expiresAt)
        {
            PixKey = pixKey;
            QrCodeBase64 = qrCodeBase64;
            PaymentLink = paymentLink;
            ExpiresAt = expiresAt;
        }
    }
}
