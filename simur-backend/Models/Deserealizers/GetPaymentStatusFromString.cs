using simur_backend.Models.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Deserealizers
{
    public class GetPaymentStatusFromString : JsonConverter<PaymentStatus>
    {
        public override PaymentStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (PaymentStatus.TryParse(reader.GetString(), out PaymentStatus status))
                {
                    return status;
                }
            }
            throw new Exception("Invalid payment status");
        }

        public override void Write(Utf8JsonWriter writer, PaymentStatus value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
