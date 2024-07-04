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
using System.Text.Json.Serialization;

namespace dug.models
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
