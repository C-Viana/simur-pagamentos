using simur_backend.Models.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Deserealizers
{
    public class GetPaymentBrandFromString : JsonConverter<PaymentBrand>
    {
        public override PaymentBrand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (PaymentBrand.TryParse(reader.GetString(), out PaymentBrand brand))
                {
                    return brand;
                }
            }
            throw new Exception("Invalid credit card brand");
        }

        public override void Write(Utf8JsonWriter writer, PaymentBrand value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
