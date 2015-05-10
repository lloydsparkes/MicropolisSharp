/// <summary>
/// From may_type.h
/// </summary>
namespace MicropolisSharp.Types
{
    public class Map<DataType>
        where DataType : struct
    {
        public int BlockSize { get; private set; }

        private DataType defaultValue;

        public readonly int width;
        public readonly int height;

        private DataType[] data;        

        public Map(int blockSize)
        {
            this.BlockSize = blockSize;

            this.width = (Constants.WorldWidth + BlockSize - 1) / BlockSize;
            this.height = (Constants.WorldHeight + BlockSize - 1) / BlockSize;
            this.defaultValue = default(DataType);
            this.data = new DataType[this.width * this.height];
        }

        public Map(DataType defaultValue, int blockSize) : this(blockSize)
        {
            this.defaultValue = defaultValue;
            Fill(defaultValue);
        }

        public Map(Map<DataType> map) : this(map.BlockSize)
        {
            for (int i = 0; i < this.width * this.height; ++i)
            {
                data[i] = map.data[i];
            }
        }

        public bool Compare(Map<DataType> other)
        {
            if(other == null)
            {
                return false;
            }

            if(other.BlockSize != this.BlockSize)
            {
                return false;
            }

            for (int i = 0; i < this.width * this.height; ++i)
            {
                if(!data[i].Equals(other.data[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public void Fill(DataType value)
        {
            for (int i = 0; i < this.width * this.height; ++i)
            {
                data[i] = value;
            }
        }

        public void Clear()
        {
            Fill(defaultValue);
        }

        public void Set(int x, int y, DataType value)
        {
            if (OnMap(x, y))
                data[x * height + y] = value;
        }

        public DataType Get(int x, int y)
        {
            if (OnMap(x, y))
                return data[x * height + y];

            return defaultValue;
        }

        public bool OnMap(int x, int y)
        {
            return (x >= 0 && x < width) && (y >= 0 && y < height);
        }

        public void WorldSet(int x, int y, DataType value)
        {
            if (WorldOnMap(x, y))
            {
                x /= BlockSize;
                y /= BlockSize;
                data[x * height + y] = value;
            }
        }

        public DataType WorldGet(int x, int y)
        {
            if (!WorldOnMap(x, y))
                return defaultValue;

            x /= BlockSize;
            y /= BlockSize;
            return data[x * height + y];
        }

        public bool WorldOnMap(int x, int y)
        {
            return (x >= 0 && x < Constants.WorldWidth) && (y >= 0 && y < Constants.WorldHeight);
        }

        public DataType[] getBase()
        {
            return data;
        }
    }

    public class ByteMap1 : Map<byte>
    {
        public ByteMap1() : base(1) { }
        public ByteMap1(byte defaultValue) : base(defaultValue, 1) { }
        public ByteMap1(ByteMap1 map) : base(map) { }
    }

    public class ByteMap2 : Map<byte>
    {
        public ByteMap2() : base(2) { }
        public ByteMap2(byte defaultValue) : base(defaultValue, 2) { }
        public ByteMap2(ByteMap2 map) : base(map) { }
    }

    public class ByteMap4 : Map<byte>
    {
        public ByteMap4() : base(4) { }
        public ByteMap4(byte defaultValue) : base(defaultValue, 4) { }
        public ByteMap4(ByteMap4 map) : base(map) { }
    }

    public class ShortMap8 : Map<short>
    {
        public ShortMap8() : base(8) { }
        public ShortMap8(short defaultValue) : base(defaultValue, 8) { }
        public ShortMap8(ShortMap8 map) : base(map) { }
    }
}
