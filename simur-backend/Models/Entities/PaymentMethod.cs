using simur_backend.Models.Constants;
using simur_backend.Models.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities
{
    public class PaymentMethod
    {
        public PaymentMethod(Guid paymentId, PaymentType? paymentType, IPaymentDetails paymentDetails)
        {
            PaymentId = paymentId;
            PaymentType = paymentType;
            PaymentDetails = paymentDetails;
        }

        public PaymentMethod(long id, Guid paymentId, PaymentType paymentType, IPaymentDetails paymentDetails)
        {
            Id = id;
            PaymentId = paymentId;
            PaymentType = paymentType;
            PaymentDetails = paymentDetails;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [MaybeNull]
        [JsonPropertyName("payment_id")]
        public Guid? PaymentId { get; set; }

        [EnumDataType(typeof(PaymentType))]
        [MaybeNull]
        [JsonPropertyName("payment_type")]
        public PaymentType? PaymentType { get; set; }

        [JsonPropertyName("payment_details")]
        public IPaymentDetails PaymentDetails { get; set; }
    }
}
