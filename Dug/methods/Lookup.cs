using dug.dataContainers;
using dug.enums;
using dug.helpers;
using dug.models;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace dug.methods
{
    public class Lookup

    {
        private readonly DataContainer _dataContainer;
        private readonly UdpListener _udpListener;
        private readonly Options _options;
        // Set up Logging
        Logger _logger = new Logger();
        LoggerOptions loggerOptions = new LoggerOptions();
        public Lookup(DataContainer dataContainer, UdpListener udpListener, Options options)
        {
            _dataContainer = dataContainer;
            _udpListener = udpListener;
            _options = options;
            // Set up Logging
            _logger = new Logger("Dug", "Lookup", options.Verbose);
        }
        public async void Go(List<string> dnsServers, string domainName, TypeFlags flags, ResponseFlagsModel responseFlags)
        {
            if (_options.Trace)
            {
                // First find the names of the Root Servers

                // Use an empty string to find root
                DnsTestRecord? rootResponse = await FindRecordNS(dnsServers, "", true);

                // Check if we got a valid response

                if (rootResponse != null)
                {

                    // Now we need to resolve the A records for the root servers

                    List<string>? tempDnsServers = await ResolveAllIpAddresses(rootResponse,  _options.Verbose);

                    // Check if we got at least one valid response

                    if (tempDnsServers != null)
                    {
                        // Now we have the root server IP addresses we can start to query down to the authoritive server

                        bool foundAuthorityServer = false;
                        bool forceExit = false;
                        DnsTestRecord? response = new DnsTestRecord();
                        int loopCount = 0;

                        while (!foundAuthorityServer && !forceExit)
                        {
                            response = await FindRecordA(tempDnsServers, domainName);
                            if (response == null) forceExit = true;
                            else
                            {
                                if (response.ResponseHeader.responseFlags.HasFlag(ResponseFlags.AA)) foundAuthorityServer = true;
                                else
                                {
                                    tempDnsServers.Clear();
                                    tempDnsServers = await ResolveAllIpAddresses(response, _options.Verbose);
                                    if (tempDnsServers == null) forceExit = true;
                                }
                            }
                            loopCount++;
                            if (loopCount > _options.MaxRecords) forceExit = true;
                        }

                        if (_options.Test && foundAuthorityServer && response != null)
                        {
                            //do test
                            List<string> testDnsServers = new List<string>();
                            if (response.ResponseAnswers.Count > 0) testDnsServers.AddRange(await ExtractNSList(response.ResponseAnswers));
                            if (response.ResponseAuthorityRecords.Count > 0) testDnsServers.AddRange(await ExtractNSList(response.ResponseAuthorityRecords));
                            _ = await FindRecordA(testDnsServers, domainName, true, true);
                        }
                    }
                    else
                    {
                        // Unable to resolve root A records
                        _logger.Log(new LogModel("Go.Trace", dug.enums.SeverityLevel.Notice, "Unable to resolve any A record for the root server"),LoggerOptions.Default,true);
                        //await Console.Out.WriteLineAsync("Unable to resolve any A record for the root servers");
                    }
                }
                else
                {
                    _logger.Log(new LogModel("Go.Trace", dug.enums.SeverityLevel.Notice, "Unable to resolve root servers using any of the provided name servers"), LoggerOptions.Default, true);
                    // Unable to resolve root
                    //await Console.Out.WriteLineAsync("Unable to resolve root servers using any of the provided name servers");
                }
            }
            else
            {
                // Resolve the domain name against each DNS server
                foreach (string dnsServer in dnsServers)
                {
                    try
                    {
                        bool typeFound = false;
                        foreach (var property in typeof(TypeFlags).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                        {
                            if (property.PropertyType == typeof(bool) && (bool)property.GetValue(flags) && property.Name != "IsFlagSet")
                            {
                                typeFound = true;
                                var responseType = (DnsRecordType)Enum.Parse(typeof(DnsRecordType), property.Name);
                                DnsTestRecord? response = await DoAsync(dnsServer, domainName, responseType, responseFlags);
                                if (response == null) {
                                    _logger.Log(new LogModel("Go", dug.enums.SeverityLevel.Notice, $"Server {dnsServer} did not respond to request."), LoggerOptions.Default, true);
                                    //await Console.Out.WriteLineAsync($"Server {dnsServer} did not respond to request."); 
                                }
                            }
                        }
                        if (!typeFound)
                        {
                            DnsTestRecord? response = await DoAsync(dnsServer, domainName, DnsRecordType.A, responseFlags);
                            if (response == null) { _logger.Log(new LogModel("Go", dug.enums.SeverityLevel.Notice, $"Server {dnsServer} did not respond to request."), LoggerOptions.Default, true); }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(new LogModel("Go", dug.enums.SeverityLevel.Warning, $"Error resolving domain name against {dnsServer}: {ex.Message}"), LoggerOptions.Default, true);
                        //Console.WriteLine($"Error resolving domain name against {dnsServer}: {ex.Message}");
                    }
                }
            }
            var output = new Output(_dataContainer);
            output.OutResults(_options);
        }




        private async Task<DnsTestRecord?> DoAsync(string serverIP, string domainName, DnsRecordType recordType, ResponseFlagsModel responseFlags, bool visable = true)
        {
            ParseDnsPacket read = new(_dataContainer);
            bool responseArrived = false;
            int index = -1;
            var spinWait = new SpinWait();

            for (int retry = 0; retry < _options.MaxRetryCount; retry++)
            {
                try
                {
                    ushort idNumber = _dataContainer.GetIdCount();
                    byte[] queryBytes = CreateQuery.CreateDnsQuery(domainName, recordType, responseFlags, idNumber);
                    index = await read.ParseDnsQuery(queryBytes, DateTime.UtcNow,serverIP, _options, visable);
                    //Console.WriteLine($"Index: {index} : ID: {idNumber}");
                    await _udpListener.SendDataAsync(queryBytes, serverIP, _options.UdpPort);
                    DateTime startTime = DateTime.UtcNow;
                    while (!responseArrived && (DateTime.UtcNow - startTime).TotalSeconds < _options.Timeout)
                    {
                        responseArrived = _dataContainer.ResponseReceived(idNumber);
                        //Task.Yield();
                        spinWait.SpinOnce(50);
                    }
                    if (responseArrived) break;
                    _dataContainer.Timeout(index);
                    _logger.Log(new LogModel("DoAsync", dug.enums.SeverityLevel.Notice, $"Timeout. Retry {retry + 1} of {_options.MaxRetryCount}"), LoggerOptions.Default, true);
                    //await Console.Out.WriteLineAsync($"Timeout. Retry {retry + 1} of {_options.MaxRetryCount}");
                    spinWait.SpinOnce(_options.SendDelay);
                }
                catch (Exception ex)
                {
                    _logger.Log(new LogModel("DoAsync", dug.enums.SeverityLevel.Alert, $"Error: ({ex})"), LoggerOptions.Default, true);
                    //Console.WriteLine($"Error: ({ex})");
                    return null;
                }
            }
            if (responseArrived)
            {
                return _dataContainer.ReadData(index);
            }
            else
            {
                return null;
            }
        }

        private async Task<DnsTestRecord?> FindRecordNS(List<string> dnsServers, string domainName,  bool visable = true)
        {
            int i = 0;
            string dnsServer = string.Empty;

            DnsTestRecord? responseRecord = new DnsTestRecord();
            bool foundNS = false;
            DnsRecordType recordType = DnsRecordType.NS;
            ResponseFlagsModel rfm = new ResponseFlagsModel();
            rfm.RD = false;
            rfm.AD = true;

            while (i < dnsServers.Count && !foundNS)
            {
                dnsServer = dnsServers[i];
                responseRecord = await DoAsync(dnsServer, domainName, recordType, rfm, visable);
                if (responseRecord == null)
                {
                    _logger.Log(new LogModel("FindRecordNS", dug.enums.SeverityLevel.Notice, $"Unable to resolve root servers using server {dnsServer}"), LoggerOptions.Default, true);
                    //Console.WriteLine($"Unable to resolve root servers using server {dnsServer}");
                    i++;
                }
                else
                {
                    foundNS = true;
                }
            }

            if (foundNS) { return responseRecord; }
            else { return null; }
        }

        /// <summary>
        /// Performs a A record lookup with Authority flag set and no recurs
        /// </summary>
        /// <param name="dnsServers"></param>
        /// <param name="domainName"></param>
        /// <param name="options"></param>
        /// <param name="visable"></param>
        /// <returns></returns>
        private async Task<DnsTestRecord?> FindRecordA(List<string> dnsServers, string domainName, bool visable = true, bool forceAll = false)
        {
            int i = 0;
            string dnsServer = string.Empty;

            DnsTestRecord? responseRecord = new DnsTestRecord();
            bool foundA = false;
            DnsRecordType recordType = DnsRecordType.A;
            ResponseFlagsModel rfm = new ResponseFlagsModel();
            rfm.RD = false;
            rfm.AD = true;
            int index = -1;
            while (i < dnsServers.Count && !foundA && index < _options.MaxRecords)
            {
                dnsServer = dnsServers[i];
                responseRecord = await DoAsync(dnsServer, domainName, recordType, rfm, visable);
                if (responseRecord == null)
                {
                    _logger.Log(new LogModel("FindRecordA", dug.enums.SeverityLevel.Notice, $"Unable to resolve {domainName} using server {dnsServer}"), LoggerOptions.Default, true);
                    //Console.WriteLine($"Unable to resolve {domainName} using server {dnsServer}");
                    i++;
                }
                else
                {
                    if (!forceAll) { foundA = true; }
                    if (forceAll) { i++; }
                    index = responseRecord.QueryHeader.Id;
                }
            }

            if (foundA) { return responseRecord; }
            else { return null; }
        }

        /// <summary>
        /// Provides IP address for the Name Servers passed in the DNS Response.
        /// Either by collecting them from the Addition Records section if avalible, or by resolving via lookups
        /// </summary>
        /// <param name="resourceRecords"></param>
        /// <returns>A List of strings containing resolved IP addresses</returns>
        private async Task<List<string>?> ResolveAllIpAddresses(DnsTestRecord record,  bool visable = true)
        {

            bool foundA = false;
            List<string> dnsServers = new List<string>();

            // Check if we have any Addition Records, and if so, do they contain the IP addresses we need. If so, we dont need to do queries
            if (record.ResponseAdditionalRecords.Count > 0)
            {
                List<string> aList = await ExtractAList(record.ResponseAdditionalRecords);
                if (aList.Count > 0)
                {
                    foundA = true;
                    dnsServers = aList;
                }
            }
            if (!foundA)
            {
                // We have not been able to find any A records, so will have to query them

                ResponseFlagsModel rfm = new ResponseFlagsModel();
                rfm.RD = true;
                DnsRecordType recordType = DnsRecordType.A;

                List<DnsTestRecord> responsesA = new List<DnsTestRecord>();

                // Create a combined list of responses
                List<ResourceRecord> resourceRecords = new List<ResourceRecord>();
                if (record.ResponseAuthorityRecords.Count > 0) resourceRecords.AddRange(record.ResponseAuthorityRecords);
                if (record.ResponseAnswers.Count > 0) resourceRecords.AddRange(record.ResponseAnswers);

                foreach (ResourceRecord resourceRecord in resourceRecords)
                {
                    if (resourceRecord.TypeNS != null)
                    {
                        string? rootNameServer = resourceRecord?.TypeNS?.NameServer;
                        if (rootNameServer != null)
                        {
                            DnsTestRecord? ResolveA = await DoAsync(record.QueryServer, rootNameServer, recordType, rfm,   visable);
                            if (ResolveA != null)
                            {
                                if (ResolveA.ResponseAuthorityRecords.Count > 0)
                                {
                                    List<string> aList = await ExtractAList(ResolveA.ResponseAuthorityRecords);
                                    if (aList.Count > 0)
                                    {
                                        foundA = true;
                                        dnsServers = aList;
                                    }
                                }
                                if (ResolveA.ResponseAnswers.Count > 0)
                                {
                                    List<string> aList = await ExtractAList(ResolveA.ResponseAnswers);
                                    if (aList.Count > 0)
                                    {
                                        foundA = true;
                                        dnsServers.AddRange(aList);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            if (foundA) { return dnsServers; }
            else { return null; }
        }

        /// <summary>
        /// Extracts the IP addresses from the passed DNS resource Record where type = A
        /// </summary>
        /// <param name="resourceRecords"></param>
        /// <returns></returns>
        private async Task<List<string>> ExtractAList(List<ResourceRecord> resourceRecords)
        {
            List<string> aList = new List<string>();
            foreach (ResourceRecord resourceRecord in resourceRecords)
            {
                if (resourceRecord.Type == DnsRecordType.A)
                {
                    string? serverIP = resourceRecord?.TypeA?.ipAddress?.ToString();
                    if (serverIP != null)
                    {
                        aList.Add(serverIP);
                    }
                }
            }
            return aList;
        }

        private async Task<List<string>> ExtractNSList(List<ResourceRecord> resourceRecords)
        {
            List<string> nsList = new List<string>();
            foreach (ResourceRecord resourceRecord in resourceRecords)
            {
                if (resourceRecord.Type == DnsRecordType.NS)
                {
                    string? serverIP = resourceRecord?.TypeA?.ipAddress?.ToString();
                    if (serverIP != null)
                    {
                        nsList.Add(serverIP);
                    }
                }
            }
            return nsList;
        }
    }
}
