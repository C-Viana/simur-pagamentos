using System.Text.Json;
using System.Text.Json.Serialization;

namespace simur_backend.Models.Deserealizers
{
    public class StringEnumSerializer<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                if (Nullable.GetUnderlyingType(typeToConvert) != null)
                    return default;
                throw new JsonException($"Cannot convert null to non-nullable enum {typeToConvert.Name}");
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected string for enum {typeToConvert.Name}, got {reader.TokenType}");
            }

            string enumString = reader.GetString();
            if (Enum.TryParse<T>(enumString, ignoreCase: true, out T value))
            {
                return value;
            }

            throw new JsonException($"Invalid enum value '{enumString}' for {typeToConvert.Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
