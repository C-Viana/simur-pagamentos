using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities
{
    public class Payment
    {
        public Payment(Guid id, string externalOrderId, decimal amount, string currency, PaymentType methodType, PaymentStatus status, DateTimeOffset createdAt, DateTimeOffset? updatedAt, string merchantDocument, string customerDocument)
        {
            Id = id;
            ExternalOrderId = externalOrderId;
            Amount = amount;
            Currency = currency;
            MethodType = methodType;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            MerchantDocument = merchantDocument;
            CustomerDocument = customerDocument;
        }

        public Payment(Guid id, string externalOrderId, decimal amount, string currency, PaymentType methodType, PaymentStatus status, DateTimeOffset createdAt, string merchantDocument, string customerDocument)
        {
            Id = id;
            ExternalOrderId = externalOrderId;
            Amount = amount;
            Currency = currency;
            MethodType = methodType;
            Status = status;
            CreatedAt = createdAt;
            MerchantDocument = merchantDocument;
            CustomerDocument = customerDocument;
        }

        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [NotNull]
        public string ExternalOrderId { get; set; }

        [Required]
        [NotNull]
        [Range(0.0, 9999999.99)]
        public decimal Amount { get; set; }

        [Required]
        [NotNull]
        public string Currency { get; set; }

        [Required]
        [NotNull]
        [EnumDataType(typeof(PaymentType))]
        public PaymentType MethodType { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus Status { get; set; } = PaymentStatus.CREATED;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now.DateTime;
        public DateTimeOffset? UpdatedAt { get; set; }

        [Required]
        [NotNull]
        public string MerchantDocument { get; set; }

        [Required]
        [NotNull]
        public string CustomerDocument { get; set; }
    }
}
