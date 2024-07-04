using DNS_Checker.enums;
namespace DNS_Checker.models
{
    public class DnsQuestion
    {
        public string DomainName { get; set; }
        public DnsRecordType RecordType { get; set; }
        public DnsClassType Class { get; set; }
    }
}
