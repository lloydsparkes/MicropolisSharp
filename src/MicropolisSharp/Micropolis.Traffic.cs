/* Micropolis.Traffic.cs
 *
 * MicropolisSharp. This library is a port of the original code 
 *   (http://wiki.laptop.org/go/Micropolis) to C#/.Net. See the README 
 * for more details.
 *
 * If you need assistance with the Port please raise an issue on GitHub
 *   (https://github.com/lloydsparkes/MicropolisSharp/issues)
 * 
 * The Original code is - Copyright (C) 1989 - 2007 Electronic Arts Inc.  
 * If you need assistance with this program, you may contact:
 *   http://wiki.laptop.org/go/Micropolis  or email  micropolis@laptop.org.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or (at
 * your option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.  You should have received a
 * copy of the GNU General Public License along with this program.  If
 * not, see <http://www.gnu.org/licenses/>.
 *
 *             ADDITIONAL TERMS per GNU GPL Section 7
 *
 * No trademark or publicity rights are granted.  This license does NOT
 * give you any right, title or interest in the trademark SimCity or any
 * other Electronic Arts trademark.  You may not distribute any
 * modification of this program using the trademark SimCity or claim any
 * affliation or association with Electronic Arts Inc. or its employees.
 *
 * Any propagation or conveyance of this program must include this
 * copyright notice and these terms.
 *
 * If you convey this program (or any modifications of it) and assume
 * contractual liability for the program to recipients of it, you agree
 * to indemnify Electronic Arts for any liability that those contractual
 * assumptions impose on Electronic Arts.
 *
 * You may not misrepresent the origins of this program; modified
 * versions of the program must be marked as such and not identified as
 * the original program.
 *
 * This disclaimer supplements the one included in the General Public
 * License.  TO THE FULLEST EXTENT PERMISSIBLE UNDER APPLICABLE LAW, THIS
 * PROGRAM IS PROVIDED TO YOU "AS IS," WITH ALL FAULTS, WITHOUT WARRANTY
 * OF ANY KIND, AND YOUR USE IS AT YOUR SOLE RISK.  THE ENTIRE RISK OF
 * SATISFACTORY QUALITY AND PERFORMANCE RESIDES WITH YOU.  ELECTRONIC ARTS
 * DISCLAIMS ANY AND ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES,
 * INCLUDING IMPLIED WARRANTIES OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * FITNESS FOR A PARTICULAR PURPOSE, NONINFRINGEMENT OF THIRD PARTY
 * RIGHTS, AND WARRANTIES (IF ANY) ARISING FROM A COURSE OF DEALING,
 * USAGE, OR TRADE PRACTICE.  ELECTRONIC ARTS DOES NOT WARRANT AGAINST
 * INTERFERENCE WITH YOUR ENJOYMENT OF THE PROGRAM; THAT THE PROGRAM WILL
 * MEET YOUR REQUIREMENTS; THAT OPERATION OF THE PROGRAM WILL BE
 * UNINTERRUPTED OR ERROR-FREE, OR THAT THE PROGRAM WILL BE COMPATIBLE
 * WITH THIRD PARTY SOFTWARE OR THAT ANY ERRORS IN THE PROGRAM WILL BE
 * CORRECTED.  NO ORAL OR WRITTEN ADVICE PROVIDED BY ELECTRONIC ARTS OR
 * ANY AUTHORIZED REPRESENTATIVE SHALL CREATE A WARRANTY.  SOME
 * JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF OR LIMITATIONS ON IMPLIED
 * WARRANTIES OR THE LIMITATIONS ON THE APPLICABLE STATUTORY RIGHTS OF A
 * CONSUMER, SO SOME OR ALL OF THE ABOVE EXCLUSIONS AND LIMITATIONS MAY
 * NOT APPLY TO YOU.
 */
