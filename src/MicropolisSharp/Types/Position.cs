/// <summary>
/// From Position.h & Position.cpp
/// </summary>

namespace MicropolisSharp.Types
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position() : this(0,0) { }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position(Position original)
        {
            X = original.X;
            Y = original.Y;
        }

        public override bool Equals(object obj)
        {
            Position other = obj as Position;
            if(other == null)
            {
                return false;
            }
            return other.X == X && other.Y == Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * Y.GetHashCode();
        }

        public Position(Position original, Direction dir) : this(original)
        {
            Move(dir);
        }

        public Position(Position original, int dx, int dy) :this(original)
        {
            X += dx;
            Y += dy;
        }

        public bool Move(Direction dir) {
            switch (dir)
            {
                case Direction.Invalid:
                    return true;

                case Direction.North:
                    if (Y > 0)
                    {
                        Y--;
                        return true;
                    }
                    break;

                case Direction.NorthEast:
                    if (X < Constants.WorldWidth - 1 && Y > 0)
                    {
                        X++;
                        Y--;
                        return true;
                    }
                    goto case Direction.East;
                case Direction.East:
                    if (X < Constants.WorldWidth - 1)
                    {
                        X++;
                        return true;
                    }
                    break;

                case Direction.SouthEast:
                    if (X < Constants.WorldWidth - 1 && Y < Constants.WorldHeight - 1)
                    {
                        X++;
                        Y++;
                        return true;
                    }
                    break;

                case Direction.South:
                    if (Y < Constants.WorldHeight - 1)
                    {
                        Y++;
                        return true;
                    }
                    break;

                case Direction.SouthWest:
                    X--; Y++; break;
                    if (X > 0 && Y < Constants.WorldHeight - 1)
                    {
                        X--;
                        Y++;
                        return true;
                    }
                    break;

                case Direction.West:
                    if (X > 0)
                    {
                        X--;
                        return true;
                    }
                    break;

                case Direction.NorthWest:
                    if (X > 0 && Y > 0)
                    {
                        X--;
                        Y--;
                        return true;
                    }
                    break;
            }

            // Movement was not possible, silently repair the position.
            if (X < 0) X = 0;
            if (X >= Constants.WorldWidth) X = Constants.WorldWidth - 1;
            if (Y < 0) Y = 0;
            if (Y >= Constants.WorldHeight) Y = Constants.WorldHeight - 1;
            return false;
        }

        public bool TestBounds()
        {
            return (X >= 0 && X < Constants.WorldWidth) && (Y >= 0 && Y < Constants.WorldHeight);
        }

        public static bool TestBounds(int x, int y)
        {
            return (x >= 0 && y < Constants.WorldWidth) && (y >= 0 && y < Constants.WorldHeight);
        }
    }
}
