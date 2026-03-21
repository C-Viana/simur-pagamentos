using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using simur_backend.Models.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace simur_backend.Models.Entities
{
    public class PaymentMethod
    {

        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [MaybeNull]
        public Guid? PaymentId { get; set; }

        [EnumDataType(typeof(PaymentType))]
        [BsonRepresentation(BsonType.String)]
        public PaymentType PaymentType { get; set; }

        public IPaymentDetails PaymentDetails { get; set; }

        public PaymentMethod(Guid paymentId, PaymentType paymentType, IPaymentDetails paymentDetails)
        {
            PaymentId = paymentId;
            PaymentType = paymentType;
            PaymentDetails = paymentDetails;
        }

        public PaymentMethod(Guid id, Guid paymentId, PaymentType paymentType, IPaymentDetails paymentDetails)
        {
            Id = id;
            PaymentId = paymentId;
            PaymentType = paymentType;
            PaymentDetails = paymentDetails;
        }
    }
}
