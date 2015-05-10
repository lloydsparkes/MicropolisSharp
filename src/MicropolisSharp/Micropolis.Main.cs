/* Micropolis.Main.cs
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
using System.IO;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of main.cpp
    /// </summary>
    public partial class Micropolis
    {
        public int SimLoops { get; private set; }
        public int SimPass { get; private set; }
        public bool SimPaused { get; private set; }
        public int SimPausedSpeed { get; private set; }
        public int SimPasses { get; private set; }

        public bool TilesAnimated { get; private set; }

        public bool DoMessages { get; private set; }
        public bool DoAnimation { get; private set; }
        public bool DoNotices { get; private set; }

        public string CityFileName { get; private set; }
        public string CityName { get; private set; }

        public int HeatSteps { get; private set; }
        public int HeatFlow { get; private set; }
        public int HeatRule { get; private set; }
        public int HeatWrap { get; private set; }

        public short CellSrc { get; private set; }
        public short CellDest { get; private set; }
        public ushort[,] CellMap { get; private set; }

        public string GetMicropolisVersion()
        {
            return "5.0";
        }

        public static bool TestDirectory(string dir)
        {
            return Directory.Exists(dir);
        }

        public void EnvironmentInit()
        {
            string simHome = Environment.GetEnvironmentVariable("SIMHOME");
            if (string.IsNullOrWhiteSpace(simHome))
            {
                simHome = Directory.GetCurrentDirectory();
            }

            if (TestDirectory(simHome))
            {
                resourceDir = simHome + Path.PathSeparator + "res" + Path.PathSeparator;
                if (TestDirectory(resourceDir))
                {
                    return;
                }
            }

            //TODO: Exception - cannot find res dir
        }

        public void SimInit()
        {
            SetEnableSound(true); // Enable sound
            MustUpdateOptions = true; // Update options displayed at user
            Scenario = Scenario.None;
            StartingYear = 1900;
            SimPasses = 1;
            SimPass = 0;
            SetAutoGoTo(true); // Enable auto-goto
            SetCityTax(7);
            CityTime = 50;
            SetEnableDisasters(true); // Enable disasters
            SetAutoBulldoze(true); // Enable auto bulldoze
            SetAutoBudget(true); // Enable auto-budget
            BlinkFlag = 1;
            SimSpeed = 3;
            ChangeEval();
            SimPaused = false; // Simulation is running
            SimLoops = 0;
            InitSimLoad = 2;

            InitMapArrays();
            InitGraphs();
            InitFundingLevel();
            ResetMapState();
            ResetEditorState();
            ClearMap();
            InitWillStuff();
            SetFunds(5000);
            SetGameLevelFunds(Levels.Easy);
            SetSpeed(0);
            SetPasses(1);
        }

        public void SimUpdate()
        {     
            
            //printf("simUpdate\n");
            BlinkFlag = (short)(((TickCount() % 60) < 30) ? 1 : -1);

            if (SimSpeed.IsTrue() && HeatSteps.IsFalse())
            {
                TilesAnimated = false;
            }

            DoUpdateHeads();
            GraphDoer();
            UpdateBudget();
            ScoreDoer();
        }

        private ushort SimHeat_GetValue(ushort[,] map, int pos)
        {
            int posX = pos / map.GetLength(1);
            int posY = pos - (map.GetLength(1) * posX);
            return map[posX, posY];
        }

        private void SimHeat_SetValue(ushort[,] map, int pos, ushort value)
        {
            int posX = pos / map.GetLength(1);
            int posY = pos - (map.GetLength(1) * posX);
            map[posX, posY] = value;
        }

        private void SimHeat_CopyPosition(int destInd, int srcInd, ushort[,] dest, ushort[,] src)
        {
            int srcPosX = srcInd / src.GetLength(1);
            int srcPosY = srcInd - (src.GetLength(1) * srcPosX);

            int destPosX = destInd / dest.GetLength(1);
            int destPosY = destInd - (dest.GetLength(1) * destPosX);

            dest[destPosX, destPosY] = src[srcPosX, srcPosY];
        }

        private void SimHeat_MemCopy(int destInd, int srcInd, int count, ushort[,] dest, ushort[,] src)
        {
            for(int i = 0; i < count; ++i)
            {
                SimHeat_CopyPosition(destInd + i, srcInd + i, dest, src);
            }
        }

        public void SimHeat() {
            int x, y;
            int a = 0;
            int src, dst;
            short fl = (short)HeatFlow;

            const int SRCCOL = Constants.WorldHeight + 2;
            const int DSTCOL = Constants.WorldHeight;

            //cellSrc, cellDst, src, dst, - are all pointers to arrays;
            //In c# they will just be indexes to positions in said array
            CellSrc = 0;
            CellDest = 0;

            CellMap = new ushort[Constants.WorldWidth + 2, Constants.WorldHeight * 2];

            /** - See above initialization
            if (cellSrc == 0)
            {
                cellSrc = (short*)newPtr((WORLD_W + 2) * (WORLD_H + 2) * sizeof(short));
                cellDst = (short*)&map[0][0];
            }**/

            src = CellSrc + SRCCOL + 1;
            dst = CellSrc;

            switch (HeatWrap)
            {
                case 0:
                    break;
                case 1:
                    //Copy one map onto the other from the starting index.
                    //Copy a row at a time
                    for (x = 0; x < Constants.WorldWidth; x++)
                    {
                        SimHeat_MemCopy(src, dst, Constants.WorldHeight, CellMap, Map);
                        //memcpy(src, dst, Constants.WorldHeight * sizeof(short));
                        src += SRCCOL;
                        dst += DSTCOL;
                    }
                    break;
                case 2:
                    for (x = 0; x < Constants.WorldWidth; x++)
                    {
                        SimHeat_CopyPosition(src - 1, src + (Constants.WorldHeight - 1), CellMap, CellMap);
                        //src[-1] = src[Constants.WorldHeight - 1];
                        SimHeat_CopyPosition(src + Constants.WorldHeight, src, CellMap, CellMap);
                        //src[Constants.WorldHeight] = src[0];
                        src += SRCCOL;
                        dst += DSTCOL;
                    }
                    SimHeat_MemCopy(CellSrc, CellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc, cellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL * sizeof(short));
                    SimHeat_MemCopy(CellSrc + SRCCOL * (Constants.WorldWidth + 1), CellSrc + SRCCOL, SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc + SRCCOL * (Constants.WorldWidth + 1), cellSrc + SRCCOL, SRCCOL * sizeof(short));
                    break;
                case 3:
                    for (x = 0; x < Constants.WorldWidth; x++)
                    {
                        SimHeat_MemCopy(src, dst, Constants.WorldHeight, CellMap, Map);
                        //memcpy(src, dst, Constants.WorldHeight * sizeof(short));
                        SimHeat_CopyPosition(src - 1, src + (Constants.WorldHeight - 1), CellMap, CellMap);
                        //src[-1] = src[Constants.WorldHeight - 1];
                        SimHeat_CopyPosition(src + Constants.WorldHeight, src, CellMap, CellMap);
                        //src[Constants.WorldHeight] = src[0];
                        src += SRCCOL;
                        dst += DSTCOL;
                    }
                    SimHeat_MemCopy(CellSrc, CellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc, cellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL * sizeof(short));
                    SimHeat_MemCopy(CellSrc + SRCCOL * (Constants.WorldWidth + 1), CellSrc + SRCCOL, SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc + SRCCOL * (Constants.WorldWidth + 1), cellSrc + SRCCOL, SRCCOL * sizeof(short));
                    break;
                case 4:
                    SimHeat_CopyPosition(src, dst, CellMap, Map);
                    //src[0] = dst[0];
                    SimHeat_CopyPosition(src + (1 + Constants.WorldHeight), dst + (1 - Constants.WorldHeight), CellMap, Map);
                    //src[1 + WORLD_H] = dst[WORLD_H - 1];
                    SimHeat_CopyPosition(src + ((1 + Constants.WorldWidth) * SRCCOL), dst + ((Constants.WorldWidth - 1) * DSTCOL), CellMap, Map);
                    //src[(1 + Constants.WorldWidth) * SRCCOL] = dst[(Constants.WorldWidth - 1) * DSTCOL];
                    SimHeat_CopyPosition(src + (((2 + Constants.WorldWidth) * SRCCOL) - 1), dst + ((Constants.WorldWidth * Constants.WorldHeight) - 1), CellMap, Map);
                    //src[((2 + Constants.WorldWidth) * SRCCOL) - 1] = dst[(Constants.WorldWidth * Constants.WorldHeight) - 1];

                    for (x = 0; x < Constants.WorldWidth; x++)
                    {
                        SimHeat_MemCopy(src, dst, Constants.WorldHeight, CellMap, Map);
                        //memcpy(src, dst, Constants.WorldHeight * sizeof(short));
                        SimHeat_CopyPosition(src - 1, src, CellMap, CellMap);
                        //src[-1] = src[0];
                        SimHeat_CopyPosition(src + Constants.WorldHeight, src+(Constants.WorldHeight - 1), CellMap, CellMap);
                        //src[WORLD_H] = src[Constants.WorldHeight - 1];
                        src += SRCCOL;
                        dst += DSTCOL;
                    }
                    SimHeat_MemCopy(CellSrc + (SRCCOL * (Constants.WorldWidth + 1)), CellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc + (SRCCOL * (Constants.WorldWidth + 1)), cellSrc + (SRCCOL * Constants.WorldWidth), SRCCOL * sizeof(short));
                    SimHeat_MemCopy(CellSrc, CellSrc + SRCCOL, SRCCOL, CellMap, CellMap);
                    //memcpy(cellSrc, cellSrc + SRCCOL, SRCCOL * sizeof(short));

                    break;
                default:
                    //not_reached(307, "..\\src\\main.cpp");
                    break;
            }

            switch (HeatRule)
            {

                case 0:
                    src = CellSrc;
                    dst = CellDest;
                    for (x = 0; x < Constants.WorldWidth;)
                    {
                        short nw, n, ne, w, c, e, sw, s, se;
                        src = CellSrc + (x * SRCCOL);
                        dst = CellDest + (x * DSTCOL);

                        w = (short)SimHeat_GetValue(CellMap, src);
                        //w = src[0];
                        c = (short)SimHeat_GetValue(CellMap, src + SRCCOL);
                        //c = src[SRCCOL];
                        e = (short)SimHeat_GetValue(CellMap, src + (2 * SRCCOL));
                        //e = src[2 * SRCCOL];
                        sw = (short)SimHeat_GetValue(CellMap, src + 1);
                        //sw = src[1];
                        s = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 1));
                        //s = src[SRCCOL + 1];
                        se = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 1));
                        //se = src[(2 * SRCCOL) + 1];

                        for (y = 0; y < Constants.WorldHeight; y++)
                        {
                            nw = w;
                            w = sw;
                            sw = (short)SimHeat_GetValue(CellMap, src + 2);
                            //sw = src[2];
                            n = c;
                            c = s;
                            s = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 2));
                            //s = src[SRCCOL + 2];
                            ne = e;
                            e = se;
                            se = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 2));
                            //se = src[(2 * SRCCOL) + 2];
                            {
                                a += nw + n + ne + w + e + sw + s + se + fl;
                                SimHeat_SetValue(Map, dst, (ushort)(((a >> 3) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                                //dst[0] = ((a >> 3) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                                a &= 7;
                            }
                            src++;
                            dst++;
                        }
                        x++;
                        src = CellSrc + ((x + 1) * SRCCOL) - 3;
                        dst = CellDest + ((x + 1) * DSTCOL) - 1;
                        nw = (short)SimHeat_GetValue(CellMap, src + 1);
                        //nw = src[1]; 
                        n = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 1));
                        //n = src[SRCCOL + 1];
                        ne = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 1));
                        //ne = src[(2 * SRCCOL) + 1];
                        w = (short)SimHeat_GetValue(CellMap, src + 2);
                        //w = src[2]; 
                        c = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 2));
                        //c = src[SRCCOL + 2];
                        e = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 2));
                        //e = src[(2 * SRCCOL) + 2];
                        for (y = Constants.WorldHeight - 1; y >= 0; y--)
                        {
                            sw = w;
                            w = nw;
                            nw = (short)SimHeat_GetValue(CellMap, src);
                            //nw = src[0];
                            s = c;
                            c = n;
                            n = (short)SimHeat_GetValue(CellMap, src + (SRCCOL));
                            //n = src[SRCCOL];
                            se = e;
                            e = ne;
                            ne = (short)SimHeat_GetValue(CellMap, src + (2 * SRCCOL));
                            //ne = src[2 * SRCCOL];
                            {
                                a += nw + n + ne + w + e + sw + s + se + fl;
                                SimHeat_SetValue(Map, dst, (ushort)(((a >> 3) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                                //dst[0] = ((a >> 3) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                                a &= 7;
                            }
                            src--;
                            dst--;
                        }
                        x++;
                    };
                    break;

                case 1:

                    src = CellSrc;
                    dst = CellDest;
                    for (x = 0; x < Constants.WorldWidth;)
                    {
                        short nw, n, ne, w, c, e, sw, s, se;

                        src = CellSrc + (x * SRCCOL);
                        dst = CellDest + (x * DSTCOL);
                        w = (short)SimHeat_GetValue(CellMap, src);
                        //w = src[0];
                        c = (short)SimHeat_GetValue(CellMap, src + SRCCOL);
                        //c = src[SRCCOL];
                        e = (short)SimHeat_GetValue(CellMap, src + (2 * SRCCOL));
                        //e = src[2 * SRCCOL];
                        sw = (short)SimHeat_GetValue(CellMap, src + 1);
                        //sw = src[1];
                        s = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 1));
                        //s = src[SRCCOL + 1];
                        se = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 1));
                        //se = src[(2 * SRCCOL) + 1];

                        for (y = 0; y < Constants.WorldHeight; y++)
                        {
                            nw = w;
                            w = sw;
                            sw = (short)SimHeat_GetValue(CellMap, src + 2);
                            //sw = src[2];
                            n = c;
                            c = s;
                            s = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 2));
                            //s = src[SRCCOL + 2];
                            ne = e;
                            e = se;
                            se = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 2));
                            //se = src[(2 * SRCCOL) + 2];
                            {
                                {
                                    c -= fl;
                                    n -= fl;
                                    s -= fl;
                                    e -= fl;
                                    w -= fl;
                                    ne -= fl;
                                    nw -= fl;
                                    se -= fl;
                                    sw -= fl;

                                    int sum = (c & 1) + (n & 1) + (s & 1) + (e & 1) + (w & 1) + (ne & 1) + (nw & 1) + (se & 1) + (sw & 1), cell;

                                    if (((sum > 5) || (sum == 4)))
                                    {
                                        cell = ((c << 1) & (0x3fc)) | (((((c >> 1) & 3) == 0) && (((n & 2) + (s & 2) + (e & 2) + (w & 2) + (ne & 2) + (nw & 2) + (se & 2) + (sw & 2)) == (2 << 1))) ? 2 : 0) | 1;
                                    }
                                    else
                                    {
                                        sum = ((n & 2) + (s & 2) + (e & 2) + (w & 2) + (ne & 2) + (nw & 2) + (se & 2) + (sw & 2)) >> 1;
                                        cell = (((c ^ 2) << 1) & 0x3fc) | ((c & 2).IsTrue() ? ((sum != 5) ? 2 : 0) : (((sum != 5) && (sum != 6)) ? 2 : 0));
                                    }

                                    SimHeat_SetValue(Map, dst, (ushort)(((fl + cell) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                                    //dst[0] = ((fl + cell) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                                    c += fl;
                                    n += fl;
                                    s += fl;
                                    e += fl;
                                    w += fl;
                                    ne += fl;
                                    nw += fl;
                                    se += fl;
                                    sw += fl;
                                }
                            }
                            src++;
                            dst++;
                        }
                        x++;

                        src = CellSrc + ((x + 1) * SRCCOL) - 3;
                        dst = CellDest + ((x + 1) * DSTCOL) - 1;
                        nw = (short)SimHeat_GetValue(CellMap, src + 1);
                        //nw = src[1];
                        n = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 1));
                        //n = src[SRCCOL + 1];
                        ne = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 1));
                        //ne = src[(2 * SRCCOL) + 1];
                        w = (short)SimHeat_GetValue(CellMap, src + 2);
                        //w = src[2];
                        c = (short)SimHeat_GetValue(CellMap, src + (SRCCOL + 2));
                        //c = src[SRCCOL + 2];
                        e = (short)SimHeat_GetValue(CellMap, src + ((2 * SRCCOL) + 2));
                        //e = src[(2 * SRCCOL) + 2];

                        for (y = Constants.WorldHeight - 1; y >= 0; y--)
                        {
                            sw = w;
                            w = nw;
                            //nw = src[0];
                            s = c;
                            c = n;
                            //n = src[SRCCOL];
                            se = e;
                            e = ne;
                            //ne = src[2 * SRCCOL];
                            {
                                {
                                    c -= fl;
                                    n -= fl;
                                    s -= fl;
                                    e -= fl;
                                    w -= fl;
                                    ne -= fl;
                                    nw -= fl;
                                    se -= fl;
                                    sw -= fl;

                                    int sum = (c & 1) + (n & 1) + (s & 1) + (e & 1) + (w & 1) + (ne & 1) + (nw & 1) + (se & 1) + (sw & 1), cell;

                                    if (((sum > 5) || (sum == 4)))
                                    {
                                        cell = ((c << 1) & (0x3fc)) | (((((c >> 1) & 3) == 0) && (((n & 2) + (s & 2) + (e & 2) + (w & 2) + (ne & 2) + (nw & 2) + (se & 2) + (sw & 2)) == (2 << 1))) ? 2 : 0) | 1;
                                    }
                                    else
                                    {
                                        sum = ((n & 2) + (s & 2) + (e & 2) + (w & 2) + (ne & 2) + (nw & 2) + (se & 2) + (sw & 2)) >> 1;
                                        cell = (((c ^ 2) << 1) & 0x3fc) | ((c & 2).IsTrue() ? ((sum != 5) ? 2 : 0) : (((sum != 5) && (sum != 6)) ? 2 : 0));
                                    }

                                    SimHeat_SetValue(Map, dst, (ushort)(((fl + cell) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                                    //dst[0] = ((fl + cell) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                                    c += fl;
                                    n += fl;
                                    s += fl;
                                    e += fl;
                                    w += fl;
                                    ne += fl;
                                    nw += fl;
                                    se += fl;
                                    sw += fl;
                                }
                            }
                            src--;
                            dst--;
                        }
                        x++;
                    };

                    break;

                default:
                    //not_reached(397, "..\\src\\main.cpp");
                    break;
            }
        }

        public void SimLoop(bool doSim)
        {
            if (HeatSteps.IsTrue())
            {
                int j;

                for (j = 0; j < HeatSteps; j++)
                {
                    SimHeat();
                }

                MoveObjects();
                SimRobots();

                NewMap = 1;

            }
            else
            {
                if (doSim)
                {
                    SimFrame();
                }

                MoveObjects();
                SimRobots();
            }

            SimLoops++;
        }

        public void SimTick()
        {
            if (SimSpeed.IsTrue())
            {
                for (SimPass = 0; SimPass < SimPasses; SimPass++)
                {
                    SimLoop(true);
                }
            }
            SimUpdate();
        }

        public void SimRobots()
        {
            Callback("simRobots", "");
        }
    }
}
