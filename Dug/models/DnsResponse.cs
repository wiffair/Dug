using DNS_Checker.enums;
using DNS_Checker.converters;
using System.Text.Json.Serialization;

namespace DNS_Checker.models
{
    public class DnsResponse
    {
        public DnsHeader QueryHeader { get; set; }
        public List<DnsQuestion> QueryQuestions { get; set; } = new List<DnsQuestion>();
        public DateTime QueryDateTime { get; set; }
        [JsonPropertyName("QueryRawData")]
        [JsonConverter(typeof(RawDataConverter))]
        public Dictionary<IndexXY, ushort> QueryRawData { get; set; } = new();
        public int QueryByteCount { get; set; }
        public DnsHeader ResponseHeader { get; set; }
        public List<DnsQuestion> ResponseQuestions { get; } = new List<DnsQuestion>();
        public List<ResourceRecord> ResponseAnswers { get; } = new List<ResourceRecord>();
        public List<ResourceRecord> ResponseAuthorityRecords { get; } = new List<ResourceRecord>();
        public List<ResourceRecord> ResponseAdditionalRecords { get; } = new List<ResourceRecord>();
        public string Server { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public TimeSpan QueryTime { get; set; }
        [JsonPropertyName("ResponseRawData")]
        [JsonConverter(typeof(RawDataConverter))]
        public Dictionary<IndexXY, ushort> ResponseRawData { get; set; } = new();
        public int ResponseByteCount { get; set; }

    }
}
