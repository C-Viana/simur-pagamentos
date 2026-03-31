using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class PaymentDto
    {
        [BsonId]
        public Guid Id { get; set; }

        [NotNull]
        [Range(0.0, 9999999.99)]
        public decimal Amount { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now.DateTime;

        [Required]
        [NotNull]
        public string Currency { get; set; }

        [Required]
        [NotNull]
        public string ExternalOrderId { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus Status { get; set; } = PaymentStatus.CREATED;

        [Required]
        [NotNull]
        public string PayerDocument { get; set; }

        [Required]
        [NotNull]
        public string SellerDocument { get; set; }

        [Required]
        public IPaymentDetails PaymentDetails { get; set; }
    }
}
