using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Checker.models
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
