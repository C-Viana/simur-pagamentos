using simur_backend.Models.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Deserealizers
{
    public class GetPaymentTypeFromString : JsonConverter<PaymentType>
    {
        public override PaymentType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.String)
            {
                if(PaymentType.TryParse(reader.GetString(), out PaymentType type))
                {
                    return type;
                }
            }
            throw new Exception("Invalid payment type");
        }

        public override void Write(Utf8JsonWriter writer, PaymentType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
