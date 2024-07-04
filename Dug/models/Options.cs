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

namespace dug.models
{
    public class Options
    {
        public bool OutCsv { get; set; }
        public bool OutJson { get; set; }
        public bool OutConsole { get; set; }
        public bool OutHeadersOnly { get; set; }
        public string? OutFileName { get; set; }=null;
        public bool Raw { get; set; }
        public bool ShowQuery { get; set; }
        public bool Trace { get; set; }
        public bool Test { get; set; }
        public bool Verbose { get; set; }
        public int Timeout { get; set; } = 5;
        /// <summary>
        /// Delay between sending packets in ms
        /// </summary>
        public int SendDelay { get; set; } = 500;
        public int MaxRetryCount { get; set; } = 4;
        public int RetryDelay { get; set; } = 1;
        public int MaxRecords { get; set; } = 300;
        public int UdpPort { get; set; } = 53;
        public int TcpPort { get; set; } = 53;
    }
}
