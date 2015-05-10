/// <summary>
/// From Position.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum Direction
    {
        Invalid,    ///< Invalid direction.
        North,      ///< Direction pointing north.
        NorthEast, ///< Direction pointing north-east.
        East,       ///< Direction pointing east.
        SouthEast, ///< Direction pointing south-east.
        South,      ///< Direction pointing south.
        SouthWest, ///< Direction pointing south-west.
        West,       ///< Direction pointing west.
        NorthWest, ///< Direction pointing north-west.

        Begin = North,        ///< First valid direction.
        End = NorthWest + 1, ///< End-condition for directions
    }

    public static class DirectionUtils
    {
        public static Direction Increment45(this Direction dir, int count = 1)
        {
            return (Direction)dir + count;
        }

        public static Direction Increment90(this Direction dir)
        {
            return Increment45(dir, 2);
        }

        public static Direction Rotate45(this Direction dir, int count = 1)
        {
            return (Direction)(((dir - Direction.North + count) & 7) + Direction.North);
        }

        public static Direction Rotate90(this Direction dir)
        {
            return Rotate45(dir, 2);
        }

        public static Direction Rotate180(this Direction dir)
        {
            return Rotate45(dir, 4);
        }
    }
}
