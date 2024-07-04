using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DNS_Checker.enums;

namespace DNS_Checker.helpers
{

    public static class DnsResponseDecoder
    {
        // RFC 1035 - Domain Names - Implementation and Specification
        // Section 4.1.1. ResponseHeader section format

        public static ReplyCode GetReplyCode(ushort flags)
        {
            return (ReplyCode)(flags & 0xF);
        }

        public static Opcode GetOpcode(ushort flags)
        {
            return (Opcode)((flags >> 11) & 0xF);
        }

        public static ResponseFlags GetResponseFlags(ushort flags)
        {
            ResponseFlags responseFlags = ResponseFlags.None;
            if ((flags & 0x0400) != 0)
            {
                responseFlags |= ResponseFlags.AA;
            }

            if ((flags & 0x0200) != 0)
            {
                responseFlags |= ResponseFlags.TC;
            }

            if ((flags & 0x0100) != 0)
            {
                responseFlags |= ResponseFlags.RD;
            }

            if ((flags & 0x0080) != 0)
            {
                responseFlags |= ResponseFlags.RA;
            }

            if ((flags & 0x0020) != 0)
            {
                responseFlags |= ResponseFlags.AD;
            }

            if ((flags & 0x0010) != 0)
            {
                responseFlags |= ResponseFlags.CD;
            }

            return responseFlags;
        }

        public static string GetMessage(ReplyCode replyCode)
        {
            switch (replyCode)
            {
                case ReplyCode.NoError:
                    return "No error condition";
                case ReplyCode.FormatError:
                    return "Format error";
                case ReplyCode.ServerFailure:
                    return "Server failure";
                case ReplyCode.NameError:
                    return "Name error, meaning domain name was not found";
                case ReplyCode.NotImplemented:
                    return "Not implemented";
                case ReplyCode.Refused:
                    return "Query refused";
                default:
                    return "Unknown reply code";
            }
        }
    }

}
