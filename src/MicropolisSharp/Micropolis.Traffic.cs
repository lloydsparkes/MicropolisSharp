using MicropolisSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// From Traffic.cpp
/// </summary>

namespace MicropolisSharp
{
    public partial class Micropolis
    {
        private short curMapStackPointer;
        private Position[] curMapStackXY;
        private int trafMaxX;
        private int trafMaxY;

        public short MakeTrafficAt(int x, int y, ZoneType dest)
        {
            Position pos = new Position(x, y);

            if (TryDrive(pos, dest))
            { /* attempt to drive somewhere */
                addToTrafficDensityMap(); /* if sucessful, inc trafdensity */
                return 1;             /* traffic passed */
            }

            return 0;                 /* traffic failed */
        }

        public short MakeTraffic(int x, int y, ZoneType dest)
        {
            Position startPos = new Position(x,y);
            return MakeTraffic(startPos, dest);
        }

        public short MakeTraffic(Position startPos, ZoneType dest) {
            curMapStackPointer = 0; // Clear position stack

            Position pos = new Position(startPos);

            //TODO: if 0 section

            if (FindPerimeterRoad(pos))
            { /* look for road on zone perimeter */

                if (TryDrive(pos, dest))
                { /* attempt to drive somewhere */
                    addToTrafficDensityMap(); /* if sucessful, inc trafdensity */
                    return 1;             /* traffic passed */
                }

                return 0;                 /* traffic failed */
            }
            else
            {
                return -1;                /* no road found */
            }
        }

        public void addToTrafficDensityMap()
        { /* For each saved position of the drive */
            while (curMapStackPointer > 0)
            {

                Position pos = PullPos();
                if (pos.TestBounds())
                {

                    ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

                    if (tile >= (ushort)MapTileCharacters.ROADBASE && tile < (ushort)MapTileCharacters.POWERBASE)
                    {
                        SimSprite sprite = new SimSprite();

                        // Update traffic density.
                        int traffic = TrafficDensityMap.WorldGet(pos.X, pos.Y);
                        traffic += 50;
                        traffic = Math.Min(traffic, 240);
                        TrafficDensityMap.WorldSet(pos.X, pos.Y, (Byte)traffic);

                        // Check for heavy traffic.
                        if (traffic >= 240 && GetRandom(5) == 0)
                        {

                            trafMaxX = pos.X;
                            trafMaxY = pos.Y;

                            /* Direct helicopter towards heavy traffic */
                            sprite = GetSprite(SpriteType.Helicopter);
                            if (sprite != null && sprite.Control == -1)
                            {

                                sprite.DestX = trafMaxX * 16;
                                sprite.DestY = trafMaxY * 16;

                            }
                        }
                    }
                }
            }
        }

        public void PushPos(Position pos)
        {
            //TODO: Fix & Enable Assert
            //assert(curMapStackPointer < MAX_TRAFFIC_DISTANCE + 1);
            curMapStackPointer++;
            if (curMapStackPointer > Constants.MaxTrafficDistance)
                return;
            curMapStackXY[curMapStackPointer] = pos;
        }

        public Position PullPos()
        {
            //TODO: Fix & Enable Assert
            //assert(curMapStackPointer > 0);
            if (curMapStackPointer > 0)
            {
                curMapStackPointer--;
                return curMapStackXY[curMapStackPointer + 1];
            }
            return null;
        }

        public bool FindPerimeterRoad(Position pos)
        { /* look for road on edges of zone */
            short[] PerimX = { -1, 0, 1, 2, 2, 2, 1, 0, -1, -2, -2, -2 };
            short[] PerimY = { -2, -2, -2, -1, 0, 1, 2, 2, 2, 1, 0, -1 };
            int tx, ty;

            for (short z = 0; z < 12; z++)
            {

                tx = pos.X + PerimX[z];
                ty = pos.Y + PerimY[z];

                if (Position.TestBounds(tx, ty))
                {

                    if (RoadTest(Map[tx,ty]))
                    {
                        pos.X = tx;
                        pos.Y = ty;

                        return true;
                    }
                }
            }

            return false;
        }

