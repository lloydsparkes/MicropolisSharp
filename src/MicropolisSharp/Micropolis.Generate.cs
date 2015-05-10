/* Micropolis.Generate.cs
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

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of generate.cpp
    /// </summary>
    public partial class Micropolis
    {
        public int TerrainTreeLevel { get; private set; }
        public int TerrainLakeLevel { get; private set; }
        public int TerrainCurveLevel { get; private set; }
        public int TerrainCreateIsland { get; private set; }
        public int GeneratedCitySeed { get; private set; }

        public void GenerateMap()
        {
            GenerateSomeCity(GetRandom16());
        }

        public void GenerateSomeCity(int seed)
        {
            CityFileName = "";

            GenerateMap(seed);
            Scenario = Scenario.None;
            CityTime = 0;
            InitSimLoad = 2;
            DoInitialEval = false;

            InitWillStuff();
            ResetMapState();
            ResetEditorState();
            InvalidateMaps();
            UpdateFunds();
            DoSimInit();

            SimUpdate();

            Callback("didGenerateMap", "");
        }

        private void GenerateMap(int seed)
        {
            GeneratedCitySeed = seed;

            SeedRandom(seed);

            // Construct land.
            if (TerrainCreateIsland < 0)
            {
                if (GetRandom(100) < 10)
                { /* chance that island is generated */
                    MakeIsland();
                    return;
                }
            }

            if (TerrainCreateIsland == 1)
            {
                MakeNakedIsland();
            }
            else
            {
                ClearMap();
            }

            // Lay a river.
            if (TerrainCurveLevel != 0)
            {
                int terrainXStart = 40 + GetRandom(Constants.WorldWidth - 80);
                int terrainYStart = 33 + GetRandom(Constants.WorldHeight - 67);

                Position terrainPos = new Position(terrainXStart, terrainYStart);

                DoRivers(terrainPos);
            }

            // Lay a few lakes.
            if (TerrainLakeLevel != 0)
            {
                MakeLakes();
            }

            SmoothRiver();

            // And add trees.
            if (TerrainTreeLevel != 0)
            {
                DoTrees();
            }
        }

        public void ClearMap()
        {
            short x, y;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {
                    Map[x,y] = (ushort)MapTileCharacters.DIRT;
                }
            }
        }

        public void ClearUnnatural()
        {
            int x, y;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {
                    if (Map[x,y] > (ushort)MapTileCharacters.WOODS)
                    {
                        Map[x,y] = (ushort)MapTileCharacters.DIRT;
                    }
                }
            }
        }

        private void MakeNakedIsland()
        {
            const int terrainIslandRadius = Constants.IslandRadius;
            int x, y;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {
                    if ((x < 5) || (x >= Constants.WorldWidth - 5) ||
                        (y < 5) || (y >= Constants.WorldHeight - 5))
                    {
                        Map[x,y] = (ushort)MapTileCharacters.RIVER;
                    }
                    else
                    {
                        Map[x,y] = (ushort)MapTileCharacters.DIRT;
                    }
                }
            }

            for (x = 0; x < Constants.WorldWidth - 5; x += 2)
            {

                int mapY = GetERandom(terrainIslandRadius);
                PlopBRiver(new Position(x, mapY));

                mapY = (Constants.WorldHeight - 10) - GetERandom(terrainIslandRadius);
                PlopBRiver(new Position(x, mapY));

                PlopSRiver(new Position(x, 0));
                PlopSRiver(new Position(x, Constants.WorldHeight - 6));
            }

            for (y = 0; y < Constants.WorldHeight - 5; y += 2)
            {

                int mapX = GetERandom(terrainIslandRadius);
                PlopBRiver(new Position(mapX, y));

                mapX = (Constants.WorldWidth - 10) - GetERandom(terrainIslandRadius);
                PlopBRiver(new Position(mapX, y));

                PlopSRiver(new Position(0, y));
                PlopSRiver(new Position(Constants.WorldWidth - 6, y));
            }
        }

        public void MakeIsland()
        {
            MakeNakedIsland();
            SmoothRiver();
            DoTrees();
        }

        private void MakeLakes()
        {
            int numLakes;

            if (TerrainLakeLevel < 0)
            {
                numLakes = GetRandom(10);
            }
            else
            {
                numLakes = TerrainLakeLevel / 2;
            }

            while (numLakes > 0)
            {
                int x = GetRandom(Constants.WorldWidth - 21) + 10;
                int y = GetRandom(Constants.WorldHeight - 20) + 10;

                MakeSingleLake(new Position(x, y));

                numLakes--;
            }
        }

        public void MakeSingleLake(Position pos)
        {
            int numPlops = GetRandom(12) + 2;

            while (numPlops > 0)
            {
                Position plopPos = new Position(pos, GetRandom(12) - 6, GetRandom(12) - 6);

                if (GetRandom(4).IsTrue())
                {
                    PlopSRiver(plopPos);
                }
                else
                {
                    PlopBRiver(plopPos);
                }

                numPlops--;
            }
        }

        public void TreeSplash(int xLoc, int yLoc)
        {
            int numTrees;

            if (TerrainTreeLevel < 0)
            {
                numTrees = GetRandom(150) + 50;
            }
            else
            {
                numTrees = GetRandom((short)(100 + (TerrainTreeLevel * 2))) + 50;
            }

            Position treePos = new Position(xLoc, yLoc);

            while (numTrees > 0)
            {
                Direction dir = (Direction)(Direction.North + GetRandom(7));
                treePos.Move(dir);

                if (!treePos.TestBounds())
                {
                    return;
                }

                if ((Map[treePos.X,treePos.Y] & (ushort)MapTileBits.LowMask) == (ushort)MapTileCharacters.DIRT)
                {
                    Map[treePos.X,treePos.Y] = (ushort)MapTileCharacters.WOODS | (ushort)MapTileBits.BulldozableOrBurnable;
                }

                numTrees--;
            }
        }

        private void DoTrees()
        {
            int Amount, x, xloc, yloc;

            if (TerrainTreeLevel < 0)
            {
                Amount = GetRandom(100) + 50;
            }
            else
            {
                Amount = TerrainTreeLevel + 3;
            }

            for (x = 0; x < Amount; x++)
            {
                xloc = GetRandom(Constants.WorldWidth - 1);
                yloc = GetRandom(Constants.WorldHeight - 1);
                TreeSplash(xloc, yloc);
            }

            SmoothTrees();
            SmoothTrees();
        }

        private void SmoothRiver()
        {
            short[] dx = { -1, 0, 1, 0 };
            short[] dy = { 0, 1, 0, -1 };
            short[] REdTab = {
                13 | (ushort)MapTileBits.Bulldozable,   13 | (ushort)MapTileBits.Bulldozable,     17 | (ushort)MapTileBits.Bulldozable,     15 | (ushort)MapTileBits.Bulldozable,
                5 | (ushort)MapTileBits.Bulldozable,    2,                19 | (ushort)MapTileBits.Bulldozable,     17 | (ushort)MapTileBits.Bulldozable,
                9 | (ushort)MapTileBits.Bulldozable,    11 | (ushort)MapTileBits.Bulldozable,     2,                13 | (ushort)MapTileBits.Bulldozable,
                7 | (ushort)MapTileBits.Bulldozable,    9 | (ushort)MapTileBits.Bulldozable,      5 | (ushort)MapTileBits.Bulldozable,      2
            };

            int bitIndex, z, xTemp, yTemp;
            short temp, x, y;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {

                    if (Map[x,y] == (ushort)MapTileCharacters.REDGE)
                    {
                        bitIndex = 0;

                        for (z = 0; z < 4; z++)
                        {
                            bitIndex = bitIndex << 1;
                            xTemp = x + dx[z];
                            yTemp = y + dy[z];
                            if (Position.TestBounds(xTemp, yTemp) &&
                                ((Map[xTemp,yTemp] & (ushort)MapTileBits.LowMask) != (ushort)MapTileCharacters.DIRT) &&
                                (((Map[xTemp,yTemp] & (ushort)MapTileBits.LowMask) < (ushort)MapTileCharacters.WOODS_LOW) ||
                                 ((Map[xTemp,yTemp] & (ushort)MapTileBits.LowMask) > (ushort)MapTileCharacters.WOODS_HIGH)))
                            {
                                bitIndex++;
                            }
                        }

                        temp = REdTab[bitIndex & 15];

                        if ((temp != (ushort)MapTileCharacters.RIVER) &&
                            GetRandom(1).IsTrue())
                        {
                            temp++;
                        }

                        Map[x,y] = (ushort)temp;
                    }
                }
            }
        }

        private bool IsTree(ushort cell)
        {
            if ((cell & (ushort)MapTileBits.LowMask) >= (ushort)MapTileCharacters.WOODS_LOW && (cell & (ushort)MapTileBits.LowMask) <= (ushort)MapTileCharacters.WOODS_HIGH)
            {
                return true;
            }

            return false;
        }

        private void SmoothTrees()
        {
            int x, y;
            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {
                    if (IsTree(Map[x,y]))
                    {
                        SmoothTreesAt(x, y, false);
                    }
                }
            }
        }

        private void SmoothTreesAt(int x, int y, bool preserve)
        {
            ToolEffects effects = new ToolEffects(this);

            SmoothTreesAt(x, y, preserve, effects);
            effects.ModifyWorld();
        }

        public void SmoothTreesAt(int x, int y, bool preserve, ToolEffects effects)
        {
            short[] dx = { -1, 0, 1, 0 };
            short[] dy = { 0, 1, 0, -1 };
            short[] treeTable = {
                0,  0,  0,  34,
                0,  0,  36, 35,
                0,  32, 0,  33,
                30, 31, 29, 37,
            };

            if (!IsTree(effects.GetMapValue(x, y)))
            {
                return;
            }

            int bitIndex = 0;
            int z;
            for (z = 0; z < 4; z++)
            {
                bitIndex = bitIndex << 1;
                int xTemp = x + dx[z];
                int yTemp = y + dy[z];
                if (Position.TestBounds(xTemp, yTemp)
                                    && IsTree(effects.GetMapValue(xTemp, yTemp)))
                {
                    bitIndex++;
                }
            }

            int temp = treeTable[bitIndex & 15];
            if (temp.IsTrue())
            {
                if (temp != (ushort)MapTileCharacters.WOODS)
                {
                    if (((x + y) & 1).IsTrue())
                    {
                        temp = temp - 8;
                    }
                }
                effects.SetMapValue(x, y, (ushort)(temp | (ushort)MapTileBits.Bulldozable));
            }
            else
            {
                if (!preserve)
                {
                    effects.SetMapValue(x, y, (ushort)temp);
                }
            }
        }
                
        private void DoRivers(Position terrainPos)
        {
            Direction riverDir;   // Global direction of the river
            Direction terrainDir; // Local direction of the river

            riverDir = (Direction)(Direction.North + (GetRandom(3) * 2));
            DoBRiver(terrainPos, riverDir, riverDir);

            riverDir = riverDir.Rotate180();
            terrainDir = DoBRiver(terrainPos, riverDir, riverDir);

            riverDir = (Direction)(Direction.North + (GetRandom(3) * 2));
            DoSRiver(terrainPos, riverDir, terrainDir);
        }

        private Direction DoBRiver(Position riverPos, Direction riverDir, Direction terrainDir)
        {
            int rate1, rate2;

            if (TerrainCurveLevel < 0)
            {
                rate1 = 100;
                rate2 = 200;
            }
            else
            {
                rate1 = TerrainCurveLevel + 10;
                rate2 = TerrainCurveLevel + 100;
            }

            Position pos = new Position(riverPos);

            while (Position.TestBounds(pos.X + 4, pos.Y + 4))
            {
                PlopBRiver(pos);
                if (GetRandom((short)rate1) < 10)
                {
                    terrainDir = riverDir;
                }
                else
                {
                    if (GetRandom((short)rate2) > 90)
                    {
                        terrainDir = terrainDir.Rotate45();
                    }
                    if (GetRandom((short)rate2) > 90)
                    {
                        terrainDir = terrainDir.Rotate45(7);
                    }
                }
                pos.Move(terrainDir);
            }

            return terrainDir;
        }

        private Direction DoSRiver(Position riverPos, Direction riverDir, Direction terrainDir)
        {
            int rate1, rate2;

            if (TerrainCurveLevel < 0)
            {
                rate1 = 100;
                rate2 = 200;
            }
            else
            {
                rate1 = TerrainCurveLevel + 10;
                rate2 = TerrainCurveLevel + 100;
            }

            Position pos = new Position(riverPos);

            while (Position.TestBounds(pos.X + 3, pos.Y + 3))
            {
                //printf("doSRiver %d %d td %d rd %d\n", pos.posX, pos.posY, terrainDir, riverDir);
                PlopSRiver(pos);
                if (GetRandom((short)rate1) < 10)
                {
                    terrainDir = riverDir;
                }
                else
                {
                    if (GetRandom((short)rate2) > 90)
                    {
                        terrainDir = terrainDir.Rotate45();
                    }
                    if (GetRandom((short)rate2) > 90)
                    {
                        terrainDir = terrainDir.Rotate45(7);
                    }
                }
                pos.Move(terrainDir);
            }

            return terrainDir;
        }

        private void PutOnMap(ushort mChar, int xLoc, int yLoc)
        {
            if (mChar == 0)
            {
                return;
            }

            if (!Position.TestBounds(xLoc, yLoc))
            {
                return;
            }

            ushort temp = Map[xLoc,yLoc];

            if (temp != (ushort)MapTileCharacters.DIRT)
            {
                temp = (ushort)(temp & (ushort)MapTileBits.LowMask);
                if (temp == (ushort)MapTileCharacters.RIVER)
                {
                    if (mChar != (ushort)MapTileCharacters.CHANNEL)
                    {
                        return;
                    }
                }
                if (temp == (ushort)MapTileCharacters.CHANNEL)
                {
                    return;
                }
            }
            Map[xLoc,yLoc] = mChar;
        }

        public void PlopBRiver(Position pos)
        {
            int x, y;
            ushort[,] BRMatrix = 
            {
                {     0,       0,       0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.REDGE,       0,       0,       0 },
                {     0,       0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE,       0,       0 },
                {     0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE,       0 },
                { (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE },
                { (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.CHANNEL,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE },
                { (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE },
                {     0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE,       0 },
                {     0,       0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.RIVER,   (ushort)MapTileCharacters.REDGE,       0,       0 },
                {     0,       0,       0,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.REDGE,   (ushort)MapTileCharacters.REDGE,       0,       0,       0 },
            };

            for (x = 0; x < 9; x++)
            {
                for (y = 0; y < 9; y++)
                {
                    PutOnMap(BRMatrix[y,x], pos.X + x, pos.Y + y);
                }
            }
        }

        public void PlopSRiver(Position pos)
        {
            int x, y;
            ushort[,] SRMatrix =
            {
                {     0,     0, (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.REDGE,     0,     0 },
                {     0, (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.REDGE,     0 },
                { (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.REDGE },
                { (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.REDGE },
                {     0, (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.REDGE,     0 },
                {     0,     0, (ushort)MapTileCharacters.REDGE, (ushort)MapTileCharacters.REDGE,     0,     0 },
            };

            for (x = 0; x < 6; x++)
            {
                for (y = 0; y < 6; y++)
                {
                    PutOnMap(SRMatrix[y,x], pos.X + x, pos.Y + y);
                }
            }
        }

        private void SmoothWater() {
            int x, y;
            ushort tile;
            Direction dir;

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {

                    tile = (ushort)(Map[x,y] & (ushort)MapTileBits.LowMask);

                    /* If (x, y) is water: */
                    if (tile >= (ushort)MapTileCharacters.WATER_LOW && tile <= (ushort)MapTileCharacters.WATER_HIGH)
                    {

                        Position pos = new Position(x, y);
                        for (dir = Direction.Begin; dir < Direction.End; dir = dir.Increment90())
                        {

                            /* If getting a tile off-map, condition below fails. */
                            // @note I think this may have been a bug, since it always uses DIR2_WEST instead of dir.
                            //tile = getTileFromMap(pos, DIR2_WEST, WATER_LOW);
                            tile = GetTileFromMap(pos, dir, (ushort)MapTileCharacters.WATER_LOW);

                            /* If nearest object is not water: */
                            if (tile < (ushort)MapTileCharacters.WATER_LOW || tile > (ushort)MapTileCharacters.WATER_HIGH)
                            {
                                Map[x,y] = (ushort)MapTileCharacters.REDGE; /* set river edge */
                                break; // Continue with next tile
                            }
                        }
                    }
                }
            }

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {

                    tile = (ushort)(Map[x, y] & (ushort)MapTileBits.LowMask);

                    /* If water which is not a channel: */
                    if (tile != (ushort)MapTileCharacters.CHANNEL && tile >= (ushort)MapTileCharacters.WATER_LOW && tile <= (ushort)MapTileCharacters.WATER_HIGH)
                    {

                        bool makeRiver = true; // make (x, y) a river

                        Position pos = new Position(x, y);
                        for (dir = Direction.Begin; dir < Direction.End; dir = dir.Increment90())
                        {

                            /* If getting a tile off-map, condition below fails. */
                            // @note I think this may have been a bug, since it always uses DIR2_WEST instead of dir.
                            //tile = getTileFromMap(pos, DIR2_WEST, WATER_LOW);
                            tile = GetTileFromMap(pos, dir, (ushort)MapTileCharacters.WATER_LOW);

                            /* If nearest object is not water: */
                            if (tile < (ushort)MapTileCharacters.WATER_LOW || tile > (ushort)MapTileCharacters.WATER_HIGH)
                            {
                                makeRiver = false;
                                break;
                            }
                        }

                        if (makeRiver)
                        {
                            Map[x,y] = (ushort)MapTileCharacters.RIVER; /* make it a river */
                        }
                    }
                }
            }

            for (x = 0; x < Constants.WorldWidth; x++)
            {
                for (y = 0; y < Constants.WorldHeight; y++)
                {

                    tile = (ushort)(Map[x, y] & (ushort)MapTileBits.LowMask);

                    /* If woods: */
                    if (tile >= (ushort)MapTileCharacters.WOODS_LOW && tile <= (ushort)MapTileCharacters.WOODS_HIGH)
                    {

                        Position pos = new Position(x, y);
                        for (dir = Direction.Begin; dir < Direction.End; dir = dir.Increment90())
                        {

                            /* If getting a tile off-map, condition below fails. */
                            // @note I think this may have been a bug, since it always uses DIR2_WEST instead of dir.
                            //tile = getTileFromMap(pos, DIR2_WEST, WATER_LOW);
                            tile = GetTileFromMap(pos, dir, unchecked((ushort)MapTileCharacters.TILE_INVALID));

                            if (tile == (ushort)MapTileCharacters.RIVER || tile == (ushort)MapTileCharacters.CHANNEL)
                            {
                                Map[x,y] = (ushort)MapTileCharacters.REDGE; /* make it water's edge */
                                break;
                            }
                        }
                    }
                }
            }
        }      
    }
}
