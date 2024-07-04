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

using dug.enums;

namespace dug.models
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
