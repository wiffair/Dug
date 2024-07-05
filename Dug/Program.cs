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
using dug.methods;
using dug.models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

// Configure the DNS servers
List<string> dnsServers = new();
string[] Args = Environment.GetCommandLineArgs();
Options options = new Options();
options.OutConsole = true;
bool argsSet = false;

//Error Checking
bool dnsServerSet = false;
string dnsServerSetError = " Must set dnsServer address.";
bool lookupSet = false;
string lookupSetError = " Must provide something to resolve.";
string qString = string.Empty;

TypeFlags typeFlags = new();
ResponseFlagsModel responseFlags = new();

// Get the current assembly
Assembly currentAssembly = Assembly.GetExecutingAssembly();

// Get the assembly version
string assemblyVersion = currentAssembly?.GetName()?.Version?.ToString() ?? "999.999.999.999";


if (Args.Length > 1)
{
    argsSet = true;
    string[] argument = Args;
    for (int i = 1; i < Args.Length; i++)
    {
        if (argument[i].StartsWith('-'))
        {
            switch (argument[i].Substring(1, 1))
            {
                // Set Query Type flags
                case "t":
                    i++;
                    typeFlags = SetFlags(typeFlags, argument[i].ToUpper());
                    break;
                // Set output format
                case "o":
                    i++;
                    string outType = argument[i].ToUpper();
                    if (outType == "CSV") options.OutCsv = true;
                    if (outType == "JSON") options.OutJson = true;
                    if (outType == "NOC") options.OutConsole = false;
                    break;
                case "F":
                    i++;
                    options.OutFileName = argument[i];
                    break;
                case "H":
                    options.OutHeadersOnly = true;
                    break;
                case "q":
                    options.ShowQuery = true;
                    break;
                case "V":
                    options.Verbose = true;
                    break;
                case "v":
                    Console.WriteLine($"Dug v{assemblyVersion}");
                    Environment.Exit(0);
                    break;
                case "h":
                    DisplayTextFiles displayTextFile = new DisplayTextFiles();
                    displayTextFile.PrintHelpText();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine($"Invalid argumnet: {argument}");
                    Environment.Exit(5);
                    break;
            }
        }
        else if (argument[i].StartsWith('@'))
        {
            // Set dns Server address(es)
            dnsServers.Add(argument[i].Substring(1));
            dnsServerSet = true;
        }
        else if (argument[i].StartsWith('+'))
        {
            switch (argument[i].Substring(1).ToUpper())
            {
                //flags
                // Set recursive Lookup
                case "RDFLAG":
                    responseFlags.RD = true;
                    break;
                case "NORDFLAG":
                    responseFlags.RD = false;
                    break;
                case "RECURSE":
                    responseFlags.RD = true;
                    break;
                case "NORECURSE":
                    responseFlags.RD = false;
                    break;
                case "ADFLAG":
                    responseFlags.AD = true;
                    break;
                case "NOADFLAG":
                    responseFlags.AD = false;
                    break;
                case "CDFLAG":
                    responseFlags.CD = true;
                    break;
                case "NOCDFLAG":
                    responseFlags.CD = false;
                    break;
                case "RAW":
                    options.Raw = true;
                    break;
                case "TRACE":
                    options.Trace = true;
                    break;
                case "TEST":
                    options.Test = true;
                    break;
                case "TIMEOUT":
                    i++;
                    if (int.TryParse(argument[i], out int to)) options.Timeout = to;
                    break;
                case "RETRY":
                    i++;
                    if (int.TryParse(argument[i], out int rc)) options.MaxRetryCount = rc;
                    break;
                default:
                    Console.WriteLine($"Invalid argumnet: {argument}");
                    Environment.Exit(6);
                    break;
            }
        }
        else if (argument[i].ToUpper() == "SHOW")
        {
            i++;
            DisplayTextFiles displayTextFile = new DisplayTextFiles();
            switch (argument[i].ToUpper())
            {
                case "LICENSE":
                    displayTextFile.PrintLicenseText();
                    Environment.Exit(0);
                    break;
                case "HELP":
                    displayTextFile.PrintHelpText();
                    Environment.Exit(0);
                    break;
                default:
                    displayTextFile.PrintHelpText();
                    Environment.Exit(0);
                    break;
            }
        }
        else
        {
            // If no switch, it should be the query name...
            if (argument[i] == ".") qString = ""; else qString = argument[i];

            lookupSet = true;
        }
    }
}


// Set up Logging
Logger _logger = new Logger("Dug","Program", options.Verbose);
LoggerOptions loggerOptions = new LoggerOptions();

// Create Instance
var dataContainer = new DataContainer();

// Obtain a listening port
int? udpPort = NetworkHelpers.GetFreeUdpPort();
if (!udpPort.HasValue)
{
    _logger.Log(new LogModel("Obtain UDP Port", dug.enums.SeverityLevel.Alert, "Unable to open UDP lisener"), LoggerOptions.ForceConsole | LoggerOptions.ForceLog);
    Environment.Exit(-999);
}
else
{
    _logger.Log(new LogModel("Obtain UDP Port", SeverityLevel.Info, $"Listening on UDP Port: {udpPort}"),LoggerOptions.Default,true);
}
UdpListener udpListener = new UdpListener((int)udpPort, dataContainer);
var LookUp = new Lookup(dataContainer, udpListener, options);
Task listeningTask = udpListener.StartListeningAsync();

// Process if we have passed in the values at run time
if (argsSet)
{
    if (Process(lookupSet, dnsServerSet, dnsServers, qString, typeFlags, responseFlags, options) < 0)
    {
        _logger.Log(new LogModel("ArgInput", dug.enums.SeverityLevel.Alert, "Error processing request"), LoggerOptions.ForceConsole | LoggerOptions.ForceLog);
        Environment.Exit(-1);
    }
    else Environment.Exit(0);
}

