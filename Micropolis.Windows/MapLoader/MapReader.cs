using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micropolis.Windows.MapLoader
{
    public class MapReader
    {
        public const int HISTORY_SIZE = 240;

        public short[,] ReadFile(BinaryReader stream)
        {
            var history = new short[HISTORY_SIZE];
            var map = new short[120, 100];

            readHistory(history, stream);
            readHistory(history, stream);
            readHistory(history, stream);
            readHistory(history, stream);
            readHistory(history, stream);
            readHistory(history, stream);
            readHistory(history, stream, HISTORY_SIZE / 2);

            readMap(map, stream);

            return map;
        }

        private void readMap(short[,] legacyMap, BinaryReader stream)
        {
            for (int x = 0; x < legacyMap.GetLength(0); x++)
            {
                for (int y = 0; y < legacyMap.GetLength(1); y++)
                {
                    legacyMap[x, y] = getShort(stream.ReadBytes(2));
                }
            }
        }

        private void readHistory(short[] historyArray, BinaryReader stream, int size = HISTORY_SIZE)
        {
            for (int i = 0; i < size; i++)
            {
                historyArray[i] = getShort(stream.ReadBytes(2));
            }
        }

        private short getShort(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
                BitConverter.ToInt16(bytes, 0);
            }
            return BitConverter.ToInt16(bytes, 0); ;
        }

        private int getLong(short[] shorts, int indexOfFirst)
        {
            byte[] bytes = new byte[4];
            bytes[0] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
            bytes[1] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
            indexOfFirst++;
            bytes[2] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
            bytes[3] = BitConverter.GetBytes(shorts[indexOfFirst])[1];

            if (BitConverter.IsLittleEndian)
            {
                bytes[2] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
                bytes[3] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
                indexOfFirst++;
                bytes[0] = BitConverter.GetBytes(shorts[indexOfFirst])[0];
                bytes[1] = BitConverter.GetBytes(shorts[indexOfFirst])[1];
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
