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
    public class ResourceRecord
    {
        public RrType RrType { get; set; }
        public string Name { get; set; }
        public DnsRecordType Type { get; set; }
        public DnsClassType Class { get; set; }
        public uint Ttl { get; set; }
        public ushort RdLength { get; set; }
        public string RData { get; set; }
        public TypeA? TypeA { get; set; }
        public TypeAAAA? TypeAAAA { get; set; }
        public TypeNS? TypeNS { get; set; }
        public TypeCName? TypeCName { get; set; }
        public TypePTR? TypePTR { get; set; }
        public TypeSOA? TypeSOA { get; set; }
    }
}
