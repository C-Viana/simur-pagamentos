using MongoDB.Bson.Serialization.Attributes;
using simur_backend.Models.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("payment_details")]
    [BsonKnownTypes(
        typeof(PixDynamicDetails),
        typeof(CreditCardDetails),
        typeof(BoletoDetails)
        )]

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(PixDynamicDetails), "PIX")]
    [JsonDerivedType(typeof(CreditCardDetails), "CREDIT_CARD")]
    [JsonDerivedType(typeof(BoletoDetails), "BOLETO")]
    public abstract class IPaymentDetails
    {
        [EnumDataType(typeof(PaymentType))]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore]
        public Constants.PaymentType PaymentType { get; set; }
    }
}
