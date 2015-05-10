/* Micropolis.Stubs.cs
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
    /// Partial Class Containing the content of stubs.cpp
    /// </summary>
    public partial class Micropolis
    {
        public void InvalidateMaps()
        {
            MapSerial++;
            Callback("update", "s", "map");
        }

        public void MakeSound(String channel, String sound, int x, int y)
        {
            Callback("makeSound", "ssdd", channel, sound, x.ToString(), y.ToString());
        }

        public void SetFunds(int dollars)
        {
            TotalFunds = dollars;
            UpdateFunds();
        }

        public long TickCount()
        {
            return DateTime.Now.Ticks;
        }

        public void InitGame()
        {
            SimPaused = false;
            SimPausedSpeed = 0;
            SimPass = 0;
            SimPasses = 1;
            HeatSteps = 0;
            SetSpeed(0);
        }

        public void Callback(String name, params String[] values)
        {
            //TODO: Implement Call Backs - its not fullr requires atm
            throw new NotImplementedException();
        }

        public void DoEarthquake(int strength)
        {
            MakeSound("city", "ExplosionLow", Constants.NoWhere, Constants.NoWhere); // Make the sound all over.

            Callback("startEarthquake", "d", strength.ToString());
        }

        public int GetTile(int x, int y)
        {
            if (!Position.TestBounds(x, y))
            {
                return (ushort)MapTileCharacters.DIRT;
            }

            return Map[x,y];
        }

        public void SetTile(int x, int y, int tile)
        {
            if (!Position.TestBounds(x, y))
            {
                return;
            }

            Map[x,y] = (ushort)tile;
        }

        public ushort[,] GetMapBuffer()
        {
            return Map;
        }

        public int GetPowerGrid(int x, int y)
        {
            return PowerGridMap.WorldGet(x, y);
        }

        public void SetPowerGrid(int x, int y, int power)
        {
            PowerGridMap.WorldSet(x, y, (byte)power);
        }

        public ByteMap1 GetPowerGridMapBuffer()
        {
            return PowerGridMap;
        }

        public int GetPopulationDensity(int x, int y)
        {
            return PopulationDensityMap.Get(x, y);
        }

        public void SetPopulationDensity(int x, int y, int density)
        {
            PopulationDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetPopulationDensityMapBuffer()
        {
            return PopulationDensityMap;
        }

        public int GetRateOfGrowth(int x, int y)
        {
            return RateOfGrowthMap.Get(x, y);
        }

        public void SetRateOfGrowth(int x, int y, int rate)
        {
            RateOfGrowthMap.Set(x, y, (short)rate);
        }

        public ShortMap8 GetRateOfGrowthMapBuffer()
        {
            return RateOfGrowthMap;
        }

        int GetTrafficDensity(int x, int y)
        {
            return TrafficDensityMap.Get(x, y);
        }

        void SetTrafficDensity(int x, int y, int density)
        {
            TrafficDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetTrafficDensityMapBuffer()
        {
            return TrafficDensityMap;
        }

        public int GetPollutionDensity(int x, int y)
        {
            return PollutionDensityMap.Get(x, y);
        }

        public void SetPollutionDensity(int x, int y, int density)
        {
            PollutionDensityMap.Set(x, y, (byte)density);
        }

        public ByteMap2 GetPollutionDensityMapBuffer()
        {
            return PollutionDensityMap;
        }

        public int GetCrimeRate(int x, int y)
        {
            return CrimeRateMap.Get(x, y);
        }

        public void SetCrimeRate(int x, int y, int rate)
        {
            CrimeRateMap.Set(x, y, (byte)rate);
        }

        public ByteMap2 getCrimeRateMapBuffer()
        {
            return CrimeRateMap;
        }

        public int GetLandValue(int x, int y)
        {
            return LandValueMap.Get(x, y);
        }

        public void SetLandValue(int x, int y, int value)
        {
            LandValueMap.Set(x, y, (byte)value);
        }

        public ByteMap2 GetLandValueMapBuffer()
        {
            return LandValueMap;
        }

        public int GetFireCoverage(int x, int y)
        {
            return FireStationEffectMap.Get(x, y);
        }

        public void SetFireCoverage(int x, int y, int coverage)
        {
            FireStationEffectMap.Set(x, y, (short)coverage);
        }

        public ShortMap8 getFireCoverageMapBuffer()
        {
            return FireStationEffectMap;
        }

        public int GetPoliceCoverage(int x, int y)
        {
            return PoliceStationEffectMap.Get(x, y);
        }

        public void SetPoliceCoverage(int x, int y, int coverage)
        {
            PoliceStationEffectMap.Set(x, y, (short)coverage);
        }

        public ShortMap8 GetPoliceCoverageMapBuffer()
        {
            return PoliceStationEffectMap;
        }
    }
}
