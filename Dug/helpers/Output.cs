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

using dug.dataContainers;
using dug.enums;
using dug.models;
using System.Reflection;
using System.Text.Json;

namespace dug.helpers
{
    public class Output
    {
        private readonly DataContainer _dataContainer;

        public Output(DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
        }

        public void OutResults(Options options)
        {
            // Get the current assembly
            Assembly currentAssembly = Assembly.GetExecutingAssembly();

            // Get the assembly version
            string assemblyVersion = currentAssembly?.GetName()?.Version?.ToString() ?? "999.999.999.999";

            List<DnsTestRecord> dnsResponses = _dataContainer.ReadAllData();
            if (options.OutCsv)
            {
                //output cvs format
                Console.WriteLine($"Header_ID, Header_QR, Header_Opcode, Header_ResponseFlags(RA), Header_ResponseFlags(RD), Header_ResponseFlags(TC), Header_ResponseFlags(AA), ");
                foreach (DnsTestRecord dnsResponse in dnsResponses)
                {
                    string responseFlagsCsv = string.Join("\",\"",
                        from flag in Enum.GetValues(typeof(ResponseFlags)).Cast<ResponseFlags>().ToArray()
                        where (flag != ResponseFlags.None)
                        select (dnsResponse.ResponseHeader.responseFlags & flag) == flag ? Enum.GetName(typeof(ResponseFlags), flag) : "");


                    Console.WriteLine($"\"0x{dnsResponse.ResponseHeader.Id.ToString("X4")}\",\"{dnsResponse.ResponseHeader.QR}\",\"{dnsResponse.ResponseHeader.opcode}\",\"{responseFlagsCsv}\",\"\",\"\",\"");
                }
            }
            if (options.OutJson)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = System.Text.Json.JsonSerializer.Serialize(dnsResponses, jsonOptions);
                if (options.OutFileName != null)
                {
                    System.IO.File.WriteAllText(options.OutFileName, json);
                }
                else
                {
                    Console.WriteLine(json);
                }
            }
            if (options.OutConsole)
            {

                Console.WriteLine(Environment.NewLine + $"Dug v{assemblyVersion}");
                Console.WriteLine($"---------" + Environment.NewLine);

                foreach (DnsTestRecord dnsResponse in dnsResponses)
                {
                    if (dnsResponse.Visable)
                    {
                        if (options.ShowQuery)
                        {
                            Console.WriteLine("Query");
                            Console.WriteLine("-----");
                            Console.WriteLine("Header Section");
                            Console.WriteLine("--------------------");
                            Console.WriteLine($"ID: 0x{dnsResponse.QueryHeader.Id.ToString("X4")}");
                            Console.WriteLine($"QR: {dnsResponse.QueryHeader.QR}; Opcode: {dnsResponse.QueryHeader.opcode}; response flags: {dnsResponse.QueryHeader.responseFlags}; RCode: {dnsResponse.QueryHeader.replyCode} ({dnsResponse.QueryHeader.errorMessage})");
                            Console.WriteLine($"Query Count: {dnsResponse.QueryHeader.QuestionCount}; Answer: {dnsResponse.QueryHeader.AnswerCount}; Authority: {dnsResponse.QueryHeader.AuthorityCount}; Additional: {dnsResponse.QueryHeader.AdditionalCount}" + Environment.NewLine);
                            Console.WriteLine($"Question Section");
                            Console.WriteLine($"----------------");


                            Console.WriteLine($"{dnsResponse.QueryQuestion.DomainName.PadRight(40)}\t{dnsResponse.QueryQuestion.Class.ToString().PadRight(4)}\t{dnsResponse.QueryQuestion.RecordType}");
                            
                            if (options.Raw) DisplayRawData(dnsResponse.QueryRawData);
                            Console.WriteLine(Environment.NewLine + $"Query SERVER: {dnsResponse.QueryServer}");
                            Console.WriteLine($"WHEN: {dnsResponse.QueryDateTime.ToString("O")}" + Environment.NewLine);
                            Console.WriteLine($"MSG SIZE: {dnsResponse.QueryByteCount} Bytes");

                            Console.WriteLine();
                            Console.WriteLine("Response");
                            Console.WriteLine("--------");
                        }

                        if (dnsResponse.Receive)
                        {
                            Console.WriteLine("Header Section");
                            Console.WriteLine("-----------------------");
                            Console.WriteLine($"ID: 0x{dnsResponse.ResponseHeader.Id.ToString("X4")}");
                            Console.WriteLine($"QR: {dnsResponse.ResponseHeader.QR}; Opcode: {dnsResponse.ResponseHeader.opcode}; response flags: {dnsResponse.ResponseHeader.responseFlags}; RCode: {dnsResponse.ResponseHeader.replyCode} ({dnsResponse.ResponseHeader.errorMessage})");
                            Console.WriteLine($"Query Count: {dnsResponse.ResponseHeader.QuestionCount}; Answer: {dnsResponse.ResponseHeader.AnswerCount}; Authority: {dnsResponse.ResponseHeader.AuthorityCount}; Additional: {dnsResponse.ResponseHeader.AdditionalCount}" + Environment.NewLine);
                            if (!options.OutHeadersOnly)
                            {
                                Console.WriteLine($"Question Section");
                                Console.WriteLine($"----------------");

                                foreach (DnsQuestion dnsQuestion in dnsResponse.ResponseQuestions)
                                {
                                    Console.WriteLine($"{dnsQuestion.DomainName.PadRight(40)}\t{dnsQuestion.Class.ToString().PadRight(4)}\t{dnsQuestion.RecordType}");
                                }
                            }

                            if (!options.OutHeadersOnly && dnsResponse.ResponseHeader.AnswerCount > 0)
                            {
                                Console.WriteLine(Environment.NewLine + $"Answer Section");
                                Console.WriteLine("--------------");
                                foreach (ResourceRecord record in dnsResponse.ResponseAnswers)
                                {
                                    Display(record);
                                }
                            }

                            if (!options.OutHeadersOnly && dnsResponse.ResponseHeader.AuthorityCount > 0)
                            {
                                Console.WriteLine(Environment.NewLine + $"Authority Section");
                                Console.WriteLine("-----------------");
                                foreach (ResourceRecord record in dnsResponse.ResponseAuthorityRecords)
                                {
                                    Display(record);
                                }
                            }

                            if (!options.OutHeadersOnly && dnsResponse.ResponseHeader.AdditionalCount > 0)
                            {
                                Console.WriteLine(Environment.NewLine + $"Additional Section");
                                Console.WriteLine("------------------");
                                foreach (ResourceRecord record in dnsResponse.ResponseAdditionalRecords)
                                {
                                    Display(record);
                                }
                            }
                            if (options.Raw) DisplayRawData(dnsResponse.ResponseRawData);
                            if (!options.OutHeadersOnly)
                            {
                                Console.WriteLine(Environment.NewLine + $"Query Time: {(int)dnsResponse.QueryTime.TotalMilliseconds} msec");
                                Console.WriteLine($"Retry Count: {dnsResponse.QueryRetryCount}");
                                Console.WriteLine($"Reply SERVER: {dnsResponse.ReplyServer}");
                                Console.WriteLine($"WHEN: {dnsResponse.ResponseDateTime.ToString("O")}" + Environment.NewLine);
                                Console.WriteLine($"MSG SIZE: {dnsResponse.ResponseByteCount} Bytes"+Environment.NewLine);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No Response from {dnsResponse.QueryServer} for lookup on '{dnsResponse.QueryQuestion.DomainName}'");
                        }
                    }
                }
            }
        }



