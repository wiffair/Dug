// Dug is a DNS lookup tool
// Copyright(C) 2024  Richard Cole
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

using dug.models;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace dug.converters
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
