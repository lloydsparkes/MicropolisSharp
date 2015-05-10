using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public class StringTable
    {
        public long Id { get; set; }
        public int Lines { get; set; }
        public string[] Strings { get; set; }
        public StringTable Next { get; set; }
    }
}
