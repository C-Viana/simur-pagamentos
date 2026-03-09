using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Controllers.Utils;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities;
using simur_backend.Models.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.DTO.V1
{
    public class PaymentDto
    {
        [BsonId]
        [MaybeNull]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("amount")]
        [Range(0.0, 9999999.99)]
        public decimal Amount { get; set; }

        [JsonPropertyName("payment_date")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now.DateTime;

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("external_order_id")]
        public string ExternalOrderId { get; set; }

        [JsonPropertyName("payment_type")]
        [EnumDataType(typeof(PaymentType))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentType MethodType { get; set; }

        [JsonPropertyName("payment_status")]
        [EnumDataType(typeof(PaymentStatus))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentStatus Status { get; set; } = PaymentStatus.CREATED;

        [JsonPropertyName("seller_document")]
        public string MerchantDocument { get; set; }

        [JsonPropertyName("payer_document")]
        public string CustomerDocument { get; set; }

        [JsonPropertyName("payment_details")]
        public IPaymentDetails Details { get; set; }
    }
}
