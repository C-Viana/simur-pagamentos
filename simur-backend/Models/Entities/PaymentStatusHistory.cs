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
        [JsonConstructor]
        public PaymentStatusHistory(Guid paymentId, PaymentStatus status, string? reason, DateTimeOffset? changedAt)
        {
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
            ChangedAt = changedAt;
        }

        public PaymentStatusHistory(Guid id, Guid paymentId, PaymentStatus status, string? reason, DateTimeOffset changedAt)
        {
            Id = id;
            PaymentId = paymentId;
            Status = status;
            Reason = reason;
            ChangedAt = changedAt;
        }

        [BsonId]
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("payment_id")]
        [JsonPropertyName("payment_id")]
        public Guid PaymentId { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(GetPaymentStatusFromString))]
        [JsonPropertyName("status")]
        public PaymentStatus Status { get; set; }

        [MaybeNull]
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [MaybeNull]
        [JsonPropertyName("changed_at_date")]
        public DateTimeOffset? ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
