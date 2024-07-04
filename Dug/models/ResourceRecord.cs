using DNS_Checker.enums;
namespace DNS_Checker.models
{
    public class ResourceRecord
    {
        public RrType RrType { get; set; }
        public string Name { get; set; }
        public DnsRecordType Type { get; set; }
        public DnsClassType Class { get; set; }
        public uint Ttl { get; set; }
        public ushort RdLength { get; set; }
        public string RData { get; set; }
        public TypeA? TypeA { get; set; }
        public TypeAAAA? TypeAAAA { get; set; }
        public TypeNS? TypeNS { get; set; }
        public TypeCName? TypeCName { get; set; }
        public TypePTR? TypePTR { get; set; }
        public TypeSOA? TypeSOA { get; set; }
    }
}
