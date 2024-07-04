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