using MicropolisSharp.Types;
using System;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of traffic.cpp
    /// </summary>
    public partial class Micropolis
    {
        private short curMapStackPointer;
        private Position[] curMapStackXY;
        private int trafMaxX;
        private int trafMaxY;

        /// <summary>
        /// Makes traffic starting from the road tile at \x, \y.
        /// </summary>
        /// <param name="x">Start x position of the attempt</param>
        /// <param name="y">Start y position of the attempt</param>
        /// <param name="dest">Zone type to go to.</param>
        /// <returns>1 if connection found, 0 if not, -1 if no road connection</returns>
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

        /// <summary>
        /// Find a connection over a road from position \a x \a y to a specified zone type.
        /// </summary>
        /// <param name="x">Start x position of the attempt</param>
        /// <param name="y">Start y position of the attempt</param>
        /// <param name="dest">Zone type to go to.</param>
        /// <returns>1 if connection found, 0 if not, -1 if no road connection</returns>
        public short MakeTraffic(int x, int y, ZoneType dest)
        {
            Position startPos = new Position(x,y);
            return MakeTraffic(startPos, dest);
        }

        /// <summary>
        /// Find a connection over a road from \a startPos to a specified zone type.
        /// </summary>
        /// <param name="startPos">Start position of the attempt.</param>
        /// <param name="dest">Zone type to go to.</param>
        /// <returns>1 if connection found, 0 if not, -1 if no road connection</returns>
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

        /// <summary>
        /// Update the #trafficDensityMap from the positions at the #curMapStackXY stack.
        /// </summary>
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

        /// <summary>
        /// Push a position onto the position stack.
        /// </summary>
        /// <param name="pos">Position to push.</param>
        public void PushPos(Position pos)
        {
            //TODO: Fix & Enable Assert
            //assert(curMapStackPointer < MAX_TRAFFIC_DISTANCE + 1);
            curMapStackPointer++;
            if (curMapStackPointer > Constants.MaxTrafficDistance)
                return;
            curMapStackXY[curMapStackPointer] = pos;
        }

        /// <summary>
        /// Pull top-most position from the position stack.
        /// </summary>
        /// <returns>Pulled position.</returns>
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

        /// <summary>
        /// Find a connection to a road at the perimeter.
        /// 
        /// TODO: We could randomize the search.
        /// </summary>
        /// <param name="pos">Starting position. Gets updated when a perimeter has been found.</param>
        /// <returns>Indication that a connection has been found.</returns>
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

        /// <summary>
        /// Find a telecom connection at the perimeter.
        /// 
        /// TODO: Decide whether we want telecomm code.
        /// </summary>
        /// <param name="pos">Position to start searching.</param>
        /// <returns>A telecom connection has been found.</returns>
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

        /// <summary>
        /// Try to drive to a destination.
        /// 
        /// TODO: The stack is popped, but position and dirLast is not updated
        /// </summary>
        /// <param name="startPos">Starting position.</param>
        /// <param name="dest">Zonetype to drive to.</param>
        /// <returns>Was drive succesful?</returns>
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

        /// <summary>
        /// Try to drive one tile in a random direction.
        /// </summary>
        /// <param name="pos">Current position.</param>
        /// <param name="dirLast">Forbidden direction for movement (to prevent reversing).</param>
        /// <returns>Direction of movement, \c #DIR2_INVALID is returned if not moved.</returns>
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

        /// <summary>
        /// Get neighbouring tile from the map.
        /// </summary>
        /// <param name="pos">Current position.</param>
        /// <param name="dir">Direction of neighbouring tile, only horizontal and vertical directions are supported.</param>
        /// <param name="defaultTile">Tile to return if off-map.</param>
        /// <returns>The tile in the indicated direction. If tile is off-world or an incorrect direction is given, \c DIRT is returned.</returns>
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

        /// <summary>
        /// Has the journey arrived at its destination?
        /// </summary>
        /// <param name="pos">Current position.</param>
        /// <param name="destZone">Zonetype to drive to.</param>
        /// <returns>Destination has been reached.</returns>
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

        /// <summary>
        /// Can the given tile be used as road?
        /// </summary>
        /// <param name="mapValue">Value from the map.</param>
        /// <returns>Indication that you can drive on the given tile</returns>
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