        private static void Display(ResourceRecord record)
        {
            switch (record.Type)
            {
                case DnsRecordType.A:
                    DisplayA(record);
                    break;
                case DnsRecordType.AAAA:
                    DisplayAAAA(record);
                    break;
                case DnsRecordType.NS:
                    DisplayNS(record);
                    break;
                case DnsRecordType.CNAME:
                    DisplayCName(record);
                    break;
                case DnsRecordType.PTR:
                    DisplayPTR(record);
                    break;
                case DnsRecordType.SOA:
                    DisplaySOA(record);
                    break;
                default:
                    Console.WriteLine($"Unknown Record Type {record.Type}");
                    break;
            }
        }
        private static void DisplayA(ResourceRecord record)
        {
            if (record.TypeA != null)
            {
                string ipaddress = "Error";
                if (record.TypeA.ipAddress != null) ipaddress = record.TypeA.ipAddress.ToString();
                Console.WriteLine($"{record.TypeA.Name.PadRight(32)}\t{record.Ttl.ToString().PadRight(4)}\t{record.Class.ToString().PadRight(4)}\t{record.Type.ToString().PadRight(4)}\t{ipaddress}");
            }
            else Console.WriteLine("Error");
        }
        private static void DisplayAAAA(ResourceRecord record)
        {
            if (record.TypeAAAA != null)
            {
                string ipaddress = "Error";
                if (record.TypeAAAA.ipAddress != null) ipaddress = record.TypeAAAA.ipAddress.ToString();
                Console.WriteLine($"{record.TypeAAAA.Name.PadRight(32)}\t{record.Ttl.ToString().PadRight(4)}\t{record.Class.ToString().PadRight(4)}\t{record.Type.ToString().PadRight(4)}\t{ipaddress}");
            }
            else Console.WriteLine("Error");
        }
        private static void DisplayNS(ResourceRecord record)
        {
            if (record.TypeNS != null)
            {
                Console.WriteLine($"{record.TypeNS.Name.PadRight(32)}\t{record.Ttl.ToString().PadRight(4)}\t{record.Class.ToString().PadRight(4)}\t{record.Type.ToString().PadRight(4)}\t{record.TypeNS.NameServer}");
            }
            else Console.WriteLine("Error");
        }
        private static void DisplayCName(ResourceRecord record)
        {
            if (record.TypeCName != null)
            {
                Console.WriteLine($"{record.TypeCName.Name.PadRight(32)}\t{record.Ttl.ToString().PadRight(4)}\t{record.Class.ToString().PadRight(4)}\t{record.Type.ToString().PadRight(4)}\t{record.TypeCName.CName}");
            }
            else Console.WriteLine("Error");
        }

