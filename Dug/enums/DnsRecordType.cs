

namespace DNS_Checker.enums
{
    [Flags]
    public enum DnsRecordType : ushort
    {
        A = 1,
        NS = 2,
        MD = 3,
        MF = 4,
        CNAME = 5,
        SOA = 6,
        MB = 7,
        MG = 8,
        MR = 9,
        NULL = 10,
        WKS = 11,
        PTR = 12,
        HINFO = 13,
        MINFO = 14,
        MX = 15,
        TXT = 16,
        AAAA = 28,
        AXFR = 252,
        MAILB = 253,
        MAILA = 254,
        ALL = 255
    }
    public enum DnsClassType : ushort
    {
        IN = 1,
        CS = 2,
        CH = 3,
        HS = 4
    }

    public enum ReplyCode : ushort
    {
        NoError = 0, // No error condition
        FormatError = 1, // Format error
        ServerFailure = 2, // Server failure
        NameError = 3, // Name error, meaning domain name was not found
        NotImplemented = 4, // Not implemented
        Refused = 5, // Query refused
    }

    public enum Opcode : ushort
    {
        Query = 0, // Standard query
        IQuery = 1, // Inverse query
        Status = 2, // Server status request
        Notify = 4, // Notify (used for DNS notify)
        Update = 5, // Dynamic update
    }
    [Flags]
    public enum ResponseFlags : ushort
    {
        None = 0,
        AA = 0x0400, // Authoritative answer
        TC = 0x0200, // Message truncated
        RD = 0x0100, // Recursion desired
        RA = 0x0080, // Recursion available
        AD = 0x0020, // Authenticated Data
        CD = 0x0010, // Checking Disabled
    }

    public enum RrType : int
    {
        Answer = 0,
        Authority = 1,
        Additional = 2
    }
}
