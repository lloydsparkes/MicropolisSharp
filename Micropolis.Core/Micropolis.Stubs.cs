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
using System.Diagnostics;
using System.Linq;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of stubs.cpp
    /// </summary>
    public partial class Micropolis
    {
        /// <summary>
        /// Tell the front-end that the maps are not valid any more
        /// </summary>
        public void InvalidateMaps()
        {
            MapSerial++;
            Callback("update", "s", "map");
        }

        /// <summary>
        /// Tell Front End to make a sound
        /// </summary>
        /// <param name="channel">Name of the sound channel, which can effect the
        ///                sound(location, volume, spatialization, etc).
        ///                Use "city" for city sounds effects, and "interface"
        ///                for user interface sounds.</param>
        /// <param name="sound">Name of the sound.</param>
        /// <param name="x">Tile X position of sound, 0 to WORLD_W, or -1 for everywhere.</param>
        /// <param name="y">Tile Y position of sound, 0 to WORLD_H, or -1 for everywhere.</param>
        public void MakeSound(String channel, String sound, int x, int y)
        {
            Callback("makeSound", "ssdd", channel, sound, x.ToString(), y.ToString());
        }

        /// <summary>
        /// Set player funds to \a dollars.
        /// 
        /// Modify the player funds, and warn the front-end about the new amount of
        /// money.
        /// </summary>
        /// <param name="dollars">New value for the player funds.</param>
        public void SetFunds(int dollars)
        {
            TotalFunds = dollars;
            UpdateFunds();
        }

        /// <summary>
        /// Get number of ticks.
        /// 
        /// TODO: What is a Tick
        /// </summary>
        /// <returns></returns>
        public long TickCount()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Initialize the game.
        /// 
        /// Called from the Scripting Language
        /// </summary>
        public void InitGame()
        {
            SimPaused = false;
            SimPausedSpeed = 0;
            SimPass = 0;
            SimPasses = 1;
            HeatSteps = 0;
            SetSpeed(0);
        }

        /// <summary>
        /// Scripting language independent callback mechanism.
        /// 
        /// This allows Micropolis to send callback messages with
        /// a variable number of typed parameters back to the
        /// scripting language, while maintining independence from
        /// the particular scripting language(or user interface
        /// runtime).
        /// 
        /// The name is the name of a message to send.
        /// The params is a string that specifies the number and
        /// types of the following vararg parameters.
        /// There is one character in the param string per vararg
        /// parameter. The following parameter types are currently
        /// supported:
        ///  - i: integer
        ///  - f: float
        ///  - s: string
        /// 
        /// See PythonCallbackHook defined in \c swig/micropolis-swig-python.i
        /// for an example of a callback function.
        /// 
        /// TODO: Replace this with a specific API
        /// </summary>
        /// <param name="name">Name of the callback.</param>
        /// <param name="values">Parameters of the callback.</param>
        public void Callback(String name, params String[] values)
        {
            //TODO: Implement Call Backs - its not fullr requires atm
            //throw new NotImplementedException();
            //Debug.WriteLine("Callback Called: " + name + ", vars: " + String.Join(",", values));
        }

        /// <summary>
        /// Tell the front-end to show an earthquake to the user (shaking the map for some time).
        /// </summary>
        /// <param name="strength"></param>
        public void DoEarthquake(int strength)
        {
            MakeSound("city", "ExplosionLow", Constants.NoWhere, Constants.NoWhere); // Make the sound all over.

            Callback("startEarthquake", "d", strength.ToString());
        }

        /// <summary>
        /// Get a Tile from the Map
        /// 
        /// TODO: Remove this
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetTile(int x, int y)
        {
            if (!Position.TestBounds(x, y))
            {
                return (ushort)MapTileCharacters.DIRT;
            }

            return Map[x,y];
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tile"></param>
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

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetPowerGrid(int x, int y)
        {
            return PowerGridMap.WorldGet(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPowerGrid(int x, int y, int power)
        {
            PowerGridMap.WorldSet(x, y, (byte)power);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap1 GetPowerGridMapBuffer()
        {
            return PowerGridMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetPopulationDensity(int x, int y)
        {
            return PopulationDensityMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPopulationDensity(int x, int y, int density)
        {
            PopulationDensityMap.Set(x, y, (byte)density);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap2 GetPopulationDensityMapBuffer()
        {
            return PopulationDensityMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetRateOfGrowth(int x, int y)
        {
            return RateOfGrowthMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetRateOfGrowth(int x, int y, int rate)
        {
            RateOfGrowthMap.Set(x, y, (short)rate);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ShortMap8 GetRateOfGrowthMapBuffer()
        {
            return RateOfGrowthMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        int GetTrafficDensity(int x, int y)
        {
            return TrafficDensityMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetTrafficDensity(int x, int y, int density)
        {
            TrafficDensityMap.Set(x, y, (byte)density);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap2 GetTrafficDensityMapBuffer()
        {
            return TrafficDensityMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetPollutionDensity(int x, int y)
        {
            return PollutionDensityMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPollutionDensity(int x, int y, int density)
        {
            PollutionDensityMap.Set(x, y, (byte)density);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap2 GetPollutionDensityMapBuffer()
        {
            return PollutionDensityMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetCrimeRate(int x, int y)
        {
            return CrimeRateMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCrimeRate(int x, int y, int rate)
        {
            CrimeRateMap.Set(x, y, (byte)rate);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap2 getCrimeRateMapBuffer()
        {
            return CrimeRateMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetLandValue(int x, int y)
        {
            return LandValueMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetLandValue(int x, int y, int value)
        {
            LandValueMap.Set(x, y, (byte)value);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ByteMap2 GetLandValueMapBuffer()
        {
            return LandValueMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetFireCoverage(int x, int y)
        {
            return FireStationEffectMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetFireCoverage(int x, int y, int coverage)
        {
            FireStationEffectMap.Set(x, y, (short)coverage);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ShortMap8 getFireCoverageMapBuffer()
        {
            return FireStationEffectMap;
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int GetPoliceCoverage(int x, int y)
        {
            return PoliceStationEffectMap.Get(x, y);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetPoliceCoverage(int x, int y, int coverage)
        {
            PoliceStationEffectMap.Set(x, y, (short)coverage);
        }

        /// <summary>
        /// TODO: Remove THis
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ShortMap8 GetPoliceCoverageMapBuffer()
        {
            return PoliceStationEffectMap;
        }
    }
}
