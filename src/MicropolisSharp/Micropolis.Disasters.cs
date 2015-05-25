/* Micropolis.Disasters.cs
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
using System.Diagnostics;

namespace MicropolisSharp
{
    /// <summary>
    /// Partial Class Containing the content of disasters.cpp
    /// </summary>
    public partial class Micropolis
    {
        public short FloodCount { get; private set; }

        /// <summary>
        /// Let disasters happen.
        /// 
        /// TODO: Decide what to do with the 'nothing happens' disaster (since the
        ///       chance that a disaster happens is expressed in the \c DisChance
        ///       table).
        /// </summary>
        private void DoDisasters()
        { 
            /* Chance of disasters at lev 0 1 2 */
            short[] DisChance = {
                10 * 48, // Game level 0
                5 * 48,  // Game level 1
                60 // Game level 2
            };
            Debug.Assert((int)Levels.Count == DisChance.Length);

            if (FloodCount.IsTrue())
            {
                FloodCount--;
            }

            if (DisasterEvent != Scenario.None)
            {
                ScenarioDisaster();
            }

            if (!EnableDisasters)
            { // Disasters have been disabled
                return;
            }

            Levels x = GameLevel;
            if (x > Levels.Last)
            {
                x = Levels.Easy;
            }

            if (GetRandom(DisChance[(int)x]).IsFalse())
            {
                switch (GetRandom(8))
                {
                    case 0:
                    case 1:
                        SetFire();  // 2/9 chance a fire breaks out
                        break;

                    case 2:
                    case 3:
                        MakeFlood(); // 2/9 chance for a flood
                        break;

                    case 4:
                        // 1/9 chance nothing happens (was airplane crash,
                        // which EA removed after 9/11, and requested it be
                        // removed from this code)
                        break;

                    case 5:
                        MakeTornado(); // 1/9 chance tornado
                        break;

                    case 6:
                        MakeEarthquake(); // 1/9 chance earthquake
                        break;

                    case 7:
                    case 8:
                        // 2/9 chance a scary monster arrives in a dirty town
                        if (PollutionAverage > /* 80 */ 60)
                        {
                            MakeMonster();
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Let disasters of the scenario happen
        /// </summary>
        private void ScenarioDisaster()
        {
            switch (DisasterEvent)
            {
                case Scenario.Dullsville:
                    break;

                case Scenario.SanFrancisco:
                    if (DisasterWait == 1)
                    {
                        MakeEarthquake();
                    }
                    break;

                case Scenario.Hamburg:
                    if (DisasterWait % 10 == 0)
                    {
                        MakeFireBombs();
                    }
                    break;

                case Scenario.Bern:
                    break;

                case Scenario.Tokyo:
                    if (DisasterWait == 1)
                    {
                        MakeMonster();
                    }
                    break;

                case Scenario.Detroit:
                    break;

                case Scenario.Boston:
                    if (DisasterWait == 1)
                    {
                        MakeMeltdown();
                    }
                    break;

                case Scenario.Rio:
                    if ((DisasterWait % 24) == 0)
                    {
                        MakeFlood();
                    }
                    break;
            }

            if (DisasterWait > 0)
            {
                DisasterWait--;
            }
            else
            {
                DisasterEvent = Scenario.None;
            }
        }

        /// <summary>
        /// Make a nuclear power plant melt
        /// 
        /// TODO: Randomize which Nuclear Power Plant melts down
        /// </summary>
        public void MakeMeltdown()
        {
            short x, y;

            for (x = 0; x < (Constants.WorldWidth - 1); x++)
            {
                for (y = 0; y < (Constants.WorldHeight - 1); y++)
                {
                    if ((Map[x,y] & (ushort)MapTileBits.LowMask) == (ushort)MapTileCharacters.NUCLEAR)
                    {
                        DoMeltdown(new Position(x, y));
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Let a fire bomb explode at a random location
        /// </summary>
        public void FireBomb()
        {
            int crashX = GetRandom(Constants.WorldWidth - 1);
            int crashY = GetRandom(Constants.WorldHeight - 1);
            MakeExplosion(crashX, crashY);
            SendMessage(GeneralMessages.MESSAGE_FIREBOMBING, (short)crashX, (short)crashY, true, true);
        }

        /// <summary>
        /// Throw several bombs onto the city.
        /// </summary>
        public void MakeFireBombs()
        {
            int count = 2 + (GetRandom16() & 1);

            while (count > 0)
            {
                FireBomb();
                count--;
            }

            // TODO: Schedule periodic fire bombs over time, every few ticks.
        }

        /// <summary>
        /// Change random tiles to fire or dirt as result of the earthquake
        /// </summary>
        public void MakeEarthquake()
        {
            short x, y, z;

            int strength = GetRandom(700) + 300; // strength/duration of the earthquake

            DoEarthquake(strength);

            SendMessage(GeneralMessages.MESSAGE_EARTHQUAKE, CityCenterX, CityCenterY, true, false);

            for (z = 0; z < strength; z++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);

                if (Vulnerable(Map[x,y]))
                {

                    if ((z & 0x3) != 0)
                    { // 3 of 4 times reduce to rubble
                        Map[x,y] = RandomRubble();
                    }
                    else
                    {
                        // 1 of 4 times start fire
                        Map[x,y] = RandomFire();
                    }
                }
            }
        }

        /// <summary>
        /// Start a fire at a random place, random disaster or scenario 
        /// </summary>
        public void SetFire()
        {
            short x, y, z;

            x = GetRandom(Constants.WorldWidth - 1);
            y = GetRandom(Constants.WorldHeight - 1);
            z = (short)Map[x,y];

            if ((z & (short)MapTileBits.CenterOfZone) == 0)
            {
                z = (short)(z & (short)MapTileBits.LowMask);
                if (z > (short)MapTileCharacters.LHTHR && z < (short)MapTileCharacters.LASTZONE)
                {
                    Map[x,y] = RandomFire();
                    SendMessage(GeneralMessages.MESSAGE_FIRE_REPORTED, x, y, true, false);
                }
            }
        }

        /// <summary>
        /// Start a fire at a random place, requested by user 
        /// </summary>
        public void MakeFire()
        {
            short t, x, y, z;

            for (t = 0; t < 40; t++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);
                z = (short)Map[x,y];

                if (((z & (short)MapTileBits.CenterOfZone).IsFalse()) && (z & (short)MapTileBits.Burnable).IsTrue())
                {
                    z = (short)(z & (short)MapTileBits.LowMask);
                    if ((z > 21) && (z < (short)MapTileCharacters.LASTZONE))
                    {
                        Map[x,y] = RandomFire();
                        SendMessage(GeneralMessages.MESSAGE_FIRE_REPORTED, x, y, false, false);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Flood many tiles
        /// 
        /// TODO: Use Direction and some form of XYPosition class here
        /// </summary>
        public void MakeFlood()
        {
            short[] Dx = { 0, 1, 0, -1 };
            short[] Dy = { -1, 0, 1, 0 };
            short xx, yy, c;
            short z, t, x, y;

            for (z = 0; z < 300; z++)
            {
                x = GetRandom(Constants.WorldWidth - 1);
                y = GetRandom(Constants.WorldHeight - 1);
                c = (short)(Map[x,y] & (short)MapTileBits.LowMask);

                if (c > (short)MapTileCharacters.CHANNEL && c <= (short)MapTileCharacters.WATER_HIGH)
                { /* if riveredge  */
                    for (t = 0; t < 4; t++)
                    {
                        xx = (short)(x + Dx[t]);
                        yy = (short)(y + Dy[t]);
                        if (Position.TestBounds(xx, yy))
                        {
                            c = (short)Map[xx,yy];

                            /* tile is floodable */
                            if (c == (short)MapTileCharacters.DIRT
                                  || (c & ((short)MapTileBits.Bulldozable | (short)MapTileBits.Burnable)) == ((short)MapTileBits.Bulldozable | (short)MapTileBits.Burnable))
                            {
                                Map[xx,yy] = (ushort)MapTileCharacters.FLOOD;
                                FloodCount = 30;
                                SendMessage(GeneralMessages.MESSAGE_FLOODING_REPORTED, xx, yy, true, false);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Is tile vulnerable for an earthquake?
        /// </summary>
        /// <param name="tem">Tile data</param>
        /// <returns>Function returns true if tile is vulnerable, and false if not</returns>
        private bool Vulnerable(int tem)
        {
            int tem2 = tem & (short)MapTileBits.LowMask;

            if (tem2 < (short)MapTileCharacters.RESBASE || tem2 > (short)MapTileCharacters.LASTZONE || (tem & (short)MapTileBits.CenterOfZone).IsTrue())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Flood around the given position.
        /// 
        /// TODO: Use some form of rotating around a position.
        /// </summary>
        /// <param name="pos">Position around which to flood further.</param>
        private void DoFlood(Position pos)
        {
            short[] Dx = { 0, 1, 0, -1 };
            short[] Dy = { -1, 0, 1, 0 };

            if (FloodCount > 0)
            {
                // Flood is not over yet
                for (int z = 0; z < 4; z++)
                {
                    if ((GetRandom16() & 7) == 0)
                    { // 12.5% chance
                        int xx = pos.X + Dx[z];
                        int yy = pos.Y + Dy[z];
                        if (Position.TestBounds(xx, yy))
                        {
                            ushort c = Map[xx,yy];
                            ushort t = (ushort)(c & (ushort)MapTileBits.LowMask);

                            if ((c & (ushort)MapTileBits.Burnable) == (ushort)MapTileBits.Burnable || c == (ushort)MapTileCharacters.DIRT
                                                    || (t >= (ushort)MapTileCharacters.WOODS5 && t < (ushort)MapTileCharacters.FLOOD))
                            {
                                if ((c & (ushort)MapTileBits.CenterOfZone) == (ushort)MapTileBits.CenterOfZone)
                                {
                                    FireZone(new Position(xx, yy), c);
                                }
                                Map[xx,yy] = (ushort)(MapTileCharacters.FLOOD + GetRandom(2));
                            }
                        }
                    }
                }
            }
            else
            {
                if ((GetRandom16() & 15) == 0)
                { // 1/16 chance
                    Map[pos.X,pos.Y] = (ushort)MapTileCharacters.DIRT;
                }
            }
        }
    }
}
