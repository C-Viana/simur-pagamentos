using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("payment_details")]
    [BsonKnownTypes(typeof(PixDetails), typeof(CreditCardDetails))]

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(PixDetails), "PIX")]
    [JsonDerivedType(typeof(CreditCardDetails), "CREDIT_CARD")]
    public abstract class IPaymentDetails
    {
        [EnumDataType(typeof(PaymentType))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore]
        public Constants.PaymentType PaymentType { get; set; }
    }
}
