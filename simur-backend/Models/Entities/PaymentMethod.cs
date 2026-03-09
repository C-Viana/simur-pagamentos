using simur_backend.Models.Constants;
using simur_backend.Models.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.Entities
{
    public class PaymentMethod
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; init; }

        [MaybeNull]
        public Guid? PaymentId { get; set; }

        [EnumDataType(typeof(PaymentType))]
        public PaymentType PaymentType { get; set; }

        public IPaymentDetails PaymentDetails { get; set; }

        public PaymentMethod(Guid paymentId, PaymentType paymentType, IPaymentDetails paymentDetails)
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
    }
}
