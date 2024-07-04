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

using dug.converters;
using System.Net;
using System.Text.Json.Serialization;

namespace dug.models
{
    public class TypeAAAA
    {
        public string Name { get; set; }
        [JsonPropertyName("IPAdress")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress? ipAddress { get; set; }
    }
}
