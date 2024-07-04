using DNS_Checker.dataContainers;
using DNS_Checker.enums;
using DNS_Checker.models;
using System.Net;
using System.Text;

namespace DNS_Checker.helpers
{
    public class ParseDnsPacket
    {
        private readonly DataContainer _dataContainer;
        public ParseDnsPacket(DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
        }

        public async Task<int> ParseDnsQuery(byte[] queryBytes, DateTime dsTimeStamp, string dnsServer, Options options, bool visable = true)
        {
            DnsTestRecord response = new DnsTestRecord();
            response.Visable = visable;
            response.QueryDateTime = dsTimeStamp;
            response.QueryServer = dnsServer;
            response.Options = options;
            response.QueryByteCount = queryBytes.Length;
            using MemoryStream stream = new MemoryStream(queryBytes);
            using BinaryReader reader = new BinaryReader(stream);


            ushort y = 0;
            ushort x = 0;
            ushort[] raw = new ushort[queryBytes.Length];
            for (int i = 0; i < queryBytes.Length; i++)
            {
                ushort value = BitConverter.ToUInt16(new byte[] { queryBytes[i], 0 }, 0);
                raw[i] = value;
                IndexXY index = new(x, y);
                response.QueryRawData.Add(index, value);
                x++;
                if (x == 16)
                {
                    y++;
                    x = 0;
                }
            }

            // ResponseHeader

            response.QueryHeader = ReadHeader(reader);

            // QueryQuestion
                response.QueryQuestion=ReadQuestion(reader);

            return _dataContainer.AddData(response);
        }

        public async Task ParseDnsResponse(byte[] responseBytes, DateTime dsTimeStamp, IPAddress remoteIpAddress)
        {
            DnsTestRecord response = new();
            response.ResponseByteCount = responseBytes.Length;
            response.ResponseDateTime = dsTimeStamp;
            response.ReplyServer = remoteIpAddress.ToString();


            //response.QueryTime = response.ResponseDateTime.Subtract(response.QueryDateTime);
            response.Receive = true;
            using MemoryStream stream = new MemoryStream(responseBytes);
            using BinaryReader reader = new BinaryReader(stream);


            ushort y = 0;
            ushort x = 0;
            ushort[] raw = new ushort[responseBytes.Length];
            for (int i = 0; i < responseBytes.Length; i++)
            {
                ushort value = BitConverter.ToUInt16(new byte[] { responseBytes[i], 0 }, 0);
                raw[i] = value;
                IndexXY index = new(x, y);
                response.ResponseRawData.Add(index, value);
                x++;
                if (x == 16)
                {
                    y++;
                    x = 0;
                }

            }

            // ResponseHeader

            response.ResponseHeader = ReadHeader(reader);

            // QueryQuestion
            for (int i = 0; i < response.ResponseHeader.QuestionCount; i++)
            {
                response.ResponseQuestions.Add(ReadQuestion(reader));
            }

            // ResponseAnswers
            for (int i = 0; i < response.ResponseHeader.AnswerCount; i++)
            {
                response.ResponseAnswers.Add(ReadResourceRecord(RrType.Answer, stream, reader));
            }
            // Authority Records
            for (int i = 0; i < response.ResponseHeader.AuthorityCount; i++)
            {
                response.ResponseAuthorityRecords.Add(ReadResourceRecord(RrType.Authority, stream, reader));
            }
            // Additional Records
            for (int i = 0; i < response.ResponseHeader.AdditionalCount; i++)
            {
                response.ResponseAdditionalRecords.Add(ReadResourceRecord(RrType.Additional, stream, reader));
            }
            _dataContainer.MergeData(response);
            //return response;
        }

        public static DnsHeader ReadHeader(BinaryReader reader)
        {
            DnsHeader header = new DnsHeader();

            byte[] idBytes = reader.ReadBytes(2);
            header.Id = (ushort)((idBytes[0] << 8) | idBytes[1]);

            byte[] FlagsBytes = reader.ReadBytes(2);
            header.Flags = (ushort)((FlagsBytes[0] << 8) | FlagsBytes[1]);
            if ((byte)(FlagsBytes[0] >> 7) == 1) { header.QR = 'R'; } else { header.QR = 'Q'; };
            header.replyCode = DnsResponseDecoder.GetReplyCode(header.Flags);
            header.opcode = DnsResponseDecoder.GetOpcode(header.Flags);
            header.responseFlags = DnsResponseDecoder.GetResponseFlags(header.Flags);
            header.errorMessage = DnsResponseDecoder.GetMessage(header.replyCode);

            byte[] QuestionCountBytes = reader.ReadBytes(2);
            header.QuestionCount = (ushort)((QuestionCountBytes[0] << 8) | QuestionCountBytes[1]);

            byte[] AnswerCountBytes = reader.ReadBytes(2);
            header.AnswerCount = (ushort)((AnswerCountBytes[0] << 8) | AnswerCountBytes[1]);

            byte[] AuthorityCountBytes = reader.ReadBytes(2);
            header.AuthorityCount = (ushort)((AuthorityCountBytes[0] << 8) | AuthorityCountBytes[1]);

            byte[] AdditionalCountBytes = reader.ReadBytes(2);
            header.AdditionalCount = (ushort)((AdditionalCountBytes[0] << 8) | AdditionalCountBytes[1]);

            return header;
        }

