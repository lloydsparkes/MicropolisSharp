/* Micropolis.Scan.cs
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
    /// Partial Class Containing the content of scan.cpp
    /// </summary>
    public partial class Micropolis
    {
        public short NewMap { get; private set; }

        public short[] NewMapFlags { get; private set; }

        public short CityCenterX { get; private set; }
        public short CityCenterY { get; private set; }

        public short PollutionMaxX { get; private set; }
        public short PollutionMaxY { get; private set; }

        public short CrimeMaxX { get; private set; }
        public short CrimeMaxY { get; private set; }

        public long DonDither { get; private set; }

        private static void SmoothStationMap(ShortMap8 map)
        {
            short x, y, edge;
            ShortMap8 tempMap = new ShortMap8(map);

            for (x = 0; x < tempMap.width; x++)
            {
                for (y = 0; y < tempMap.height; y++)
                {
                    edge = 0;
                    if (x > 0)
                    {
                        edge += tempMap.Get(x - 1, y);
                    }
                    if (x < tempMap.width - 1)
                    {
                        edge += tempMap.Get(x + 1, y);
                    }
                    if (y > 0)
                    {
                        edge += tempMap.Get(x, y - 1);
                    }
                    if (y < tempMap.height - 1)
                    {
                        edge += tempMap.Get(x, y + 1);
                    }
                    edge = (short)(tempMap.Get(x, y) + edge / 4);
                    map.Set(x, y, (short)(edge / 2));
                }
            }
        }

        public void FireAnalysis()
        {
            SmoothStationMap(FireStationMap);
            SmoothStationMap(FireStationMap);
            SmoothStationMap(FireStationMap);

            FireStationEffectMap = FireStationMap;

            NewMapFlags[(int)MapType.FireRadius] = 1;
            NewMapFlags[(int)MapType.Dynamic] = 1;
        }

        public void PopulationDensityScan()
        { 
            /*  sets: populationDensityMap, , , comRateMap  */
            TempMap1.Clear();
            long Xtot = 0;
            long Ytot = 0;
            long Ztot = 0;
            for (int x = 0; x < Constants.WorldWidth; x++)
            {
                for (int y = 0; y < Constants.WorldHeight; y++)
                {
                    ushort mapValue = Map[x,y];
                    if ((mapValue & (ushort)MapTileBits.CenterOfZone).IsTrue())
                    {
                        ushort mapTile = (ushort)(mapValue & (ushort)MapTileBits.LowMask);
                        int pop = GetPopulationDensity(new Position(x, y), mapTile) * 8;
                        pop = Math.Min(pop, 254);

                        TempMap1.WorldSet(x, y, (Byte)pop);
                        Xtot += x;
                        Ytot += y;
                        Ztot++;
                    }
                }
            }

            DoSmooth1(); // tempMap1 -> tempMap2
            DoSmooth2(); // tempMap2 -> tempMap1
            DoSmooth1(); // tempMap1 -> tempMap2

            //TODO: Redo Asserts
            //assert(populationDensityMap.MAP_W == tempMap2.MAP_W);
            //assert(populationDensityMap.MAP_H == tempMap2.MAP_H);

            // Copy tempMap2 to populationDensityMap, multiplying by 2
            for(int x = 0; x < PopulationDensityMap.width; x++)
            {
                for(int y = 0; y < PopulationDensityMap.height; y++)
                {
                    PopulationDensityMap.Set(x, y, TempMap2.Get(x, y));
                }
            }

            ComputeComRateMap();          /* Compute the comRateMap */


            // Compute new city center
            if (Ztot > 0)
            {               /* Find Center of Mass for City */
                CityCenterX = (short)(Xtot / Ztot);
                CityCenterY = (short)(Ytot / Ztot);
            }
            else
            {
                CityCenterX = Constants.WorldWidth / 2;  /* if pop==0 center of map is city center */
                CityCenterY = Constants.WorldHeight / 2;
            }

            // Set flags for updated maps
            NewMapFlags[(int)MapType.PopulationDensity] = 1;
            NewMapFlags[(int)MapType.RateOfGrowth] = 1;
            NewMapFlags[(int)MapType.Dynamic] = 1;
        }

        public int GetPopulationDensity(Position pos, ushort tile)
        {
            int pop;

            if (tile == (ushort)MapTileCharacters.FREEZ)
            {
                pop = DoFreePop(pos);
                return pop;
            }

            if (tile < (ushort)MapTileCharacters.COMBASE)
            {
                pop = GetResZonePop(tile);
                return pop;
            }

            if (tile < (ushort)MapTileCharacters.INDBASE)
            {
                pop = GetComZonePop(tile) * 8;
                return pop;
            }

            if (tile < (ushort)MapTileCharacters.PORTBASE)
            {
                pop = GetIndZonePop(tile) * 8;
                return pop;
            }

            return 0;
        }

        public void PollutionTerrainLandValueScan()
        { /* Does pollution, terrain, land value */
            long ptot, LVtot;
            int x, y, z, dis;
            int pollutionLevel, loc, worldX, worldY, Mx, My, pnum, LVnum, pmax;

            // tempMap3 is a map of development density, smoothed into terrainMap.
            TempMap3.Clear();

            LVtot = 0;
            LVnum = 0;

            for (x = 0; x < LandValueMap.width; x++)
            {
                for (y = 0; y < LandValueMap.height; y++)
                {
                    pollutionLevel = 0;
                    bool landValueFlag = false;
                    worldX = x * 2;
                    worldY = y * 2;

                    for (Mx = worldX; Mx <= worldX + 1; Mx++)
                    {
                        for (My = worldY; My <= worldY + 1; My++)
                        {
                            loc = (Map[Mx,My] & (ushort)MapTileBits.LowMask);
                            if (loc.IsTrue())
                            {
                                if (loc < (ushort)MapTileCharacters.RUBBLE)
                                {
                                    // Increment terrain memory.
                                    byte value = TempMap3.Get(x >> 1, y >> 1);
                                    TempMap3.Set(x >> 1, y >> 1, (byte)(value + 15));
                                    continue;
                                }
                                pollutionLevel += GetPollutionValue(loc);
                                if (loc >= (ushort)MapTileCharacters.ROADBASE)
                                {
                                    landValueFlag = true;
                                }
                            }
                        }
                    }

                    /* XXX ??? This might have to do with the radiation tile returning -40.
                                if (pollutionLevel < 0) {
                                    pollutionLevel = 250;
                                }
                    */

                    pollutionLevel = Math.Min(pollutionLevel, 255);
                    TempMap1.Set(x, y, (byte)pollutionLevel);

                    if (landValueFlag)
                    {              /* LandValue Equation */
                        dis = 34 - GetCityCenterDistance(worldX, worldY) / 2;
                        dis = dis << 2;
                        dis += TerrainDensityMap.Get(x >> 1, y >> 1);
                        dis -= PollutionDensityMap.Get(x, y);
                        if (CrimeRateMap.Get(x, y) > 190)
                        {
                            dis -= 20;
                        }
                        dis = Utilities.Restrict(dis, 1, 250);
                        LandValueMap.Set(x, y, (byte)dis);
                        LVtot += dis;
                        LVnum++;
                    }
                    else
                    {
                        LandValueMap.Set(x, y, 0);
                    }
                }
            }

            if (LVnum > 0)
            {
                LandValueAverage = (short)(LVtot / LVnum);
            }
            else
            {
                LandValueAverage = 0;
            }

            DoSmooth1(); // tempMap1 -> tempMap2
            DoSmooth2(); // tempMap2 -> tempMap1

            pmax = 0;
            pnum = 0;
            ptot = 0;

            for (x = 0; x < Constants.WorldWidth; x += PollutionDensityMap.BlockSize)
            {
                for (y = 0; y < Constants.WorldHeight; y += PollutionDensityMap.BlockSize)
                {
                    z = TempMap1.WorldGet(x, y);
                    PollutionDensityMap.WorldSet(x, y, (byte)z);

                    if (z.IsTrue())
                    { /*  get pollute average  */
                        pnum++;
                        ptot += z;
                        /* find max pol for monster  */
                        if (z > pmax || (z == pmax && (GetRandom16() & 3) == 0))
                        {
                            pmax = z;
                            PollutionMaxX = (short)x;
                            PollutionMaxY = (short)y;
                        }
                    }
                }
            }
            if (pnum.IsTrue())
            {
                PollutionAverage = (short)(ptot / pnum);
            }
            else
            {
                PollutionAverage = 0;
            }

            SmoothTerrain();

            NewMapFlags[(int)MapType.Pollution] = 1;
            NewMapFlags[(int)MapType.LandValue] = 1;
            NewMapFlags[(int)MapType.Dynamic] = 1;
        }

        public int GetPollutionValue(int loc)
        {
            if (loc < (ushort)MapTileCharacters.POWERBASE)
            {

                if (loc >= (ushort)MapTileCharacters.HTRFBASE)
                {
                    return /* 25 */ 75;     /* heavy traf  */
                }

                if (loc >= (ushort)MapTileCharacters.LTRFBASE)
                {
                    return /* 10 */ 50;     /* light traf  */
                }

                if (loc < (ushort)MapTileCharacters.ROADBASE)
                {

                    if (loc > (ushort)MapTileCharacters.FIREBASE)
                    {
                        return /* 60 */ 90;
                    }

                    /* XXX: Why negative pollution from radiation? */
                    if (loc >= (ushort)MapTileCharacters.RADTILE)
                    {
                        return /* -40 */ 255; /* radioactivity  */
                    }

                }
                return 0;
            }

            if (loc <= (ushort)MapTileCharacters.LASTIND)
            {
                return 0;
            }

            if (loc < (ushort)MapTileCharacters.PORTBASE)
            {
                return 50;        /* Ind  */
            }

            if (loc <= (ushort)MapTileCharacters.LASTPOWERPLANT)
            {
                return /* 60 */ 100;      /* prt, aprt, cpp */
            }

            return 0;
        }

        public int GetCityCenterDistance(int x, int y)
        {
            int xDis, yDis;

            if (x > CityCenterX)
            {
                xDis = x - CityCenterX;
            }
            else
            {
                xDis = CityCenterX - x;
            }

            if (y > CityCenterY)
            {
                yDis = y - CityCenterY;
            }
            else
            {
                yDis = CityCenterY - y;
            }

            return Math.Min(xDis + yDis, 64);
        }

        public void CrimeScan() {
            SmoothStationMap(PoliceStationMap);
            SmoothStationMap(PoliceStationMap);
            SmoothStationMap(PoliceStationMap);

            long totz = 0;
            int numz = 0;
            int cmax = 0;

            for (int x = 0; x < Constants.WorldWidth; x += CrimeRateMap.BlockSize)
            {
                for (int y = 0; y < Constants.WorldHeight; y += CrimeRateMap.BlockSize)
                {
                    int z = LandValueMap.WorldGet(x, y);
                    if (z > 0)
                    {
                        ++numz;
                        z = 128 - z;
                        z += PopulationDensityMap.WorldGet(x, y);
                        z = Math.Min(z, 300);
                        z -= PoliceStationMap.WorldGet(x, y);
                        z = Utilities.Restrict(z, 0, 250);
                        CrimeRateMap.WorldSet(x, y, (Byte)z);
                        totz += z;

                        // Update new crime hot-spot
                        if (z > cmax || (z == cmax && (GetRandom16() & 3) == 0))
                        {
                            cmax = z;
                            CrimeMaxX = (short)x;
                            CrimeMaxY = (short)y;
                        }

                    }
                    else
                    {
                        CrimeRateMap.WorldSet(x, y, 0);
                    }
                }
            }

            if (numz > 0)
            {
                CrimeAverage = (short)(totz / numz);
            }
            else
            {
                CrimeAverage = 0;
            }

            PoliceStationEffectMap = PoliceStationMap;

            NewMapFlags[(int)MapType.Crime] = 1;
            NewMapFlags[(int)MapType.PoliceRadius] = 1;
            NewMapFlags[(int)MapType.Dynamic] = 1;
        }

        public void SmoothTerrain() {
            if ((DonDither & 1).IsTrue())
            {
                int x, y = 0, dir = 1;
                int z = 0;

                for (x = 0; x < TerrainDensityMap.width; x++)
                {
                    for (; y != TerrainDensityMap.height && y != -1; y += dir)
                    {
                        z +=
                            TempMap3.Get((x == 0) ? x : (x - 1), y) +
                            TempMap3.Get((x == (TerrainDensityMap.width - 1)) ? x : (x + 1), y) +
                            TempMap3.Get(x, (y == 0) ? (0) : (y - 1)) +
                            TempMap3.Get(x, (y == (TerrainDensityMap.height - 1)) ? y : (y + 1)) +
                            (TempMap3.Get(x, y) << 2);
                        Byte val = (Byte)(z / 8);
                        TerrainDensityMap.Set(x, y, val);
                        z &= 0x7;
                    }
                    dir = -dir;
                    y += dir;
                }
            }
            else
            {
                short x, y;

                for (x = 0; x < TerrainDensityMap.width; x++)
                {
                    for (y = 0; y < TerrainDensityMap.height; y++)
                    {
                        int z = 0;
                        if (x > 0)
                        {
                            z += TempMap3.Get(x - 1, y);
                        }
                        if (x < (TerrainDensityMap.width - 1))
                        {
                            z += TempMap3.Get(x + 1, y);
                        }
                        if (y > 0)
                        {
                            z += TempMap3.Get(x, y - 1);
                        }
                        if (y < (TerrainDensityMap.height - 1))
                        {
                            z += TempMap3.Get(x, y + 1);
                        }
                        Byte val = (Byte)((z / 4 + TempMap3.Get(x, y)) / 2);
                        TerrainDensityMap.Set(x, y, val);
                    }
                }
            }
        }

        private static void smoothDitherMap(ByteMap2 srcMap, ByteMap2 destMap, bool dither)
        {
            if (dither)
            {
                int x, y = 0, z = 0, dir = 1;

                for (x = 0; x < srcMap.width; x++)
                {
                    for (; y != srcMap.height && y != -1; y += dir)
                    {
                        z +=
                            srcMap.Get((x == 0) ? x : (x - 1), y) +
                            srcMap.Get((x == srcMap.width - 1) ? x : (x + 1), y) +
                            srcMap.Get(x, (y == 0) ? (0) : (y - 1)) +
                            srcMap.Get(x, (y == (srcMap.height - 1)) ? y : (y + 1)) +
                            srcMap.Get(x, y);
                        Byte val = (Byte)(z / 4);
                        destMap.Set(x, y, val);
                        z &= 3;
                    }
                    dir = -dir;
                    y += dir;
                }
            }
            else
            {
                int x, y, z;

                for (x = 0; x < srcMap.width; x++)
                {
                    for (y = 0; y < srcMap.height; y++)
                    {
                        z = 0;
                        if (x > 0)
                        {
                            z += srcMap.Get(x - 1, y);
                        }
                        if (x < srcMap.width - 1)
                        {
                            z += srcMap.Get(x + 1, y);
                        }
                        if (y > 0)
                        {
                            z += srcMap.Get(x, y - 1);
                        }
                        if (y < (srcMap.height - 1))
                        {
                            z += srcMap.Get(x, y + 1);
                        }
                        z = (z + srcMap.Get(x, y)) >> 2;
                        if (z > 255)
                        {
                            z = 255;
                        }
                        destMap.Set(x, y, (Byte)z);
                    }
                }
            }
        }

        public void DoSmooth1() { smoothDitherMap(TempMap1, TempMap2, (DonDither & 2).IsTrue()); }
        public void DoSmooth2() { smoothDitherMap(TempMap2, TempMap1, (DonDither & 4).IsTrue()); }

        public void ComputeComRateMap()
        {
            int x, y, z;

            for (x = 0; x < ComRateMap.width; x++)
            {
                for (y = 0; y < ComRateMap.height; y++)
                {
                    z = (short)(GetCityCenterDistance(x * 8, y * 8) / 2); // 0..32
                    z = z * 4;  // 0..128
                    z = 64 - z; // 64..-64
                    ComRateMap.Set(x, y, (short)z);
                }
            }
        }
    }
}
