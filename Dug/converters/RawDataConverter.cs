using DNS_Checker.models;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace DNS_Checker.converters
{
    public  class RawDataConverter : JsonConverter<Dictionary<IndexXY, ushort>>
    {
        public override Dictionary<IndexXY,ushort> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<IndexXY, ushort> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            ushort? CurrentY=null;
            string row=string.Empty;
            foreach (var kvp in value)
            {
                if (CurrentY==null)
                {
                    //row = "       x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 xA xB xC xD xE xF";
                    //writer.WriteStringValue(row);
                    //writer.Flush();
                    CurrentY = kvp.Key.Y;
                    row = $"{((ushort)CurrentY).ToString("X4")}  ";
                }

                if (kvp.Key.Y == CurrentY)
                {
                    row = $"{row} {kvp.Value.ToString("X2")}";
                }
                else
                {
                    writer.WriteStringValue(row);
                    writer.Flush();
                    CurrentY = kvp.Key.Y;
                    row = $"{((ushort)CurrentY).ToString("X4")}   {kvp.Value.ToString("X2")}";
                }


            }
            writer.WriteStringValue(row);
            writer.Flush();
            writer.WriteEndArray();
        }
    }
}