        public static DnsQuestion ReadQuestion(BinaryReader reader)
        {
            // QueryQuestion
            DnsQuestion question = new DnsQuestion();
            question.DomainName = ReadDomainName(reader);

            byte[] RecordTypeBytes = new byte[2];
            reader.Read(RecordTypeBytes, 0, 2);
            int RecordType = (ushort)((RecordTypeBytes[0] << 8) | RecordTypeBytes[1]);
            question.RecordType = (DnsRecordType)RecordType;


            byte[] ClassBytes = reader.ReadBytes(2);
            int Class = (ushort)((ClassBytes[0] << 8) | ClassBytes[1]);
            question.Class = (DnsClassType)Class;

            return question;
        }

        public static ResourceRecord ReadResourceRecord(RrType rrType, MemoryStream stream, BinaryReader reader)
        {
            ResourceRecord record = new ResourceRecord();
            record.RrType = rrType;
            record.Name = ReadDomainName(reader);

            byte[] RecordTypeBytes = new byte[2];
            reader.Read(RecordTypeBytes, 0, 2);
            int RecordType = (ushort)((RecordTypeBytes[0] << 8) | RecordTypeBytes[1]);
            record.Type = (DnsRecordType)RecordType;

            byte[] ClassBytes = reader.ReadBytes(2);
            int Class = (ushort)((ClassBytes[0] << 8) | ClassBytes[1]);
            record.Class = (DnsClassType)Class;

            byte[] TtlBytes = reader.ReadBytes(4);
            record.Ttl = (uint)((TtlBytes[0] << 24) | (TtlBytes[1] << 16) | (TtlBytes[2] << 8) | TtlBytes[3]);


            byte[] dataLengthBytes = reader.ReadBytes(2);
            int dataLength = (ushort)((dataLengthBytes[0] << 8) | dataLengthBytes[1]);

            // Check for end-of-stream condition
            if (stream.Position + dataLength > stream.Length)
            {
                throw new Exception("Invalid DNS response: Answer length exceeds remaining stream length");
            }

            try
            {
                switch (record.Type)
                {
                    case DnsRecordType.A:
                        string ipv4address = ReadA(reader, dataLength);
                        IPAddress.TryParse(ipv4address, out IPAddress? ipav4);
                        record.TypeA = new();
                        record.TypeA.Name = record.Name;
                        record.TypeA.ipAddress = ipav4;
                        record.RData = ipv4address;
                        break;
                    case DnsRecordType.AAAA:
                        string ipv6address = ReadA(reader, dataLength);
                        IPAddress.TryParse(ipv6address, out IPAddress? ipav6);
                        record.TypeAAAA = new();
                        record.TypeAAAA.Name = record.Name;
                        record.TypeAAAA.ipAddress = ipav6;
                        record.RData = ipv6address;
                        break;
                    case DnsRecordType.NS:
                        string nameserver = ReadDomainName(reader);
                        record.TypeNS = new();
                        record.TypeNS.Name = record.Name;
                        record.TypeNS.NameServer = nameserver;
                        record.RData = nameserver;
                        break;
                    case DnsRecordType.CNAME:
                        string cname = ReadDomainName(reader);
                        record.TypeCName = new();
                        record.TypeCName.Name = record.Name;
                        record.TypeCName.CName = cname;
                        record.RData = cname;
                        break;
                    case DnsRecordType.PTR:
                        string ptrname = ReadDomainName(reader);
                        record.TypePTR = new();
                        record.TypePTR.Name = record.Name;
                        record.TypePTR.PTRName = ptrname;
                        record.RData = ptrname;
                        break;
                    case DnsRecordType.SOA:
                        record.TypeSOA = new();
                        record.TypeSOA.Name = record.Name;
                        record.TypeSOA.Mname = ReadDomainName(reader);
                        record.TypeSOA.Rname = ReadDomainName(reader);
                        record.TypeSOA.SerialNumber = Read32Bit(reader);
                        record.TypeSOA.RefreshInterval = Read32Bit(reader);
                        record.TypeSOA.RetryInterval = Read32Bit(reader);
                        record.TypeSOA.ExpireLimit = Read32Bit(reader);
                        record.TypeSOA.MinimumTTL = Read32Bit(reader);
                        break;
                    default:
                        Console.WriteLine($"Unknown Type: {record.Type}");
                        break;

                }


                return record;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return record;
            }
        }

        public static string ReadA(BinaryReader reader, int dataLength)
        {
            byte[] answer = reader.ReadBytes(dataLength);
            string rData = new IPAddress(answer).ToString();
            return rData;
        }

        public static UInt32 Read32Bit(BinaryReader reader)
        {
            byte[] readBits = reader.ReadBytes(4);
            UInt32 bits = (uint)((readBits[0] << 24) | (readBits[1] << 16) | (readBits[2] << 8) | readBits[3]);

            return bits;
        }
        public static string ReadDomainName(BinaryReader reader)
        {
            StringBuilder domainName = new StringBuilder();
            long currentPosition = 0;
            while (true)
            {
                byte labelLength = reader.ReadByte();
                if (labelLength == 0) break; // end of domain name

                if (labelLength >= 0xC0) // pointer to another location
                {
                    ushort offset = (ushort)((labelLength & 0x3F) << 8 | reader.ReadByte());
                    if (currentPosition == 0) currentPosition = reader.BaseStream.Position;
                    reader.BaseStream.Position = offset;
                    continue;
                }

                byte[] labelBytes = reader.ReadBytes(labelLength);
                domainName.Append(Encoding.ASCII.GetString(labelBytes));
                domainName.Append(".");
            }
            if (currentPosition > 0)
            {
                reader.BaseStream.Position = currentPosition;
            }
            return $"{domainName.ToString().TrimEnd('.')}.";
        }





    }
}
