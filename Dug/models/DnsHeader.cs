using DNS_Checker.converters;
using DNS_Checker.enums;
using System.Text.Json.Serialization;

namespace DNS_Checker.models
{
    public class DnsHeader
    {
        public ushort Id { get; set; }
        public ushort Flags { get; set; }
        public Char QR { get; set; }
        public ReplyCode replyCode { get; set; }
        public Opcode opcode { get; set; }
        public ResponseFlags responseFlags { get; set; }
        public string errorMessage { get; set; }
        public ushort QuestionCount { get; set; }
        public ushort AnswerCount { get; set; }
        public ushort AuthorityCount { get; set; }
        public ushort AdditionalCount { get; set; }

    }
}
