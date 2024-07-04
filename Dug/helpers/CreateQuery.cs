using DNS_Checker.enums;
using System.Text;
using DNS_Checker.models;

namespace DNS_Checker.helpers
{
    public class CreateQuery
    {
        //static ushort idCount = 0x0001;
        public static byte[] CreateDnsQuery(string domainName, DnsRecordType recordType, ResponseFlagsModel responseFlagsModel, ushort idCount)
        {
            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);

            // ResponseHeader
            ushort id = idCount;
            //idCount++;
            byte[] idBytes = BitConverter.GetBytes(id);
            Array.Reverse(idBytes); // Convert to big-endian
            writer.Write(idBytes);

            ushort flags = (ushort)SetResponseFlags(responseFlagsModel); //0x0100; // Flags (standard query)    
            byte[] flagsBytes = BitConverter.GetBytes(flags);
            Array.Reverse(flagsBytes); // Convert to big-endian
            writer.Write(flagsBytes);

            ushort questionCount = 1; // QueryQuestion count
            byte[] questionCountBytes = BitConverter.GetBytes(questionCount);
            Array.Reverse(questionCountBytes); // Convert to big-endian
            writer.Write(questionCountBytes);

            ushort answerCount = 0; // Answer count
            byte[] answerCountBytes = BitConverter.GetBytes(answerCount);
            Array.Reverse(answerCountBytes); // Convert to big-endian
            writer.Write(answerCountBytes);

            ushort authorityCount = 0; // Authority count
            byte[] authorityCountBytes = BitConverter.GetBytes(authorityCount);
            Array.Reverse(authorityCountBytes); // Convert to big-endian
            writer.Write(authorityCountBytes);

            ushort additionalCount = 0; // Additional count
            byte[] additionalCountBytes = BitConverter.GetBytes(additionalCount);
            Array.Reverse(additionalCountBytes); // Convert to big-endian
            writer.Write(additionalCountBytes);

            // QueryQuestion section
            WriteDomainName(writer, domainName);
            ushort recordTypeValue = (ushort)recordType; // Record type
            byte[] recordTypeBytes = BitConverter.GetBytes(recordTypeValue);
            Array.Reverse(recordTypeBytes); // Convert to big-endian
            writer.Write(recordTypeBytes);

            ushort dnsClass = 1; // Class (IN)
            byte[] dnsClassBytes = BitConverter.GetBytes(dnsClass);
            Array.Reverse(dnsClassBytes); // Convert to big-endian
            writer.Write(dnsClassBytes);

            return stream.ToArray();
        }

        public static void WriteDomainName(BinaryWriter writer, string domainName)
        {
            foreach (string label in domainName.Split('.'))
            {
                if (label.Length > 0)
                {
                    byte[] labelBytes = Encoding.ASCII.GetBytes(label);
                    writer.Write((byte)labelBytes.Length);
                    writer.Write(labelBytes);
                }
            }
            writer.Write((byte)0); // Null terminator
        }

        public static ResponseFlags SetResponseFlags(ResponseFlagsModel flags)
        {
            ResponseFlags responseFlags = ResponseFlags.None;

            if (flags.AA)
            {
                responseFlags |= ResponseFlags.AA;
            }

            if (flags.TC)
            {
                responseFlags |= ResponseFlags.TC;
            }

            if (flags.RD)
            {
                responseFlags |= ResponseFlags.RD;
            }

            if (flags.RA)
            {
                responseFlags |= ResponseFlags.RA;
            }
            if (flags.AD)
            {
                responseFlags |= ResponseFlags.AD;
            }
            if (flags.CD)
            {
                responseFlags |= ResponseFlags.CD;
            }
            return responseFlags;
        }
    }
}