// If we are here, then no arguments were passed, so we will ask questions

do
{
    Console.Write(Environment.NewLine + "Type of query?(A,NS, SOA, MX) [Enter to finish]: ");
    string qType = Console.ReadLine();
    if (string.IsNullOrEmpty(qType))
    {
        if (!typeFlags.IsFlagSet) typeFlags.A = true;
        break;
    }
    else
    {
        typeFlags = SetFlags(typeFlags, qType);
    }
} while (true);

do
{
    Console.Write(Environment.NewLine + "DNS Server [Enter to finish]: ");
    string dnsServer = Console.ReadLine();
    if (string.IsNullOrEmpty(dnsServer))
    {
        if (dnsServers.Count == 0)
        {
            Console.Write("Please enter at least one DNS server.");
        }
        else
        {
            break;
        }
    }
    else
    {
        dnsServers.Add(dnsServer);
        dnsServerSet = true;
    }
} while (true);
Console.Write("Recusive Lookup? (y/n)");
do
{
    var rd = Console.ReadKey();
    if (rd.KeyChar == 'y' || rd.KeyChar == 'Y')
    {
        responseFlags.RD = true;
        break;
    }
    if (rd.KeyChar == 'n' || rd.KeyChar == 'N')
    {
        responseFlags.RD = false;
        break;
    }
    (int x, int y) = Console.GetCursorPosition();
    Console.SetCursorPosition(x - 1, y);
}
while (true);
Console.Write("Headers Only Lookup? (y/n)");
do
{
    var rd = Console.ReadKey();
    if (rd.KeyChar == 'y' || rd.KeyChar == 'Y')
    {
        options.OutHeadersOnly = true;
        break;
    }
    if (rd.KeyChar == 'n' || rd.KeyChar == 'N')
    {
        options.OutHeadersOnly = false;
        break;
    }
    (int x, int y) = Console.GetCursorPosition();
    Console.SetCursorPosition(x - 1, y);
}
while (true);

do
{
    Console.Write(Environment.NewLine + "Resolve (ctl+c to quit): ");
    do
    {
        qString = Console.ReadLine();
        if (!string.IsNullOrEmpty(qString)) lookupSet = true;
    } while (!lookupSet);


    // Process the request

    if (Process(lookupSet, dnsServerSet, dnsServers, qString, typeFlags, responseFlags, options) < 0)
    {
        _logger.Log(new LogModel("ConsoleInput", dug.enums.SeverityLevel.Alert, "Error processing request"), LoggerOptions.ForceConsole | LoggerOptions.ForceLog);
        Environment.Exit(-1);
    }

    Thread.Sleep(1000);
    Console.WriteLine(".");
} while (!argsSet);

int Process(bool lookupSet, bool dnsServerSet, List<string> dnsServers, string queryName, TypeFlags tFlags, ResponseFlagsModel responseFlags, Options options)
{
    if (!lookupSet)
    {
        _logger.Log(new LogModel("Process", dug.enums.SeverityLevel.Alert, $"Error: {lookupSetError}"), LoggerOptions.ForceConsole | LoggerOptions.ForceLog);
        Environment.Exit(10);
    }
    // If a name has been passed- try to resolve it
    if (!dnsServerSet)
    {
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface ni in networkInterfaces)
        {
            if (ni.OperationalStatus == OperationalStatus.Up && ni.GetIPProperties().DnsAddresses.Count > 0)
            {
                foreach (IPAddress dnsAddress in ni.GetIPProperties().DnsAddresses)
                {
                    if (dnsAddress.AddressFamily == AddressFamily.InterNetwork) // IPv4
                    {
                        dnsServers.Add(dnsAddress.ToString()); // Add to list
                    }
                }
            }
        }
        if (dnsServers.Count > 0) dnsServerSet = true;
    }


    if (dnsServerSet)
    {
        // If PTR check if ip address has been passed, and if so convert it to a in-addr.arpa format
        if (tFlags.PTR) queryName = PTRName(queryName);

        // Process the request
        try
        {
            LookUp.Go(dnsServers, queryName, tFlags, responseFlags);
        }
        finally
        {
            if (udpListener != null)
            {
                udpListener.StopListening();
            }
        }
        return 0;
    }
    else
    {
        _logger.Log(new LogModel("Process", dug.enums.SeverityLevel.Alert, $"Error:{dnsServerSetError}"), LoggerOptions.ForceConsole | LoggerOptions.ForceLog);
        Environment.Exit(11);
    }
    return -1;
}


string PTRName(string Name)
{
    if (IPAddress.TryParse(Name, out IPAddress ipAddress))
    {
        // Get the bytes of the IP address
        byte[] bytes = ipAddress.GetAddressBytes();

        // Reverse the order of the bytes
        Array.Reverse(bytes);

        // Create a new string with the reversed bytes, separated by dots
        string reversedIpAddress = string.Join(".", bytes.Select(b => b.ToString()));
        return ($"{reversedIpAddress}.in-addr.arpa");
    }
    return Name;
}

TypeFlags SetFlags(TypeFlags typeFlags, string argument)
{

    switch (argument)
    {
        case "A":
            typeFlags.A = true;
            break;
        case "AAAA":
            typeFlags.AAAA = true;
            break;
        case "NS":
            typeFlags.NS = true;
            break;
        case "SOA":
            typeFlags.SOA = true;
            break;
        case "MX":
            typeFlags.MX = true;
            break;
        case "PTR":
            typeFlags.PTR = true;
            break;
        default:
            break;
    }
    return typeFlags;
}













