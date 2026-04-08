using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace simur_backend.Models.Entities
{
    public class Payment
    {
        [BsonId]
        public Guid Id { get; init; } = Guid.NewGuid();
        public string ExternalOrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        [EnumDataType(typeof(PaymentStatus))]
        [BsonRepresentation(BsonType.String)]
        public PaymentStatus Status { get; set; } = PaymentStatus.CREATED;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now.DateTime;
        public DateTimeOffset? UpdatedAt { get; set; }
        public string MerchantDocument { get; set; }
        public string CustomerDocument { get; set; }

        public Payment(Guid id, string externalOrderId, decimal amount, string currency, PaymentStatus status, DateTimeOffset createdAt, DateTimeOffset? updatedAt, string merchantDocument, string customerDocument)
        {
            Id = id;
            ExternalOrderId = externalOrderId;
            Amount = amount;
            Currency = currency;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            MerchantDocument = merchantDocument;
            CustomerDocument = customerDocument;
        }

        public Payment(Guid id, string externalOrderId, decimal amount, string currency, PaymentStatus status, DateTimeOffset createdAt, string merchantDocument, string customerDocument)
        {
            Id = id;
            ExternalOrderId = externalOrderId;
            Amount = amount;
            Currency = currency;
            Status = status;
            CreatedAt = createdAt;
            MerchantDocument = merchantDocument;
            CustomerDocument = customerDocument;
        }
    }
}
