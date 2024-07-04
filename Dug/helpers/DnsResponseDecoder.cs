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

using dug.enums;

namespace dug.helpers
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
