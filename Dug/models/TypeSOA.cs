using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_Checker.models
{
    public class TypeSOA
    {
        public string Name { get; set; }
        /// <summary>
        /// Primary name server
        /// </summary>
        public string Mname { get; set; }
        /// <summary>
        /// Responsible authority's mailbox
        /// </summary>
        public string Rname { get; set; }

        public UInt32 SerialNumber { get; set; }
        public UInt32 RefreshInterval { get; set; }
        public UInt32 RetryInterval { get; set; }
        public UInt32 ExpireLimit { get; set; }
        public UInt32 MinimumTTL { get; set; }
    }
}
