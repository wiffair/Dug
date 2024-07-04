using DNS_Checker.converters;
using System.Text.Json.Serialization;

namespace DNS_Checker.models
{
    public class DnsTestRecord
    {
        public DnsHeader QueryHeader { get; set; }
        public DnsQuestion QueryQuestion { get; set; }
        public string QueryServer { get; set; }
        public DateTime QueryDateTime { get; set; }
        public int QueryByteCount { get; set; }
        [JsonPropertyName("QueryRawData")]
        [JsonConverter(typeof(RawDataConverter))]
        public Dictionary<IndexXY, ushort> QueryRawData { get; set; } = new();
        public int QueryRetryCount { get; set; }
        public bool TimeOut { get; set; } = false;
        public bool Receive { get; set; } = false;
        public DnsHeader ResponseHeader { get; set; }
        public List<DnsQuestion> ResponseQuestions { get; } = new List<DnsQuestion>();
        public List<ResourceRecord> ResponseAnswers { get; } = new List<ResourceRecord>();
        public List<ResourceRecord> ResponseAuthorityRecords { get; } = new List<ResourceRecord>();
        public List<ResourceRecord> ResponseAdditionalRecords { get; } = new List<ResourceRecord>();
        public string ReplyServer { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public TimeSpan QueryTime { get; set; }
        public int ResponseByteCount { get; set; }
        [JsonPropertyName("ResponseRawData")]
        [JsonConverter(typeof(RawDataConverter))]
        public Dictionary<IndexXY, ushort> ResponseRawData { get; set; } = new();
        public Options Options { get; set; }
        public bool Visable { get; set; } = true;
    }
}
