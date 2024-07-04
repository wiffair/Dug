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
    public class TypeFlags
    {
        public bool A { get; set; }
        public bool AAAA { get; set; }
        public bool NS { get; set; }
        public bool MD { get; set; }
        public bool MF { get; set; }
        public bool CNAME { get; set; }
        public bool SOA { get; set; }
        public bool MB { get; set; }
        public bool MG { get; set; }
        public bool MR { get; set; }
        public bool NULL { get; set; }
        public bool WKS { get; set; }
        public bool PTR { get; set; }
        public bool HINFO { get; set; }
        public bool MINFO { get; set; }
        public bool MX { get; set; }
        public bool TXT { get; set; }
        public bool AXFR { get; set; }
        public bool MAILB { get; set; }
        public bool MAILA { get; set; }
        public bool ALL { get; set; }

        public bool IsFlagSet => A || AAAA || NS || MD || MF || CNAME || SOA || MB || MG || MR || NULL || WKS || PTR || HINFO || MINFO || MX || TXT || AXFR || MAILB || MAILA || ALL;
 
    }
}
