/* Micropolis.Zone.cs
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
    /// Partial Class Containing the content of zone.cpp
    /// </summary>
    public partial class Micropolis
    {
        /// <summary>
        /// Handle Zone
        /// </summary>
        /// <param name="pos">Position of the zone.</param>
        public void DoZone(Position pos)
        {  
            // Set Power Bit in Map from powerGridMap
            bool zonePowerFlag = SetZonePower(pos);


            if (zonePowerFlag)
            {
                PoweredZoneCount++;
            }
            else
            {
                UnpoweredZoneCount++;
            }

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            // Do special zones.
            if ((tile > (ushort)MapTileCharacters.PORTBASE) &&
                (tile < (ushort)MapTileCharacters.CHURCH1BASE))
            {
                DoSpecialZone(pos, zonePowerFlag);
                return;
            }

            // Do residential zones.
            if (tile < (ushort)MapTileCharacters.HOSPITAL)
            {
                DoResidential(pos, zonePowerFlag);
                return;
            }

            // Do hospitals and churches.
            if ((tile < (ushort)MapTileCharacters.COMBASE) ||
                ((tile >= (ushort)MapTileCharacters.CHURCH1BASE) &&
                 (tile <= (ushort)MapTileCharacters.CHURCH7LAST)))
            {
                DoHospitalChurch(pos);
                return;
            }

            // Do commercial zones.
            if (tile < (ushort)MapTileCharacters.INDBASE)
            {
                DoCommercial(pos, zonePowerFlag);
                return;
            }

            // Do industrial zones.
            if (tile < (ushort)MapTileCharacters.CHURCH1BASE)
            {
                DoIndustrial(pos, zonePowerFlag);
                return;
            }
        }

        /// <summary>
        /// Handle repairing or removing of hospitals and churches.
        /// </summary>
        /// <param name="pos"></param>
        public void DoHospitalChurch(Position pos) {
            ushort tile = (ushort)(Map[pos.X, pos.Y] & (ushort)MapTileBits.LowMask);

            if (tile == (ushort)MapTileCharacters.HOSPITAL)
            {

                HospitalPop++;

                if ((CityTime & 15).IsFalse())
                {
                    RepairZone(pos, (ushort)MapTileCharacters.HOSPITAL, 3);
                }

                if (NeedHospital == -1)
                { // Too many hospitals!
                    if (GetRandom(20) == 0)
                    {
                        ZonePlop(pos, (ushort)MapTileCharacters.RESBASE); // Remove hospital.
                    }
                }

            }
            else if ((tile == (ushort)MapTileCharacters.CHURCH0) ||
                 (tile == (ushort)MapTileCharacters.CHURCH1) ||
                 (tile == (ushort)MapTileCharacters.CHURCH2) ||
                 (tile == (ushort)MapTileCharacters.CHURCH3) ||
                 (tile == (ushort)MapTileCharacters.CHURCH4) ||
                 (tile == (ushort)MapTileCharacters.CHURCH5) ||
                 (tile == (ushort)MapTileCharacters.CHURCH6) ||
                 (tile == (ushort)MapTileCharacters.CHURCH7))
            {

                ChurchPop++;

                //printf("CHURCH %d %d %d %d\n", churchPop, pos.posX, pos.posY, tile);

                bool simulate = true;

                if ((CityTime & 15).IsFalse())
                {
                    RepairZone(pos, tile, 3);
                }

                if (NeedChurch == -1)
                { // Too many churches!
                    if (GetRandom(20) == 0)
                    {
                        ZonePlop(pos, (ushort)MapTileCharacters.RESBASE); // Remove church.
                        simulate = false;
                    }
                }

                if (simulate)
                {
                    //printf("SIM %d %d %d\n", pos.posX, pos.posY, tile);

                    int churchNumber = 0;

                    switch (tile)
                    {
                        case (ushort)MapTileCharacters.CHURCH0:
                            churchNumber = 0;
                            break;
                        case (ushort)MapTileCharacters.CHURCH1:
                            churchNumber = 1;
                            break;
                        case (ushort)MapTileCharacters.CHURCH2:
                            churchNumber = 2;
                            break;
                        case (ushort)MapTileCharacters.CHURCH3:
                            churchNumber = 3;
                            break;
                        case (ushort)MapTileCharacters.CHURCH4:
                            churchNumber = 4;
                            break;
                        case (ushort)MapTileCharacters.CHURCH5:
                            churchNumber = 5;
                            break;
                        case (ushort)MapTileCharacters.CHURCH6:
                            churchNumber = 6;
                            break;
                        case (ushort)MapTileCharacters.CHURCH7:
                            churchNumber = 7;
                            break;
                        default:
                            //TODO: Reenable Asserts
                            //assert(0); // Unexpected church tile
                            break;
                    }

                    Callback("simulateChurch", "ddd", pos.X.ToString(), pos.Y.ToString(), churchNumber.ToString());
                }

            }
        }

        public void SetSmoke(Position pos, bool zonePower)
        {
            ushort ASCBIT = ((ushort)MapTileBits.Animated | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable);
            ushort REGBIT = ((ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable);

            bool[] aniThis = { true, false, true, true, false, false, true, true };
            short[] dx1 = { -1, 0, 1, 0, 0, 0, 0, 1 };
            short[] dy1 = { -1, 0, -1, -1, 0, 0, -1, -1 };
            ushort[] aniTabA = { 0, 0, 32, 40, 0, 0, 48, 56 };
            ushort[] aniTabB = { 0, 0, 36, 44, 0, 0, 52, 60 };
            ushort[] aniTabC = { (ushort)MapTileCharacters.IND1, 0, (ushort)MapTileCharacters.IND2, (ushort)MapTileCharacters.IND4, 0, 0, (ushort)MapTileCharacters.IND6, (ushort)MapTileCharacters.IND8 };
            ushort[] aniTabD = { (ushort)MapTileCharacters.IND1, 0, (ushort)MapTileCharacters.IND3, (ushort)MapTileCharacters.IND5, 0, 0, (ushort)MapTileCharacters.IND7, (ushort)MapTileCharacters.IND9 };

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            if (tile < (short)MapTileCharacters.IZB)
            {
                return;
            }

            int z = (tile - (short)MapTileCharacters.IZB) >> 3; /// @todo Why div 8? Industry is 9 tiles long!!
            z = z & 7;

            if (aniThis[z])
            {
                int xx = pos.X + dx1[z];
                int yy = pos.Y + dy1[z];

                if (Position.TestBounds(xx, yy))
                {

                    if (zonePower)
                    {

                        /// @todo Why do we assign the same map position twice?
                        /// @todo Add #SMOKEBASE into aniTabA and aniTabB tables?
                        if ((ushort)(Map[pos.X, pos.Y] & (ushort)MapTileBits.LowMask) == aniTabC[z])
                        {
                            Map[xx,yy] = (ushort)(ASCBIT | ((short)MapTileCharacters.SMOKEBASE + aniTabA[z]));
                            Map[xx,yy] = (ushort)(ASCBIT | ((short)MapTileCharacters.SMOKEBASE + aniTabB[z]));
                        }

                    }
                    else
                    {

                        /// @todo Why do we assign the same map position twice?
                        if ((ushort)(Map[pos.X, pos.Y] & (ushort)MapTileBits.LowMask) == aniTabC[z])
                        {
                            Map[xx,yy] = (ushort)(REGBIT | aniTabC[z]);
                            Map[xx,yy] = (ushort)(REGBIT | aniTabD[z]);
                        }

                    }

                }

            }
        }     
                
        /// <summary>
        /// If needed, add a new hospital or a new church.
        /// </summary>
        /// <param name="pos"></param>
        public void MakeHospital(Position pos)
        {
            if (NeedHospital > 0)
            {
                ZonePlop(pos, (ushort)MapTileCharacters.HOSPITAL - 4);
                NeedHospital = 0;
                return;
            }

            if (NeedChurch > 0)
            {
                int churchType = GetRandom(7); // 0 to 7 inclusive
                int tile;
                if (churchType == 0)
                {
                    tile = (ushort)MapTileCharacters.CHURCH0;
                }
                else
                {
                    tile = (ushort)MapTileCharacters.CHURCH1 + ((churchType - 1) * 9);
                }

                //printf("NEW CHURCH tile %d x %d y %d type %d\n", tile, pos.posX, pos.posY, churchType);

                ZonePlop(pos, tile - 4);
                NeedChurch = 0;
                return;
            }
        }

        /// <summary>
        /// Compute land value at \a pos, taking pollution into account.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Indication of land-value adjusted for pollution (\c 0 => low value, \c 3 => high value)</returns>
        public short GetLandPollutionValue(Position pos)
        {
            short landVal;

            landVal = LandValueMap.WorldGet(pos.X, pos.Y);
            landVal -= PollutionDensityMap.WorldGet(pos.X, pos.Y);

            if (landVal < 30)
            {
                return 0;
            }

            if (landVal < 80)
            {
                return 1;
            }

            if (landVal < 150)
            {
                return 2;
            }

            return 3;
        }

        /// <summary>
        /// Update the rate of growth at position \a pos by \a amount.
        /// </summary>
        /// <param name="pos">Position to modify.</param>
        /// <param name="amount"> Amount of change (can both be positive and negative).</param>
        public void IncRateOfGrowth(Position pos, int amount)
        {
            int value = RateOfGrowthMap.WorldGet(pos.X, pos.Y);

            value = Utilities.Restrict(value + amount * 4, -200, 200);
            RateOfGrowthMap.WorldSet(pos.X, pos.Y, (short)value);
        }

        /// <summary>
        /// Put down a 3x3 zone around the center tile at \a pos..
        /// 
        /// TODO: This function allows partial on-map construction. Is that intentional? No!
        /// </summary>
        /// <param name="pos">Tile number of the top-left tile. @see MapTileCharacters</param>
        /// <param name="baseTile">Build was a success.</param>
        /// <returns>Build was a success.</returns>
        public bool ZonePlop(Position pos, int baseTile)
        {
            short z;
            ushort x;
            short[] Zx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
            short[] Zy = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };

            for (z = 0; z < 9; z++)
            {             /* check for fire  */
                int xx = pos.X + Zx[z];
                int yy = pos.Y + Zy[z];

                if (Position.TestBounds(xx, yy))
                {
                    x = (ushort)(Map[xx,yy] & (ushort)MapTileBits.LowMask);

                    if ((x >= (ushort)MapTileCharacters.FLOOD) && (x < (ushort)MapTileCharacters.ROADBASE))
                    {
                        return false;
                    }

                }

            }

            for (z = 0; z < 9; z++)
            {
                int xx = pos.X + Zx[z];
                int yy = pos.Y + Zy[z];

                if (Position.TestBounds(xx, yy))
                {
                    Map[xx,yy] = (ushort)(baseTile + (ushort)MapTileBits.BurnableOrConductive);
                }

                baseTile++;
            }

            SetZonePower(pos);
            Map[pos.X,pos.Y] |= (ushort)MapTileBits.CenterOfZone + (ushort)MapTileBits.Bulldozable;

            return true;
        }

        /// <summary>
        /// Count the number of single tile houses in a residential zone.
        /// </summary>
        /// <param name="pos">Position of the residential zone.</param>
        /// <returns>Number of single tile houses.</returns>
        public short DoFreePop(Position pos)
        {
            short count = 0;

            for (int x = pos.X - 1; x <= pos.X + 1; x++)
            {
                for (int y = pos.Y - 1; y <= pos.Y + 1; y++)
                {
                    if (x >= 0 && x < Constants.WorldWidth && y >= 0 && y < Constants.WorldHeight)
                    {
                        ushort tile = (ushort)(Map[x,y] & (ushort)MapTileBits.LowMask);
                        if (tile >= (ushort)MapTileCharacters.LHTHR && tile <= (ushort)MapTileCharacters.HHTHR)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Copy the value of #powerGridMap at position \a pos to the map.
        /// </summary>
        /// <param name="pos">Position to copy.</param>
        /// <returns>Does the tile have power?</returns>
        public bool SetZonePower(Position pos)
        {
            ushort mapValue = Map[pos.X,pos.Y];
            ushort tile = (ushort)(mapValue & (ushort)MapTileBits.LowMask);

            if (tile == (ushort)MapTileCharacters.NUCLEAR || tile == (ushort)MapTileCharacters.POWERPLANT)
            {
                Map[pos.X,pos.Y] = (ushort)(mapValue | (ushort)MapTileBits.Power);
                return true;
            }

            if (PowerGridMap.WorldGet(pos.X, pos.Y) > 0)
            {
                Map[pos.X,pos.Y] = (ushort)(mapValue | (ushort)MapTileBits.Power);
                return true;
            }
            else
            {
                Map[pos.X,pos.Y] = (ushort)(mapValue & (~(ushort)MapTileBits.Power));
                return false;
            }
        }

        /// <summary>
        /// Try to build a house at the zone at \a pos.
        /// 
        /// TODO: Have some form of looking around the center tile (like getFromMap())
        /// </summary>
        /// <param name="pos">Center tile of the zone.</param>
        /// <param name="value">Value to build (land value?)</param>
        public void BuildHouse(Position pos, int value)
        {
            short z, score, hscore, BestLoc;
            short[] ZeX = { 0, -1, 0, 1, -1, 1, -1, 0, 1 };
            short[] ZeY = { 0, -1, -1, -1, 0, 0, 1, 1, 1 };

            BestLoc = 0;
            hscore = 0;

            for (z = 1; z < 9; z++)
            {
                int xx = pos.X + ZeX[z];
                int yy = pos.Y + ZeY[z];

                if (Position.TestBounds(xx, yy))
                {

                    score = EvalLot(xx, yy);

                    /// @bug score is never 0 !!
                    if (score != 0)
                    {

                        if (score > hscore)
                        {
                            hscore = score;
                            BestLoc = z;
                        }

                        /// @todo Move the code below to a better place.
                        ///       If we just updated hscore above, we could
                        //        trigger this code too.
                        if (score == hscore && (GetRandom16() & 7).IsFalse())
                        {
                            BestLoc = z;
                        }

                    }

                }

            }

            if (BestLoc > 0)
            {
                int xx = pos.X + ZeX[BestLoc];
                int yy = pos.Y + ZeY[BestLoc];

                if (Position.TestBounds(xx, yy))
                {
                    /// @todo Is HOUSE the proper constant here?
                    Map[xx,yy] = (ushort)((ushort)MapTileCharacters.HOUSE + (ushort)MapTileBits.BurnableOrBulldozableOrConductive + GetRandom(2) + value * 3);
                }

            }
        }

        /// <summary>
        /// Evaluate suitability of the position for placing a new house.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Suitability.</returns>
        public short EvalLot(int x, int y)
        {
            ushort z;
            short score;
            short[] DX = { 0, 1, 0, -1 };
            short[] DY = { -1, 0, 1, 0 };

            /* test for clear lot */
            z = (ushort)(Map[x,y] & (ushort)MapTileBits.LowMask);

            if (z > 0 && (z < (ushort)MapTileCharacters.RESBASE || z > (ushort)MapTileCharacters.RESBASE + 8))
            {
                return -1;
            }

            score = 1;

            for (z = 0; z < 4; z++)
            {
                int xx = x + DX[z];
                int yy = y + DY[z];

                if (Position.TestBounds(xx, yy) &&
                    Map[xx,yy] != (ushort)MapTileCharacters.DIRT && (Map[xx,yy] & (ushort)MapTileBits.LowMask) <= (ushort)MapTileCharacters.LASTROAD)
                {
                    score++;          /* look for road */
                }

            }

            return score;
        }

        /// <summary>
        /// Handle residential zone.
        /// </summary>
        /// <param name="pos">Center tile of the residential zone.</param>
        /// <param name="zonePower">Does the zone have power?</param>
        public void DoResidential(Position pos, bool zonePower) {
            short tpop, zscore, locvalve, value, TrfGood;

            ResZonePop++;

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            if (tile == (ushort)MapTileCharacters.FREEZ)
            {
                tpop = DoFreePop(pos);
            }
            else
            {
                tpop = GetResZonePop(tile);
            }

            ResPop += tpop;

            if (tpop > GetRandom(35))
            {
                /* Try driving from residential to commercial */
                TrfGood = MakeTraffic(pos, ZoneType.Commercial);
            }
            else
            {
                TrfGood = 1;
            }

            if (TrfGood == -1)
            {
                value = GetLandPollutionValue(pos);
                DoResOut(pos, tpop, value);
                return;
            }

            if (tile == (ushort)MapTileCharacters.FREEZ || (GetRandom16() & 7).IsFalse())
            {

                locvalve = EvalRes(pos, TrfGood);
                zscore = (short)(ResValve + locvalve);

                if (!zonePower)
                {
                    zscore = -500;
                }

                if (zscore > -350 &&
                    ((short)(zscore - 26380) > ((short)GetRandom16Signed())))
                {

                    if (tpop.IsFalse() && (GetRandom16() & 3).IsFalse())
                    {
                        MakeHospital(pos);
                        return;
                    }

                    value = GetLandPollutionValue(pos);
                    DoResIn(pos, tpop, value);
                    return;
                }

                if (zscore < 350 &&
                    (((short)(zscore + 26380)) < ((short)GetRandom16Signed())))
                {
                    value = GetLandPollutionValue(pos);
                    DoResOut(pos, tpop, value);
                }
            }
        }

        /// <summary>
        /// Perform residential immigration into the current residential tile.
        /// </summary>
        /// <param name="pos">Position of the tile.</param>
        /// <param name="pop">Population ?</param>
        /// <param name="value">Land value corrected for pollution.</param>
        public void DoResIn(Position pos, int pop, int value)
        {
            short pollution = PollutionDensityMap.WorldGet(pos.X, pos.Y);

            if (pollution > 128)
            {
                return;
            }

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            if (tile == (ushort)MapTileCharacters.FREEZ)
            {

                if (pop < 8)
                {
                    BuildHouse(pos, value);
                    IncRateOfGrowth(pos, 1);
                    return;
                }

                if (PopulationDensityMap.WorldGet(pos.X, pos.Y) > 64)
                {
                    ResPlop(pos, 0, value);
                    IncRateOfGrowth(pos, 8);
                    return;
                }

                return;
            }

            if (pop < 40)
            {
                ResPlop(pos, (pop / 8) - 1, value);
                IncRateOfGrowth(pos, 8);
            }
        }

        /// <summary>
        /// Perform residential emigration from the current residential tile.
        /// </summary>
        /// <param name="pos">Position of the tile.</param>
        /// <param name="pop">Population ?</param>
        /// <param name="value">Land value corrected for pollution.</param>
        public void DoResOut(Position pos, int pop, int value)
        {
            short[] Brdr = { 0, 3, 6, 1, 4, 7, 2, 5, 8 };
            int x, y, loc, z;

            if (pop.IsFalse())
            {
                return;
            }

            if (pop > 16)
            {
                ResPlop(pos, (pop - 24) / 8, value);
                IncRateOfGrowth(pos, -8);
                return;
            }

            if (pop == 16)
            {
                IncRateOfGrowth(pos, -8);
                Map[pos.X,pos.Y] = ((ushort)MapTileCharacters.FREEZ | (ushort)MapTileBits.BurnableOrBulldozableOrConductive | (ushort)MapTileBits.CenterOfZone);
                for (x = pos.X - 1; x <= pos.X + 1; x++)
                {
                    for (y = pos.Y - 1; y <= pos.Y + 1; y++)
                    {
                        if (Position.TestBounds(x, y))
                        {
                            if ((Map[x,y] & (ushort)MapTileBits.LowMask) != (ushort)MapTileCharacters.FREEZ)
                            {
                                Map[x,y] = (ushort)((ushort)MapTileCharacters.LHTHR + value + GetRandom(2) + (ushort)MapTileBits.BurnableOrBulldozableOrConductive);
                            }
                        }
                    }
                }
            }

            if (pop < 16)
            {
                IncRateOfGrowth(pos, -1);
                z = 0;
                for (x = pos.X - 1; x <= pos.X + 1; x++)
                {
                    for (y = pos.Y - 1; y <= pos.Y + 1; y++)
                    {
                        if (Position.TestBounds(x, y))
                        {
                            loc = Map[x,y] & (ushort)MapTileBits.LowMask;
                            if ((loc >= (ushort)MapTileCharacters.LHTHR) && (loc <= (ushort)MapTileCharacters.HHTHR))
                            {
                                Map[x,y] = (ushort)(Brdr[z] + (ushort)MapTileBits.BurnableOrBulldozableOrConductive + (ushort)MapTileCharacters.FREEZ - 4);
                                return;
                            }
                        }
                        z++;
                    }
                }
            }
        }

        /// <summary>
        /// Return population of a residential zone center tile (RZB, RZB+9, ..., HOSPITAL - 9).
        /// </summary>
        /// <param name="mapTile">mapTile Center tile of a residential zone.</param>
        /// <returns>Population of the residential zone.</returns>
        public short GetResZonePop(ushort mapTile)
        {
            short CzDen = (short)(((mapTile - (short)MapTileCharacters.RZB) / 9) % 4);
            return (short)(CzDen * 8 + 16);
        }

        /// <summary>
        /// Put down a residential zone.
        /// </summary>
        /// <param name="pos">Center tile of the residential zone.</param>
        /// <param name="den">Population density (0..3)</param>
        /// <param name="value">Land value - pollution (0..3), higher is better.</param>
        public void ResPlop(Position pos, int den, int value)
        {
            short baseT;

            baseT = (short)(((value * 4 + den) * 9) + (short)MapTileCharacters.RZB - 4);
            ZonePlop(pos, baseT);
        }

        /// <summary>
        /// Evaluate residential zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="traf"></param>
        /// <returns></returns>
        public short EvalRes(Position pos, int traf)
        {
            short value;

            if (traf < 0)
            {
                return -3000;
            }

            value = LandValueMap.WorldGet(pos.X, pos.Y);
            value -= PollutionDensityMap.WorldGet(pos.X, pos.Y);

            if (value < 0)
            {
                value = 0;          /* Cap at 0 */
            }
            else
            {
                value = (short)Math.Min(value * 32, 6000); /* Cap at 6000 */
            }

            value = (short)(value - 3000);

            return value;
        }

        /// <summary>
        /// Handle commercial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="zonePower"></param>
        public void DoCommercial(Position pos, bool zonePower) {
            int tpop, TrfGood;
            int zscore, locvalve, value;

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            ComZonePop++;
            tpop = GetComZonePop(tile);
            ComPop += tpop;

            if (tpop > GetRandom(5))
            {
                /* Try driving from commercial to industrial */
                TrfGood = MakeTraffic(pos, ZoneType.Industrial);
            }
            else
            {
                TrfGood = 1;
            }

            if (TrfGood == -1)
            {
                value = GetLandPollutionValue(pos);
                DoComOut(pos, tpop, value);
                return;
            }

            if ((GetRandom16() & 7).IsFalse())
            {

                locvalve = EvalCom(pos, TrfGood);
                zscore = ComValve + locvalve;

                if (!zonePower)
                {
                    zscore = -500;
                }

                if (TrfGood.IsTrue() &&
                    (zscore > -350) &&
                    (((short)(zscore - 26380)) > ((short)GetRandom16Signed())))
                {
                    value = GetLandPollutionValue(pos);
                    DoComIn(pos, tpop, value);
                    return;
                }

                if ((zscore < 350) &&
                    (((short)(zscore + 26380)) < ((short)GetRandom16Signed())))
                {
                    value = GetLandPollutionValue(pos);
                    DoComOut(pos, tpop, value);
                }

            }
        }

        /// <summary>
        /// Handle immigration of commercial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pop"></param>
        /// <param name="value"></param>
        public void DoComIn(Position pos, int pop, int value)
        {
            int z;

            z = LandValueMap.WorldGet(pos.X, pos.Y);
            z = z >> 5;

            if (pop > z)
            {
                return;
            }

            if (pop < 5)
            {
                ComPlop(pos, pop, value);
                IncRateOfGrowth(pos, 8);
            }
        }

        /// <summary>
        /// Handle emigration of commercial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pop"></param>
        /// <param name="value"></param>
        public void DoComOut(Position pos, int pop, int value)
        {
            if (pop > 1)
            {
                ComPlop(pos, pop - 2, value);
                IncRateOfGrowth(pos, -8);
                return;
            }

            if (pop == 1)
            {
                ZonePlop(pos, (ushort)MapTileCharacters.COMBASE);
                IncRateOfGrowth(pos, -8);
            }
        }

        /// <summary>
        /// Get commercial zone population number.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public short GetComZonePop(ushort tile)
        {
            if (tile == (ushort)MapTileCharacters.COMCLR)
            {
                return 0;
            }

            int CzDen = ((tile - (ushort)MapTileCharacters.CZB) / 9) % 5 + 1;
            return (short)CzDen;
        }

        /// <summary>
        /// Build a commercial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="den"></param>
        /// <param name="value"></param>
        public void ComPlop(Position pos, int den, int value)
        {
            int baseT;

            baseT = ((value * 5) + den) * 9 + (ushort)MapTileCharacters.CZB - 4;
            ZonePlop(pos, baseT);
        }

        /// <summary>
        /// Compute evaluation of a commercial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="traf"></param>
        /// <returns></returns>
        public short EvalCom(Position pos, int traf)
        {
            short Value;

            if (traf < 0)
            {
                return -3000;
            }

            Value = ComRateMap.WorldGet(pos.X, pos.Y);

            return Value;
        }

        /// <summary>
        /// Handle industrial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="zonePower"></param>
        public void DoIndustrial(Position pos, bool zonePower) {
            int tpop, zscore, TrfGood;

            ushort tile = (ushort)(Map[pos.X,pos.Y] & (ushort)MapTileBits.LowMask);

            IndZonePop++;
            SetSmoke(pos, zonePower);
            tpop = GetIndZonePop(tile);
            IndPop += tpop;

            if (tpop > GetRandom(5))
            {
                /* Try driving from industrial to residential */
                TrfGood = MakeTraffic(pos, ZoneType.Residential);
            }
            else
            {
                TrfGood = 1;
            }

            if (TrfGood == -1)
            {
                DoIndOut(pos, tpop, GetRandom16() & 1);
                return;
            }

            if ((GetRandom16() & 7).IsFalse())
            {
                zscore = IndValve + EvalInd(TrfGood);

                if (!zonePower)
                {
                    zscore = -500;
                }

                if (zscore > -350 &&
                    (((short)(zscore - 26380)) > ((short)GetRandom16Signed())))
                {
                    DoIndIn(pos, tpop, GetRandom16() & 1);
                    return;
                }

                if (zscore < 350 &&
                    (((short)(zscore + 26380)) < ((short)GetRandom16Signed())))
                {
                    DoIndOut(pos, tpop, GetRandom16() & 1);
                }
            }
        }

        /// <summary>
        /// Handle immigration of industrial zone.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pop"></param>
        /// <param name="value"></param>
        public void DoIndIn(Position pos, int pop, int value)
        {
            if (pop < 4)
            {
                IndPlop(pos, pop, value);
                IncRateOfGrowth(pos, 8);
            }
        }

        /// <summary>
        /// Handle industrial zone emigration.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="pop"></param>
        /// <param name="value"></param>
        public void DoIndOut(Position pos, int pop, int value)
        {
            if (pop > 1)
            {
                IndPlop(pos, pop - 2, value);
                IncRateOfGrowth(pos, -8);
                return;
            }

            if (pop == 1)
            {
                ZonePlop(pos, (ushort)MapTileCharacters.INDBASE); // empty industrial zone
                IncRateOfGrowth(pos, -8);
            }
        }

        /// <summary>
        /// Get the population value for the given industrial tile.
        /// </summary>
        /// <param name="tile">Center tile value of the industrial zone.</param>
        /// <returns></returns>
        public short GetIndZonePop(ushort tile)
        {
            if (tile == (ushort)MapTileCharacters.INDCLR)
            {
                return 0;
            }

            int CzDen = (((tile - (ushort)MapTileCharacters.IZB) / 9) % 4) + 1;
            return (short)CzDen;
        }

        /// <summary>
        /// Place an industrial zone around center tile \a pos.
        /// </summary>
        /// <param name="pos">Center of the industrial zone.</param>
        /// <param name="den">Population density of the industrial zone (0, 1, 2, or 3).</param>
        /// <param name="value">Landvalue of the industrial zone (0 or 1).</param>
        public void IndPlop(Position pos, int den, int value)
        {
            int baseT = ((value * 4) + den) * 9 + (ushort)MapTileCharacters.IND1;
            ZonePlop(pos, baseT);
        }

        /// <summary>
        /// Compute evaluation of an industrial zone.
        /// </summary>
        /// <param name="traf">Result if traffic attempt.</param>
        /// <returns>Evaluation value of the industrial zone.</returns>
        public short EvalInd(int traf)
        {
            if (traf < 0)
            {
                return -1000;
            }

            return 0;
        }

    }
}
