/* Constants.cs
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

namespace MicropolisSharp
{
    /// <summary>
    /// Various Key Constants & Defines found around the Micropolis Code Base. All here in one place
    /// 
    /// Only put bits here, if its used in more than one place (or more than one Partial Class)
    /// 
    /// TODO: Make this configuration? - Where sensible
    /// TODO: Is there anything here which should not be here?
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Default Radius of an Island used by the Terrain Generator - from generate.h
        /// </summary>
        public const int IslandRadius = 18;

        /// <summary>
        /// Default World Size - from map_type.h
        /// </summary>
        public const int WorldWidth = 120;
        public const int WorldHeight = 100;

        /// <summary>
        /// TODO: Remove this? Is it even needed?
        /// The number of bits per tile
        /// </summary>
        [Obsolete]
        public const int BitsPerTile = 16;

        /// <summary>
        /// TODO: Remove this? Is it even needed?
        /// The number of bytes per tile
        /// </summary>
        [Obsolete]
        public const int BytesPerTile = 2;

        /// <summary>
        /// World Widths / Heights for maps with different block sizes
        /// 
        /// TODO: Can this be removed?
        /// </summary>
        [Obsolete]
        public const int WorldWidth2 = WorldWidth / 2;
        [Obsolete]
        public const int WorldHeight2 = WorldHeight / 2;
        [Obsolete]
        public const int WorldWidth4 = WorldWidth / 4;
        [Obsolete]
        public const int WorldHeight4 = WorldHeight / 4;
        [Obsolete]
        public const int WorldWidth8 = WorldWidth / 8;
        [Obsolete]
        public const int WorldHeight8 = (WorldHeight + 7) / 8;

        /// <summary>
        /// Size of Editor view Tiles - in pixels
        /// 
        /// TODO: Can this be removed?
        /// </summary>
        [Obsolete]
        public const int EditorTileSize = 16;

        //Time

        /// <summary>
        /// The number of simulator passes per CityTime unit
        /// </summary>
        public const int PassesPerCityTime = 16;

        /// <summary>
        /// The number of CityTime units per month
        /// </summary>
        public const int CityTimesPerMonth = 4;

        /// <summary>
        /// The number of CityTime units per year
        /// </summary>
        public const int CityTimesPerYear = CityTimesPerMonth * 12;

        //File Loading / Saving

        /// <summary>
        /// The length of the History Storage Array (Max Number of History Items)
        /// </summary>
        public const int HistoryLength = 480;

        /// <summary>
        /// The length of the Misc History Storage Array (Max Number of Misc History Items)
        /// </summary>
        public const int MiscHistoryLength = 240;

        /// <summary>
        /// The length of the history tables
        /// 
        /// TODO: Its not really a count - rename
        /// </summary>
        public const int HistoryCount = 120;

        /// <summary>
        /// The size of the power stack
        /// </summary>
        public const int PowerStackSize = (WorldWidth * WorldHeight) / 4;
        
        /// <summary>
        /// Use in place of X/Y to indicate "nowhere"
        /// </summary>
        public const int NoWhere = -1;
      
        //Traffic

        /// <summary>
        /// The maximum number of tiles to drive looking for a destination
        /// </summary>
        public const int MaxTrafficDistance = 30;

        /// <summary>
        /// The maximum value of RoadEffect
        /// </summary>
        public const int MaxRoadEffect = 32;

        /// <summary>
        /// The maximum value of PoliceEffect
        /// </summary>
        public const int MaxPoliceStationEffect = 1000;

        /// <summary>
        /// The maximum value of FireEffect
        /// </summary>
        public const int MaxFireStationEffect = 1000;

        //Valves
        public const int ResValveRange = 2000;
        public const int ComValveRange = 1500;
        public const int IndValveRange = 1500;

        //Strength
        public const long CoalPowerStrength = 700L;
        public const long NuclearPowerStrength = 2000L;

        //Tool.cpp
        public static short[] CostOfBuildings =  new short[] {
             100,    100,    100,    500, /* res, com, ind, fire */
             500,      0,      5,      1, /* police, query, wire, bulldozer */
              20,     10,   5000,     10, /* rail, road, stadium, park */
            3000,   3000,   5000,  10000, /* seaport, coal, nuclear, airport */
             100,      0,      0,      0, /* network, water, land, forest */
               0,
        };

        public static short[] ToolSizes = new short[] {
            3, 3, 3, 3,
            3, 1, 1, 1,
            1, 1, 4, 1,
            4, 4, 4, 6,
            1, 1, 1, 1,
            0,
        };

        public const int BusGrooveX = -39;
        public const int BusGrooveY = 6;
        public const int TraGrooveX = -39;
        public const int TraGrooveY = 6;

        //From Simulate.cpp
        public const int CensusFrequency10 = 4;
        public const int CensusFrequency120 = CensusFrequency10 * 12;
        public const int TaxFrequency = 48;
    }
}
