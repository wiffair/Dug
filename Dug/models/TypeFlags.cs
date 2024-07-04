namespace DNS_Checker.models
{
    public class TypeFlags
    {
        public bool A { get; set; }
        public bool AAAA { get; set; }
        public bool NS { get; set; }
        public bool MD { get; set; }
        public bool MF { get; set; }
        public bool CNAME { get; set; }
        public bool SOA { get; set; }
        public bool MB { get; set; }
        public bool MG { get; set; }
        public bool MR { get; set; }
        public bool NULL { get; set; }
        public bool WKS { get; set; }
        public bool PTR { get; set; }
        public bool HINFO { get; set; }
        public bool MINFO { get; set; }
        public bool MX { get; set; }
        public bool TXT { get; set; }
        public bool AXFR { get; set; }
        public bool MAILB { get; set; }
        public bool MAILA { get; set; }
        public bool ALL { get; set; }

        public bool IsFlagSet => A || AAAA || NS || MD || MF || CNAME || SOA || MB || MG || MR || NULL || WKS || PTR || HINFO || MINFO || MX || TXT || AXFR || MAILB || MAILA || ALL;
 
    }
}
