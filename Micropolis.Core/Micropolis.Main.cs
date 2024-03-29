﻿/* Micropolis.Main.cs
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

using System;
using System.Collections.Generic;
using System.IO;
using MicropolisSharp.Types;

namespace MicropolisSharp;

/// <summary>
///     Partial Class Containing the content of main.cpp
/// </summary>
public partial class Micropolis
{
    /// <summary>
    ///     ???
    /// </summary>
    public int SimLoops { get; private set; }

    /// <summary>
    ///     The count of the current pass through the simulator loop.
    /// </summary>
    public int SimPass { get; private set; }

    /// <summary>
    ///     Is the simulation paused?
    ///     TODO: Variable has reversed logic, maybe rename to sim running?
    /// </summary>
    public bool SimPaused { get; private set; }

    /// <summary>
    ///     Simulator Paused Speed
    ///     TODO: Again too, Variable has reversed logic
    /// </summary>
    public int SimPausedSpeed { get; private set; }

    /// <summary>
    ///     The number of passes through the simulator, loop to take each tick
    /// </summary>
    public int SimPasses { get; private set; }

    /// <summary>
    ///     TODO: Currently not used - should it be hooked up?
    /// </summary>
    public bool TilesAnimated { get; private set; }

    /// <summary>
    ///     TODO: Currently not used - should it be hooked up?
    /// </summary>
    public bool DoMessages { get; private set; }

    /// <summary>
    ///     Enable Animation - currently always true
    ///     TODO: Currently not used - should it be hooked up?
    /// </summary>
    public bool DoAnimation { get; private set; }

    /// <summary>
    ///     TODO: Currently not used - should it be hooked up?
    /// </summary>
    public bool DoNotices { get; private set; }

    /// <summary>
    ///     Filename of last city loaded
    /// </summary>
    public string CityFileName { get; private set; }

    /// <summary>
    ///     Name of the City
    /// </summary>
    public string CityName { get; private set; }

    /// <summary>
    ///     ????
    /// </summary>
    public int HeatSteps { get; private set; }

    /// <summary>
    ///     TODO: Always -7 - make constant
    /// </summary>
    [Obsolete]
    public int HeatFlow { get; private set; }

    /// <summary>
    ///     Get version of Micropolis program.
    ///     TODO: Use this function or Eliminate it
    /// </summary>
    /// <returns>Textual version</returns>
    public string GetMicropolisVersion()
    {
        return "5.0";
    }

    /// <summary>
    ///     Check whether \a dir points to a directory.
    ///     If not report an error
    /// </summary>
    /// <param name="dir">Directory to search.</param>
    /// <returns>Directory has been found</returns>
    public static bool TestDirectory(string dir)
    {
        return Directory.Exists(dir);
    }

    /// <summary>
    ///     Locate resource directory.
    /// </summary>
    public void EnvironmentInit()
    {
        var simHome = Environment.GetEnvironmentVariable("SIMHOME");
        if (string.IsNullOrWhiteSpace(simHome)) simHome = Directory.GetCurrentDirectory();

        if (TestDirectory(simHome))
        {
            ResourceDir = simHome + Path.PathSeparator + "res" + Path.PathSeparator;
            if (TestDirectory(ResourceDir)) return;
        }

        //TODO: Exception - cannot find res dir
    }

    /// <summary>
    ///     Initialize for a simulation
    /// </summary>
    public void SimInit()
    {
        SpriteList = new List<SimSprite>();

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

    /// <summary>
    ///     Update ??
    ///     TODO: WHat is the purpose of this function (along with, SimTick)
    /// </summary>
    public void SimUpdate()
    {
        //printf("simUpdate\n");
        BlinkFlag = (short)(TickCount() % 60 < 30 ? 1 : -1);

        if (SimSpeed.IsTrue() && HeatSteps.IsFalse()) TilesAnimated = false;

        DoUpdateHeads();
        GraphDoer();
        UpdateBudget();
        ScoreDoer();
    }

    private ushort SimHeat_GetValue(ushort[,] map, int pos)
    {
        var posX = pos / map.GetLength(1);
        var posY = pos - map.GetLength(1) * posX;
        return map[posX, posY];
    }

    private void SimHeat_SetValue(ushort[,] map, int pos, ushort value)
    {
        var posX = pos / map.GetLength(1);
        var posY = pos - map.GetLength(1) * posX;
        map[posX, posY] = value;
    }

    private void SimHeat_CopyPosition(int destInd, int srcInd, ushort[,] dest, ushort[,] src)
    {
        var srcPosX = srcInd / src.GetLength(1);
        var srcPosY = srcInd - src.GetLength(1) * srcPosX;

        var destPosX = destInd / dest.GetLength(1);
        var destPosY = destInd - dest.GetLength(1) * destPosX;

        dest[destPosX, destPosY] = src[srcPosX, srcPosY];
    }

    private void SimHeat_MemCopy(int destInd, int srcInd, int count, ushort[,] dest, ushort[,] src)
    {
        for (var i = 0; i < count; ++i) SimHeat_CopyPosition(destInd + i, srcInd + i, dest, src);
    }

    /// <summary>
    ///     ????
    ///     TODO: Why is Micropolis::cellSrc not allocated together with all the other variables
    ///     TODO: What is the purpose of this function?
    /// </summary>
    public void SimHeat()
    {
        int x, y;
        var a = 0;
        int src, dst;
        var fl = (short)HeatFlow;

        const int srccol = Constants.WorldHeight + 2;
        const int dstcol = Constants.WorldHeight;

        //cellSrc, cellDst, src, dst, - are all pointers to arrays;
        //In c# they will just be indexes to positions in said array
        const int cellSrc = 0;
        const int cellDest = 0;

        var cellMap = new ushort[Constants.WorldWidth + 2, Constants.WorldHeight * 2];

        /** - See above initialization
        if (cellSrc == 0)
        {
            cellSrc = (short*)newPtr((WORLD_W + 2) * (WORLD_H + 2) * sizeof(short));
            cellDst = (short*)&map[0][0];
        }**/

        /*
            *Copy wrapping edges:
            *
            *   0   ff f0 f1...fe ff f0
            *
            *   1   0f  00 01... 0e 0f     00
            *   2   1f  10 11... 1e 1f     10
            *       ..  .. ..     .. ..     ..
            *       ef  e0 e1 ... ee ef     e0
            *   h   ff f0 f1...fe ff f0
            *
            *   h+1 0f  00 01... 0e 0f     00
            *
            *   wrap value: effect:
            *
            *   0   no effect
            *   1   copy future=>past, no wrap
            *   2   no copy, wrap edges
            *   3   copy future=>past, wrap edges
            *   4   copy future=>past, same edges
        */

        /**
         * What does this code do? 
         * It builds up a Second Map which has wrapped edges.
         *
         * e.g. A B       ->   D C D C
         *      C D            B A B A
         *                     D C D C
         *                     B A B A
         */
        //FROM case#3 from old switch on HeatWrap --  3   copy future=>past, wrap edges
        src = cellSrc + srccol + 1;
        dst = cellSrc;

        for (x = 0; x < Constants.WorldWidth; x++)
        {
            //Copy a column from Map -> to a Offset in CellMap (Cell Map seems to have a boundary around it
            SimHeat_MemCopy(src, dst, Constants.WorldHeight, cellMap, Map);
            //Wrap the Edges
            SimHeat_CopyPosition(src - 1, src + (Constants.WorldHeight - 1), cellMap, cellMap);
            SimHeat_CopyPosition(src + Constants.WorldHeight, src, cellMap, cellMap);
            src += srccol;
            dst += dstcol;
        }

        //Copy the map from CellMap into CellMap again (so its there twice - wrapped)
        SimHeat_MemCopy(cellSrc, cellSrc + srccol * Constants.WorldWidth, srccol, cellMap, cellMap);
        //Wrap Edges
        SimHeat_MemCopy(cellSrc + srccol * (Constants.WorldWidth + 1), cellSrc + srccol, srccol, cellMap, cellMap);
        //END FROM case#3 from old switch on HeatWrap

        /**
         * What does this code do? 
         * Reads a Square Square out of the Map - Applies a mutuation to it - Square Application Order Matters!
         *
         * Step 1 -> Read Square with Center Index @ 0 -> MutationFunction -> Set Result to Center Index @ 0
         * Step 1 -> Read Square with Center Index @ 1 -> MutationFunction -> Set Result to Center Index @ 1
         * 
         * Center Index @ 0 = if the map was represented a 1D array, then 0,0 = 0, 0,1 = 1 etc.
         * 
         * From a 2D persective, it snakes through the map for e.g
         *
         *  A B         -> A C D B is the order the Squares are processed in
         *  C D
         *
         *  The Mutation Function = 
         *  
         *  res = (lastRes & 7) + (sum(Square) + 7) >> 3
         *
         *  The 7 = the heat flow
         *  >> 3 is the same as divided by 8
         *  This seems to operate on the whole map value -> how does it not change the terrain?
         *
         */

        //FROM case#0 from old switch on HeatRule
        src = cellSrc;
        dst = cellDest;
        for (x = 0; x < Constants.WorldWidth;)
        {
            short nw, n, ne, w, c, e, sw, s, se;
            src = cellSrc + x * srccol;
            dst = cellDest + x * dstcol;

            w = (short)SimHeat_GetValue(cellMap, src);
            //w = src[0];
            c = (short)SimHeat_GetValue(cellMap, src + srccol);
            //c = src[SRCCOL];
            e = (short)SimHeat_GetValue(cellMap, src + 2 * srccol);
            //e = src[2 * SRCCOL];
            sw = (short)SimHeat_GetValue(cellMap, src + 1);
            //sw = src[1];
            s = (short)SimHeat_GetValue(cellMap, src + srccol + 1);
            //s = src[SRCCOL + 1];
            se = (short)SimHeat_GetValue(cellMap, src + 2 * srccol + 1);
            //se = src[(2 * SRCCOL) + 1];

            for (y = 0; y < Constants.WorldHeight; y++)
            {
                nw = w;
                w = sw;
                sw = (short)SimHeat_GetValue(cellMap, src + 2);
                //sw = src[2];
                n = c;
                c = s;
                s = (short)SimHeat_GetValue(cellMap, src + srccol + 2);
                //s = src[SRCCOL + 2];
                ne = e;
                e = se;
                se = (short)SimHeat_GetValue(cellMap, src + 2 * srccol + 2);
                //se = src[(2 * SRCCOL) + 2];
                {
                    a += nw + n + ne + w + e + sw + s + se + fl;
                    SimHeat_SetValue(Map, dst,
                        (ushort)(((a >> 3) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated |
                                 (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                    //dst[0] = ((a >> 3) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                    a &= 7;
                }
                src++;
                dst++;
            }

            x++;
            src = cellSrc + (x + 1) * srccol - 3;
            dst = cellDest + (x + 1) * dstcol - 1;
            nw = (short)SimHeat_GetValue(cellMap, src + 1);
            //nw = src[1]; 
            n = (short)SimHeat_GetValue(cellMap, src + srccol + 1);
            //n = src[SRCCOL + 1];
            ne = (short)SimHeat_GetValue(cellMap, src + 2 * srccol + 1);
            //ne = src[(2 * SRCCOL) + 1];
            w = (short)SimHeat_GetValue(cellMap, src + 2);
            //w = src[2]; 
            c = (short)SimHeat_GetValue(cellMap, src + srccol + 2);
            //c = src[SRCCOL + 2];
            e = (short)SimHeat_GetValue(cellMap, src + 2 * srccol + 2);
            //e = src[(2 * SRCCOL) + 2];
            for (y = Constants.WorldHeight - 1; y >= 0; y--)
            {
                sw = w;
                w = nw;
                nw = (short)SimHeat_GetValue(cellMap, src);
                //nw = src[0];
                s = c;
                c = n;
                n = (short)SimHeat_GetValue(cellMap, src + srccol);
                //n = src[SRCCOL];
                se = e;
                e = ne;
                ne = (short)SimHeat_GetValue(cellMap, src + 2 * srccol);
                //ne = src[2 * SRCCOL];
                {
                    a += nw + n + ne + w + e + sw + s + se + fl;
                    SimHeat_SetValue(Map, dst,
                        (ushort)(((a >> 3) & (ushort)MapTileBits.LowMask) | (ushort)MapTileBits.Animated |
                                 (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable));
                    //dst[0] = ((a >> 3) & LOMASK) | ANIMBIT | BURNBIT | BULLBIT;
                    a &= 7;
                }
                src--;
                dst--;
            }

            x++;
        }

        ;
        //END FROM case#0 from old switch on HeatRule
    }

    /// <summary>
    /// </summary>
    /// <param name="doSim"></param>
    public void SimLoop(bool doSim)
    {
        if (HeatSteps.IsTrue())
        {
            int j;

            for (j = 0; j < HeatSteps; j++) SimHeat();

            MoveObjects();
            SimRobots();

            NewMap = 1;
        }
        else
        {
            if (doSim) SimFrame();

            MoveObjects();
            SimRobots();
        }

        SimLoops++;
    }

    /// <summary>
    ///     Move simulation forward
    ///     TODO: What is the purpose of this function? (along side SimUpdate)
    /// </summary>
    public void SimTick()
    {
        if (SimSpeed.IsTrue())
            for (SimPass = 0; SimPass < SimPasses; SimPass++)
                SimLoop(true);
        SimUpdate();
    }

    public void SimRobots()
    {
        Callback("simRobots", "");
    }
}