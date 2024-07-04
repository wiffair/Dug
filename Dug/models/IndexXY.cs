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
    public class IndexXY
    {
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public IndexXY(int x, int y)
        {
            this.X = (ushort)x;
            this.Y = (ushort)y;
        }
        public IndexXY(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }
    }
}
