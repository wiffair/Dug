using DNS_Checker.converters;
using System.Net;
using System.Text.Json.Serialization;

namespace DNS_Checker.models
{
    public class TypeA
    {
        public string Name { get; set; }
        [JsonPropertyName("IPAdress")]
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress? ipAddress { get; set; }
    }
}
