/* Micropolis.Power.cs
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
    /// Partial Class Containing the content of power.cpp
    /// </summary>
    public partial class Micropolis
    {
        private int powerStackPointer = 0;
        private Position[] powerStackXY = new Position[Constants.PowerStackSize];

        /// <summary>
        /// Scan the map for powered tiles, and copy them to the Micropolis::powerGridMap array.
        /// 
        /// Also warns the user about using too much power ('buy another power plant').
        /// </summary>
        public void DoPowerScan()
        {
            Direction anyDir, dir;
            int conNum;

            // Clear power map.
            PowerGridMap.Clear();

            // Power that the combined coal and nuclear power plants can deliver.
            long maxPower = CoalPowerPop * Constants.CoalPowerStrength +
                            NuclearPowerPop * Constants.NuclearPowerStrength;

            long numPower = 0; // Amount of power used.

            while (powerStackPointer > 0)
            {
                Position pos = PullPowerStack();
                anyDir = Direction.Invalid;
                do
                {
                    numPower++;
                    if (numPower > maxPower)
                    {
                        SendMessage(GeneralMessages.MESSAGE_NOT_ENOUGH_POWER, Constants.NoWhere, Constants.NoWhere, false, false);
                        return;
                    }
                    if (anyDir != Direction.Invalid)
                    {
                        pos.Move(anyDir);
                    }
                    PowerGridMap.WorldSet(pos.X, pos.Y, 1);
                    conNum = 0;
                    dir = Direction.Begin;
                    while (dir < Direction.End && conNum < 2)
                    {
                        if (TestForConductive(pos, dir))
                        {
                            conNum++;
                            anyDir = dir;
                        }
                        dir = dir.Increment90();
                    }
                    if (conNum > 1)
                    {
                        PushPowerStack(pos);
                    }
                } while (conNum.IsTrue());
            }
        }

        /// <summary>
        /// Check at position \a pos for a power-less conducting tile in the direction \a testDir.
        /// 
        /// TODO: Re-use something like Micropolis::getFromMap(), and fold this function into its caller.
        /// </summary>
        /// <param name="pos">Position to start from.</param>
        /// <param name="testDir">Direction to investigate.</param>
        /// <returns>Unpowered tile has been found in the indicated direction.</returns>
        public bool TestForConductive(Position pos, Direction testDir)
        {
            Position movedPos = new Position(pos);

            if (movedPos.Move(testDir))
            {
                if ((Map[movedPos.X,movedPos.Y] & (ushort)MapTileBits.Conductivity) == (ushort)MapTileBits.Conductivity)
                {
                    if (PowerGridMap.WorldGet(movedPos.X, movedPos.Y).IsFalse())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Push position \a pos onto the power stack if there is room.
        /// </summary>
        /// <param name="pos">Position to push.</param>
        public void PushPowerStack(Position pos)
        {
            if (powerStackPointer < (Constants.PowerStackSize - 2))
            {
                powerStackPointer++;
                powerStackXY[powerStackPointer] = new Position(pos);
            }
        }

        /// <summary>
        /// Pull a position from the power stack.
        /// 
        /// Stack must be non-empty (powerStackPointer > 0).
        /// </summary>
        /// <returns>Pulled position.</returns>
        public Position PullPowerStack()
        {
            //TODO: Make this an assert
            if(powerStackPointer > 0)
            {
                powerStackPointer--;
                return powerStackXY[powerStackPointer + 1];
            }
            //TODO: Change this to an Assert really
            return null;
        }
    }
}
