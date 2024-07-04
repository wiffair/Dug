using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNS_Checker.converters
{
    public class HexConverter : JsonConverter<ushort[]>
    {
        public override ushort[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //Not Implemented, only need to Write
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ushort[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach(ushort item in value)
            {
                writer.WriteStringValue(item.ToString("X2"));
            }
            writer.WriteEndArray();
        }
    }
}