        public bool FindPerimeterTelecom(Position pos)
        {     /* look for telecom on edges of zone */
            short[] PerimX = { -1, 0, 1, 2, 2, 2, 1, 0, -1, -2, -2, -2 };
            short[] PerimY = { -2, -2, -2, -1, 0, 1, 2, 2, 2, 1, 0, -1 };
            int tx, ty;
            ushort tile;

            for (short z = 0; z < 12; z++)
            {

                tx = pos.X + PerimX[z];
                ty = pos.Y + PerimY[z];

                if (Position.TestBounds(tx, ty))
                {

                    tile = (ushort)(Map[tx,ty] & (ushort)MapTileBits.LowMask);
                    if (tile >= (ushort)MapTileCharacters.TELEBASE && tile <= (ushort)MapTileCharacters.TELELAST)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryDrive(Position startPos, ZoneType dest) {
            Direction dirLast = Direction.Invalid;
            Position drivePos = new Position(startPos);

            /* Maximum distance to try */
            for (short dist = 0; dist < Constants.MaxTrafficDistance; dist++)
            {

                Direction dir = TryGo(drivePos, dirLast);
                if (dir != Direction.Invalid)
                { // we found a road
                    drivePos.Move(dir);
                    dirLast = dir.Rotate180();

                    /* Save pos every other move.
                     * This also relates to
                     * Micropolis::trafficDensityMap::MAP_BLOCKSIZE
                     */
                    if ((dist & 1).IsTrue())
                    {
                        PushPos(drivePos);
                    }

                    if (DriveDone(drivePos, dest))
                    { // if destination is reached
                        return true; /* pass */
                    }

                }
                else
                {

                    if (curMapStackPointer > 0)
                    { /* dead end, backup */
                        curMapStackPointer--;
                        dist += 3;
                    }
                    else
                    {
                        return false; /* give up at start  */
                    }

                }
            }

            return false; /* gone MAX_TRAFFIC_DISTANCE */
        }

        public Direction TryGo(Position pos, Direction dirLast) {
            Direction[] directions = new Direction[4];

            // Find connections from current position.
            Direction dir = Direction.North;
            int count = 0;
            int i = 0;
            for (i = 0; i < 4; i++)
            {
                if (dir != dirLast && RoadTest(GetTileFromMap(pos, dir, (ushort)MapTileCharacters.DIRT)))
                {
                    // found a road in an allowed direction
                    directions[i] = dir;
                    count++;
                }
                else
                {
                    directions[i] = Direction.Invalid;
                }

                dir = dir.Rotate90();
            }

            if (count == 0)
            { // dead end
                return Direction.Invalid;
            }

            // We have at least one way to go.

            if (count == 1)
            { // only one solution
                for (i = 0; i < 4; i++)
                {
                    if (directions[i] != Direction.Invalid)
                    {
                        return directions[i];
                    }
                }
            }

            // more than one choice, draw a random number.
            i = GetRandom16() & 3;
            while (directions[i] == Direction.Invalid)
            {
                i = (i + 1) & 3;
            }
            return directions[i];
        }

        public ushort GetTileFromMap(Position pos, Direction dir, ushort defaultTile) {
            switch (dir)
            {

                case Direction.North:
                    if (pos.Y > 0)
                    {
                        return (ushort)(Map[pos.X,pos.Y - 1] & (ushort)MapTileBits.LowMask);
                    }

                    return defaultTile;

                case Direction.East:
                    if (pos.X < Constants.WorldWidth - 1)
                    {
                        return (ushort)(Map[pos.X + 1,pos.Y] & (ushort)MapTileBits.LowMask);
                    }

                    return defaultTile;

                case Direction.South:
                    if (pos.Y < Constants.WorldHeight - 1)
                    {
                        return (ushort)(Map[pos.X,pos.Y + 1] & (ushort)MapTileBits.LowMask);
                    }

                    return defaultTile;

                case Direction.West:
                    if (pos.X > 0)
                    {
                        return (ushort)(Map[pos.X - 1,pos.Y] & (ushort)MapTileBits.LowMask);
                    }

                    return defaultTile;

                default:
                    return defaultTile;

            }
        }

        public bool DriveDone(Position pos, ZoneType destZone)
        { 
            // FIXME: Use macros to determine the zone type: residential, commercial or industrial.
            /* commercial, industrial, residential destinations */
            ushort[] targetLow = { (ushort)MapTileCharacters.COMBASE, (ushort)MapTileCharacters.LHTHR, (ushort)MapTileCharacters.LHTHR };
            ushort[] targetHigh = { (ushort)MapTileCharacters.NUCLEAR, (ushort)MapTileCharacters.PORT, (ushort)MapTileCharacters.COMBASE };

            //TODO: asserts fix & reenable
            //assert(ZT_NUM_DESTINATIONS == LENGTH_OF(targetLow));
            //assert(ZT_NUM_DESTINATIONS == LENGTH_OF(targetHigh));

            ushort l = targetLow[(int)destZone]; // Lowest acceptable tile value
            ushort h = targetHigh[(int)destZone]; // Highest acceptable tile value

            if (pos.Y > 0)
            {
                ushort z = (ushort)(Map[pos.X,pos.Y - 1] & (ushort)MapTileBits.LowMask);
                if (z >= l && z <= h)
                {
                    return true;
                }
            }

            if (pos.X < (Constants.WorldWidth - 1))
            {
                ushort z = (ushort)(Map[pos.X + 1,pos.Y] & (ushort)MapTileBits.LowMask);
                if (z >= l && z <= h)
                {
                    return true;
                }
            }

            if (pos.Y < (Constants.WorldHeight - 1))
            {
                ushort z = (ushort)(Map[pos.X,pos.Y + 1] & (ushort)MapTileBits.LowMask);
                if (z >= l && z <= h)
                {
                    return true;
                }
            }

            if (pos.X > 0)
            {
                ushort z = (ushort)(Map[pos.X - 1,pos.Y] & (ushort)MapTileBits.LowMask);
                if (z >= l && z <= h)
                {
                    return true;
                }
            }

            return false;
        }

        public bool RoadTest(ushort mapValue)
        {
            ushort tile = (ushort)(mapValue & (ushort)MapTileBits.LowMask);

            if (tile < (ushort)MapTileCharacters.ROADBASE || tile > (ushort)MapTileCharacters.LASTRAIL)
            {
                return false;
            }

            if (tile >= (ushort)MapTileCharacters.POWERBASE && tile < (ushort)MapTileCharacters.LASTPOWER)
            {
                return false;
            }

            return true;
        }
    }
}
