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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace dug.converters
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
