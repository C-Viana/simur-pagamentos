using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using simur_backend.Models.Deserealizers;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities
{
    public class PaymentStatusHistory
    {

        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();

        [Required]
        [BsonElement("payment_id")]
        public Guid PaymentId { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(GetPaymentStatusFromString))]
        public PaymentStatus Status { get; set; }

        [MaybeNull]
        public string Reason { get; set; }

        [MaybeNull]
        public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;

        [JsonConstructor]
        public PaymentStatusHistory(Guid paymentId, PaymentStatus status, string reason, DateTimeOffset changedAt)
        {
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
            ChangedAt = changedAt;
        }

        public PaymentStatusHistory(Guid id, Guid paymentId, PaymentStatus status, string reason, DateTimeOffset changedAt)
        {
            Id = id;
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
            ChangedAt = changedAt;
        }
    }
}
