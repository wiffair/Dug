namespace DNS_Checker.models
{
    public class ResponseFlagsModel
    {
        /// <summary>
        /// AuthoritativeAnswer
        /// </summary>
        public bool AA { get; set; }=false;

        /// <summary>
        /// Truncated
        /// </summary>
        public bool TC { get; set; } = false;

        /// <summary>
        /// RecursionDesired
        /// </summary>
        public bool RD { get; set; } = true;

        /// <summary>
        /// RecursionAvailable
        /// </summary>
        public bool RA { get; set; } = false;
        /// <summary>
        /// Authenticated Data
        /// </summary>
        public bool AD { get; set; } = true;
        /// <summary>
        /// Checking Disabled
        /// </summary>
        public bool CD { get; set; } = false;
    }
}
    //None = 0,
    //AA = 0x0400, // Authoritative answer
    //TC = 0x0200, // Message truncated
    //RD = 0x0100, // Recursion desired
    //RA = 0x0080, // Recursion available
    //AD = 0x0020, // Authenticated Data
    //CD = 0x0010, // Checking Disabled