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
using dug.helpers;
using dug.models;
using System.Reflection;

namespace dug.methods
{
    public class Lookup

    {
        private readonly DataContainer _dataContainer;
        private readonly UdpListener _udpListener;

        public Lookup(DataContainer dataContainer, UdpListener udpListener)
        {
            _dataContainer = dataContainer;
            _udpListener = udpListener;
        }
        public async void Go(List<string> dnsServers, string domainName, TypeFlags flags, ResponseFlagsModel responseFlags, Options options)
        {
            if (options.Trace)
            {
                // First find the names of the Root Servers

                // Use an empty string to find root
                DnsTestRecord? rootResponse = await FindRecordNS(dnsServers, "", options);

                // Check if we got a valid response

                if (rootResponse != null)
                {

                    // Now we need to resolve the A records for the root servers

                    List<string>? tempDnsServers = await ResolveAllIpAddresses(rootResponse, options, options.Verbose);

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
                            response = await FindRecordA(tempDnsServers, domainName, options);
                            if (response == null) forceExit = true;
                            else
                            {
                                if (response.ResponseHeader.responseFlags.HasFlag(ResponseFlags.AA)) foundAuthorityServer = true;
                                else
                                {
                                    tempDnsServers.Clear();
                                    tempDnsServers = await ResolveAllIpAddresses(response, options, options.Verbose);
                                    if (tempDnsServers == null) forceExit = true;
                                }
                            }
                            loopCount++;
                            if (loopCount > options.MaxRecords) forceExit = true;
                        }

                        if (options.Test && foundAuthorityServer && response != null)
                        {
                            //do test
                            List<string> testDnsServers = new List<string>();
                            if (response.ResponseAnswers.Count > 0) testDnsServers.AddRange(await ExtractNSList(response.ResponseAnswers));
                            if (response.ResponseAuthorityRecords.Count > 0) testDnsServers.AddRange(await ExtractNSList(response.ResponseAuthorityRecords));
                            _ = await FindRecordA(testDnsServers, domainName, options, true, true);
                        }
                    }
                    else
                    {
                        // Unable to resolve root A records
                        await Console.Out.WriteLineAsync("Unable to resolve any A record for the root servers");
                    }
                }
                else
                {
                    // Unable to resolve root
                    await Console.Out.WriteLineAsync("Unable to resolve root servers using any of the provided name servers");
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
                                DnsTestRecord? response = await DoAsync(dnsServer, domainName, responseType, responseFlags, options);
                                if (response == null) { await Console.Out.WriteLineAsync($"Server {dnsServer} did not respond to request."); }
                            }
                        }
                        if (!typeFound)
                        {
                            DnsTestRecord? response = await DoAsync(dnsServer, domainName, DnsRecordType.A, responseFlags, options);
                            if (response == null) { await Console.Out.WriteLineAsync($"Server {dnsServer} did not respond to request."); }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error resolving domain name against {dnsServer}: {ex.Message}");
                    }
                }
            }
            var output = new Output(_dataContainer);
            output.OutResults(options);
        }




        private async Task<DnsTestRecord?> DoAsync(string serverIP, string domainName, DnsRecordType recordType, ResponseFlagsModel responseFlags, Options options, bool visable = true)
        {
            ParseDnsPacket read = new(_dataContainer);
            bool responseArrived = false;
            int index = -1;
            var spinWait = new SpinWait();

            for (int retry = 0; retry < options.MaxRetryCount; retry++)
            {
                try
                {
                    ushort idNumber = _dataContainer.GetIdCount();
                    byte[] queryBytes = CreateQuery.CreateDnsQuery(domainName, recordType, responseFlags, idNumber);
                    index = await read.ParseDnsQuery(queryBytes, DateTime.UtcNow,serverIP, options, visable);
                    //Console.WriteLine($"Index: {index} : ID: {idNumber}");
                    await _udpListener.SendDataAsync(queryBytes, serverIP, options.UdpPort);
                    DateTime startTime = DateTime.UtcNow;
                    while (!responseArrived && (DateTime.UtcNow - startTime).TotalSeconds < options.Timeout)
                    {
                        responseArrived = _dataContainer.ResponseReceived(idNumber);
                        //Task.Yield();
                        spinWait.SpinOnce(50);
                    }
                    if (responseArrived) break;
                    _dataContainer.Timeout(index);
                    await Console.Out.WriteLineAsync($"Timeout. Retry {retry + 1} of {options.MaxRetryCount}");
                    spinWait.SpinOnce(options.SendDelay);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: ({ex})");
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

        private async Task<DnsTestRecord?> FindRecordNS(List<string> dnsServers, string domainName, Options options, bool visable = true)
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
                responseRecord = await DoAsync(dnsServer, domainName, recordType, rfm, options, visable);
                if (responseRecord == null)
                {
                    Console.WriteLine($"Unable to resolve root servers using server {dnsServer}");
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
        private async Task<DnsTestRecord?> FindRecordA(List<string> dnsServers, string domainName, Options options, bool visable = true, bool forceAll = false)
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
            while (i < dnsServers.Count && !foundA && index < options.MaxRecords)
            {
                dnsServer = dnsServers[i];
                responseRecord = await DoAsync(dnsServer, domainName, recordType, rfm, options, visable);
                if (responseRecord == null)
                {
                    Console.WriteLine($"Unable to resolve {domainName} using server {dnsServer}");
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
        private async Task<List<string>?> ResolveAllIpAddresses(DnsTestRecord record, Options options, bool visable = true)
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
                            DnsTestRecord? ResolveA = await DoAsync(record.QueryServer, rootNameServer, recordType, rfm, options, visable);
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
