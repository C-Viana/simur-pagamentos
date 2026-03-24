using MongoDB.Bson;
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
        typeof(BoletoDetails),
        typeof(DebitCardDetails)
        )]

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(PixDynamicDetails), "PIX_DYNAMIC") ]
    [JsonDerivedType(typeof(PixStaticDetails), "PIX_STATIC")]
    [JsonDerivedType(typeof(CreditCardDetails), "CREDIT_CARD")]
    [JsonDerivedType(typeof(BoletoDetails), "BOLETO")]
    [JsonDerivedType(typeof(DebitCardDetails), "DEBIT_CARD")]
    public abstract class IPaymentDetails
    {
        [BsonRepresentation(BsonType.String)]
        [EnumDataType(typeof(PaymentType))]
        [JsonIgnore]
        public Constants.PaymentType PaymentType { get; set; }
    }
}
