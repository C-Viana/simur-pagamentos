using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Entities.Payments
{
    [BsonDiscriminator("payment_type")]
    [BsonKnownTypes(typeof(PixDetails), typeof(CreditCardDetails))]

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "payment_type")]
    [JsonDerivedType(typeof(PixDetails), "PIX")]
    [JsonDerivedType(typeof(CreditCardDetails), "CREDIT_CARD")]
    public abstract class IPaymentDetails
    {
    }
}