        private static void DisplayPTR(ResourceRecord record)
        {
            if (record.TypePTR != null)
            {
                Console.WriteLine($"{record.TypePTR.Name.PadRight(32)}\t{record.Ttl.ToString().PadRight(4)}\t{record.Class.ToString().PadRight(4)}\t{record.Type.ToString().PadRight(4)}\t{record.TypePTR.PTRName}");
            }
            else Console.WriteLine("Error");
        }

        private static void DisplaySOA(ResourceRecord record)
        {
            if (record.TypeSOA != null)
            {
                Console.WriteLine($"{record.TypeSOA.Name}");
                Console.WriteLine($"\t  Primary name server: {record.TypeSOA.Mname}");
                Console.WriteLine($"\tResponsible mail addr: {record.TypeSOA.Rname}");
                Console.WriteLine($"\t        Serial number: {record.TypeSOA.SerialNumber}");
                Console.WriteLine($"\t     Refresh interval: {record.TypeSOA.RefreshInterval}");
                Console.WriteLine($"\t       MaxRetryCount interval: {record.TypeSOA.RetryInterval}");
                Console.WriteLine($"\t         Expire limit: {record.TypeSOA.ExpireLimit}");
                Console.WriteLine($"\t          Minimum TTL: {record.TypeSOA.MinimumTTL}");
            }
            else Console.WriteLine("Error");
        }

        private static void DisplayRawData(Dictionary<IndexXY, ushort> rawData)
        {
            Console.WriteLine(Environment.NewLine + $"Raw Data");
            Console.WriteLine("--------");
            ushort? CurrentY = null;
            string row = string.Empty;
            foreach (var kvp in rawData)
            {
                if (CurrentY == null)
                {
                    //row = "       x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 xA xB xC xD xE xF";
                    //writer.WriteStringValue(row);
                    //writer.Flush();
                    CurrentY = kvp.Key.Y;
                    row = $"{((ushort)CurrentY).ToString("X4")}  ";
                }

                if (kvp.Key.Y == CurrentY)
                {
                    row = $"{row} {kvp.Value.ToString("X2")}";
                }
                else
                {
                    Console.WriteLine($"{row}");
                    CurrentY = kvp.Key.Y;
                    row = $"{((ushort)CurrentY).ToString("X4")}   {kvp.Value.ToString("X2")}";
                }

            }
            Console.WriteLine($"{row}");
        }
    }
}
