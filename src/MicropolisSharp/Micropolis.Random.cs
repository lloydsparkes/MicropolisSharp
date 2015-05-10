using System;

/// <summary>
/// From Random.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        protected long nextRandom;

        public int SimRandom()
        {
            nextRandom = nextRandom * 1103515245 + 12345;
            return (int)(nextRandom & 0xffff00) >> 8;
        }

        public short GetRandom(short range)
        {
            int maxMultiple, rnum;

            range++; /// @bug Increment may cause range overflow.
            maxMultiple = 0xffff / range;
            maxMultiple *= range;

            do
            {
                rnum = GetRandom16();
            } while (rnum >= maxMultiple);

            return (short)(rnum % range);
        }

        public int GetRandom16()
        {
            return SimRandom() & 0x0000ffff;
        }

        public int GetRandom16Signed()
        {
            int i = GetRandom16();

            if (i > 0x7fff)
            {
                i = 0x7fff - i;
            }

            return i;
        }

        public short GetERandom(short limit)
        {
            short z = GetRandom(limit);
            short x = GetRandom(limit);

            return Math.Min(z, x);
        }

        public void SeedRandom(int seed)
        {
            nextRandom = seed;
        }

        public void RandomlySeedRandom()
        {
            nextRandom = (new Random().Next());
        }
    }
}
