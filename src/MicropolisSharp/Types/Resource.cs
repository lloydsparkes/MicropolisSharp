using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public class Resource
    {
        public byte[] Data { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public long Id { get; set; }
        public Resource Next { get; set; }
    }
}
