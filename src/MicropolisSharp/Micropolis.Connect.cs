/* Micropolis.Connect.cs
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
    /// Partial Class Containing the content of connect.cpp
    /// </summary>
    public partial class Micropolis
    {
        static ushort[] RoadTable = {
            (ushort)MapTileCharacters.ROADS, (ushort)MapTileCharacters.ROADS2, (ushort)MapTileCharacters.ROADS, (ushort)MapTileCharacters.ROADS3,
            (ushort)MapTileCharacters.ROADS2, (ushort)MapTileCharacters.ROADS2, (ushort)MapTileCharacters.ROADS4, (ushort)MapTileCharacters.ROADS8,
            (ushort)MapTileCharacters.ROADS, (ushort)MapTileCharacters.ROADS6, (ushort)MapTileCharacters.ROADS, (ushort)MapTileCharacters.ROADS7,
            (ushort)MapTileCharacters.ROADS5, (ushort)MapTileCharacters.ROADS10, (ushort)MapTileCharacters.ROADS9, (ushort)MapTileCharacters.INTERSECTION
        };

        static ushort[] RailTable = {
            (ushort)MapTileCharacters.LHRAIL, (ushort)MapTileCharacters.LVRAIL, (ushort)MapTileCharacters.LHRAIL, (ushort)MapTileCharacters.LVRAIL2,
            (ushort)MapTileCharacters.LVRAIL, (ushort)MapTileCharacters.LVRAIL, (ushort)MapTileCharacters.LVRAIL3, (ushort)MapTileCharacters.LVRAIL7,
            (ushort)MapTileCharacters.LHRAIL, (ushort)MapTileCharacters.LVRAIL5, (ushort)MapTileCharacters.LHRAIL, (ushort)MapTileCharacters.LVRAIL6,
            (ushort)MapTileCharacters.LVRAIL4, (ushort)MapTileCharacters.LVRAIL9, (ushort)MapTileCharacters.LVRAIL8, (ushort)MapTileCharacters.LVRAIL10
        };

        static ushort[] WireTable = {
            (ushort)MapTileCharacters.LHPOWER, (ushort)MapTileCharacters.LVPOWER, (ushort)MapTileCharacters.LHPOWER, (ushort)MapTileCharacters.LVPOWER2,
            (ushort)MapTileCharacters.LVPOWER, (ushort)MapTileCharacters.LVPOWER, (ushort)MapTileCharacters.LVPOWER3, (ushort)MapTileCharacters.LVPOWER7,
            (ushort)MapTileCharacters.LHPOWER, (ushort)MapTileCharacters.LVPOWER5, (ushort)MapTileCharacters.LHPOWER, (ushort)MapTileCharacters.LVPOWER6,
            (ushort)MapTileCharacters.LVPOWER4, (ushort)MapTileCharacters.LVPOWER9, (ushort)MapTileCharacters.LVPOWER8, (ushort)MapTileCharacters.LVPOWER10
        };

        /// <summary>
        /// Remove road from the tile
        /// </summary>
        /// <param name="tile">The tile to remove the road from</param>
        /// <returns></returns>
        private static ushort NeutralizeRoad(ushort tile)
        {
            if (tile >= 64 && tile <= 207)
            {
                tile = (ushort)((tile & 0x000F) + 64);
            }
            return tile;
        }

        /// <summary>
        /// Perform the command, and fix wire/road/rail/zone connections around it.
        /// 
        /// Store modification in the \a effects object.
        /// 
        /// TODO: Change X,Y to position
        /// </summary>
        /// <param name="x">X - Where to perform the command</param>
        /// <param name="y">Y - Where to perform the command</param>
        /// <param name="cmd">Command to perform</param>
        /// <param name="effects">Modification collecting Objects</param>
        /// <returns>The result</returns>
        public ToolResult ConnectTile(int x, int y, ConnectTileCommand cmd, ToolEffects effects)
        {
            ToolResult result = ToolResult.Ok;

            // Make sure the array subscripts are in bounds.
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            // Perform auto-doze if appropriate.
            switch (cmd)
            {

                case ConnectTileCommand.Road:
                case ConnectTileCommand.RailRoad:
                case ConnectTileCommand.Wire:

                    // Silently skip auto-bulldoze if no money.
                    if (AutoBulldoze)
                    {

                        ushort mapVal = effects.GetMapValue(x, y);

                        if ((mapVal & (ushort)MapTileBits.Bulldozable).IsTrue())
                        {
                            mapVal &= (ushort)MapTileBits.LowMask;
                            mapVal = NeutralizeRoad(mapVal);

                            /* Maybe this should check BULLBIT instead of checking tile values? */
                            if ((mapVal >= (ushort)MapTileCharacters.TINYEXP && mapVal <= (ushort)MapTileCharacters.LASTTINYEXP) ||
                                    (mapVal < (ushort)MapTileCharacters.HBRIDGE && mapVal != (ushort)MapTileCharacters.DIRT))
                            {

                                effects.AddCost(1);

                                effects.SetMapValue(x, y, (ushort)MapTileCharacters.DIRT);

                            }
                        }
                    }
                    break;

                default:
                    // Do nothing.
                    break;

            }

            // Perform the command.
            switch (cmd)
            {

                case ConnectTileCommand.Fix: // Fix zone.
                    FixZone(x, y, effects);
                    break;

                case ConnectTileCommand.Bulldoze: // Bulldoze zone.
                    result = LayDoze(x, y, effects);
                    FixZone(x, y, effects);
                    break;

                case ConnectTileCommand.Road: // Lay road.
                    result = LayRoad(x, y, effects);
                    FixZone(x, y, effects);
                    break;

                case ConnectTileCommand.RailRoad: // Lay railroad.
                    result = LayRail(x, y, effects);
                    FixZone(x, y, effects);
                    break;

                case ConnectTileCommand.Wire: // Lay wire.
                    result = LayWire(x, y, effects);
                    FixZone(x, y, effects);
                    break;

                default:
                    //Do Nothing
                    break;

            }

            return result;
        }

        /// <summary>
        /// Bulldoze a tile (River or Dirt)
        /// 
        /// TODO: Change X,Y to position
        /// </summary>
        /// <param name="x">X - Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        /// <returns>Tool Result</returns>
        public ToolResult LayDoze(int x, int y, ToolEffects effects)
        {
            ushort tile = effects.GetMapValue(x, y);

            if ((tile & (ushort)MapTileBits.Bulldozable).IsFalse())
            {
                return ToolResult.Failed;         /* Check dozeable bit. */
            }

            tile &= (ushort)MapTileBits.LowMask;
            tile = NeutralizeRoad(tile);

            switch (tile)
            {
                case (ushort)MapTileCharacters.HBRIDGE:
                case (ushort)MapTileCharacters.VBRIDGE:
                case (ushort)MapTileCharacters.BRWV:
                case (ushort)MapTileCharacters.BRWH:
                case (ushort)MapTileCharacters.HBRDG0:
                case (ushort)MapTileCharacters.HBRDG1:
                case (ushort)MapTileCharacters.HBRDG2:
                case (ushort)MapTileCharacters.HBRDG3:
                case (ushort)MapTileCharacters.VBRDG0:
                case (ushort)MapTileCharacters.VBRDG1:
                case (ushort)MapTileCharacters.VBRDG2:
                case (ushort)MapTileCharacters.VBRDG3:
                case (ushort)MapTileCharacters.HPOWER:
                case (ushort)MapTileCharacters.VPOWER:
                case (ushort)MapTileCharacters.HRAIL:
                case (ushort)MapTileCharacters.VRAIL:           /* Dozing over water, replace with water. */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.RIVER);
                    break;

                default:              /* Dozing on land, replace with land.  Simple, eh? */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.DIRT);
                    break;
            }

            effects.AddCost(1);                     /* Costs $1.00.... */

            return ToolResult.Ok;
        }

        /// <summary>
        /// Lay a road, and update the road around it
        /// 
        /// TODO: Change X,Y to a position
        /// </summary>
        /// <param name="x">X - Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        /// <returns>Tool Result</returns>
        public ToolResult LayRoad(int x, int y, ToolEffects effects)
        {
            int cost = 10;

            ushort tile = effects.GetMapTile(x, y);

            switch (tile)
            {

                case (ushort)MapTileCharacters.DIRT:
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.ROADS | (ushort)MapTileBits.Bulldozable | (ushort)MapTileBits.Burnable);
                    break;

                case (ushort)MapTileCharacters.RIVER:                   /* Road on Water */
                case (ushort)MapTileCharacters.REDGE:
                case (ushort)MapTileCharacters.CHANNEL:                 /* Check how to build bridges, if possible. */
                    cost = 50;

                    if (x < Constants.WorldWidth - 1)
                    {
                        tile = effects.GetMapTile(x + 1, y);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.VRAILROAD || tile == (ushort)MapTileCharacters.HBRIDGE
                                            || (tile >= (ushort)MapTileCharacters.ROADS && tile <= (ushort)MapTileCharacters.HROADPOWER))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.HBRIDGE | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (x > 0)
                    {
                        tile = effects.GetMapTile(x - 1, y);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.VRAILROAD || tile == (ushort)MapTileCharacters.HBRIDGE
                                            || (tile >= (ushort)MapTileCharacters.ROADS && tile <= (ushort)MapTileCharacters.INTERSECTION))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.HBRIDGE | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (y < Constants.WorldHeight - 1)
                    {
                        tile = effects.GetMapTile(x, y + 1);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.HRAILROAD || tile == (ushort)MapTileCharacters.VROADPOWER
                                            || (tile >= (ushort)MapTileCharacters.VBRIDGE && tile <= (ushort)MapTileCharacters.INTERSECTION))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.VBRIDGE | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (y > 0)
                    {
                        tile = effects.GetMapTile(x, y - 1);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.HRAILROAD || tile == (ushort)MapTileCharacters.VROADPOWER
                                            || (tile >= (ushort)MapTileCharacters.VBRIDGE && tile <= (ushort)MapTileCharacters.INTERSECTION))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.VBRIDGE | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    /* Can't do road... */
                    return ToolResult.Failed;

                case (ushort)MapTileCharacters.LHPOWER:         /* Road on power */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.VROADPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LVPOWER:         /* Road on power #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.HROADPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LHRAIL:          /* Road on rail */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.HRAILROAD | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LVRAIL:          /* Road on rail #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.VRAILROAD | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                default:              /* Can't do road */
                    return ToolResult.Failed;

            }

            effects.AddCost(cost);
            return ToolResult.Ok;
        }

        /// <summary>
        /// Lay a rail, update the connections around it
        /// 
        /// TODO: Change X,Y to a position
        /// </summary>
        /// <param name="x">X -Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        /// <returns>Tool Result</returns>
        public ToolResult LayRail(int x, int y, ToolEffects effects)
        {
            int cost = 20;

            ushort tile = effects.GetMapTile(x, y);

            tile = NeutralizeRoad(tile);

            switch (tile)
            {
                case (ushort)MapTileCharacters.DIRT:                   /* Rail on Dirt */

                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.LHRAIL | (ushort)MapTileBits.Bulldozable | (ushort)MapTileBits.Burnable);

                    break;

                case (ushort)MapTileCharacters.RIVER:      /* Rail on Water */
                case (ushort)MapTileCharacters.REDGE:
                case (ushort)MapTileCharacters.CHANNEL:    /* Check how to build underwater tunnel, if possible. */

                    cost = 100;

                    if (x < Constants.WorldWidth - 1)
                    {
                        tile = effects.GetMapTile(x + 1, y);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.RAILHPOWERV || tile == (ushort)MapTileCharacters.HRAIL
                                                || (tile >= (ushort)MapTileCharacters.LHRAIL && tile <= (ushort)MapTileCharacters.HRAILROAD))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.HRAIL | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (x > 0)
                    {
                        tile = effects.GetMapTile(x - 1, y);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.RAILHPOWERV || tile == (ushort)MapTileCharacters.HRAIL
                                                || (tile > (ushort)MapTileCharacters.VRAIL && tile < (ushort)MapTileCharacters.VRAILROAD))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.HRAIL | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (y < Constants.WorldHeight - 1)
                    {
                        tile = effects.GetMapTile(x, y + 1);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.RAILVPOWERH || tile == (ushort)MapTileCharacters.VRAILROAD
                                                || (tile > (ushort)MapTileCharacters.HRAIL && tile < (ushort)MapTileCharacters.HRAILROAD))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.VRAIL | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    if (y > 0)
                    {
                        tile = effects.GetMapTile(x, y - 1);
                        tile = NeutralizeRoad(tile);
                        if (tile == (ushort)MapTileCharacters.RAILVPOWERH || tile == (ushort)MapTileCharacters.VRAILROAD
                                                || (tile > (ushort)MapTileCharacters.HRAIL && tile < (ushort)MapTileCharacters.HRAILROAD))
                        {
                            effects.SetMapValue(x, y, (ushort)MapTileCharacters.VRAIL | (ushort)MapTileBits.Bulldozable);
                            break;
                        }
                    }

                    /* Can't do rail... */
                    return ToolResult.Failed;

                case (ushort)MapTileCharacters.LHPOWER:             /* Rail on power */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.RAILVPOWERH | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LVPOWER:             /* Rail on power #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.RAILHPOWERV | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.ROADS:              /* Rail on road */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.VRAILROAD | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.ROADS2:              /* Rail on road #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.HRAILROAD | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                default:              /* Can't do rail */
                    return ToolResult.Failed;
            }

            effects.AddCost(cost);
            return ToolResult.Ok;
        }

        /// <summary>
        /// Lay a wire, and update connections (rail, road, and wire) around it.
        /// 
        /// TODO: Change X,Y to a position
        /// </summary>
        /// <param name="x">X -Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        /// <returns>Tool Result</returns>
        public ToolResult LayWire(int x, int y, ToolEffects effects)
        {
            int cost = 5;

            ushort tile = effects.GetMapTile(x, y);

            tile = NeutralizeRoad(tile);

            switch (tile)
            {

                case (ushort)MapTileCharacters.DIRT:            /* Wire on Dirt */

                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.LHPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);

                    break;

                case (ushort)MapTileCharacters.RIVER:           /* Wire on Water */
                case (ushort)MapTileCharacters.REDGE:
                case (ushort)MapTileCharacters.CHANNEL:         /* Check how to lay underwater wire, if possible. */

                    cost = 25;

                    if (x < Constants.WorldWidth - 1)
                    {
                        tile = effects.GetMapValue(x + 1, y);
                        if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                        {
                            tile &= (ushort)MapTileBits.LowMask;
                            tile = NeutralizeRoad(tile);
                            if (tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.RAILHPOWERV && tile != (ushort)MapTileCharacters.HPOWER)
                            {
                                effects.SetMapValue(x, y, (ushort)MapTileCharacters.VPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Bulldozable);
                                break;
                            }
                        }
                    }

                    if (x > 0)
                    {
                        tile = effects.GetMapValue(x - 1, y);
                        if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                        {
                            tile &= (ushort)MapTileBits.LowMask;
                            tile = NeutralizeRoad(tile);
                            if (tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.RAILHPOWERV && tile != (ushort)MapTileCharacters.HPOWER)
                            {
                                effects.SetMapValue(x, y, (ushort)MapTileCharacters.VPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Bulldozable);
                                break;
                            }
                        }
                    }

                    if (y < Constants.WorldHeight - 1)
                    {
                        tile = effects.GetMapValue(x, y + 1);
                        if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                        {
                            tile &= (ushort)MapTileBits.LowMask;
                            tile = NeutralizeRoad(tile);
                            if (tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.RAILVPOWERH && tile != (ushort)MapTileCharacters.VPOWER)
                            {
                                effects.SetMapValue(x, y, (ushort)MapTileCharacters.HPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Bulldozable);
                                break;
                            }
                        }
                    }

                    if (y > 0)
                    {
                        tile = effects.GetMapValue(x, y - 1);
                        if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                        {
                            tile &= (ushort)MapTileBits.LowMask;
                            tile = NeutralizeRoad(tile);
                            if (tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.RAILVPOWERH && tile != (ushort)MapTileCharacters.VPOWER)
                            {
                                effects.SetMapValue(x, y, (ushort)MapTileCharacters.HPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Bulldozable);
                                break;
                            }
                        }
                    }

                    /* Can't do wire... */
                    return ToolResult.Failed;

                case (ushort)MapTileCharacters.ROADS:              /* Wire on Road */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.HROADPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.ROADS2:              /* Wire on Road #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.VROADPOWER | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LHRAIL:             /* Wire on rail */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.RAILHPOWERV | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                case (ushort)MapTileCharacters.LVRAIL:             /* Wire on rail #2 */
                    effects.SetMapValue(x, y, (ushort)MapTileCharacters.RAILVPOWERH | (ushort)MapTileBits.Conductivity | (ushort)MapTileBits.Burnable | (ushort)MapTileBits.Bulldozable);
                    break;

                default:              /* Can't do wire */
                    return ToolResult.Failed;

            }

            effects.AddCost(cost);
            return ToolResult.Ok;
        }

        /// <summary>
        /// Update connections (rails, and wire connections) to a zone.
        /// </summary>
        /// <param name="x">X -Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        public void FixZone(int x, int y, ToolEffects effects)
        {
            FixSingle(x, y, effects);

            if (y > 0)
            {
                FixSingle(x, y - 1, effects);
            }

            if (x < Constants.WorldWidth - 1)
            {
                FixSingle(x + 1, y, effects);
            }

            if (y < Constants.WorldHeight - 1)
            {
                FixSingle(x, y + 1, effects);
            }

            if (x > 0)
            {
                FixSingle(x - 1, y, effects);
            }
        }

        /// <summary>
        /// Modify road, rails, and wire connections at a given tile.
        /// 
        /// TODO: Change X,Y to a position
        /// </summary>
        /// <param name="x">X -Where</param>
        /// <param name="y">Y - Where</param>
        /// <param name="effects">Modification Collecting Collection</param>
        public void FixSingle(int x, int y, ToolEffects effects)
        {
            ushort adjTile = 0;

            ushort tile = effects.GetMapTile(x, y);

            tile = NeutralizeRoad(tile);

            if (tile >= (ushort)MapTileCharacters.ROADS && tile <= (ushort)MapTileCharacters.INTERSECTION)
            {           /* Cleanup Road */

                if (y > 0)
                {
                    tile = effects.GetMapTile(x, y - 1);
                    tile = NeutralizeRoad(tile);
                    if ((tile == (ushort)MapTileCharacters.HRAILROAD || (tile >= (ushort)MapTileCharacters.ROADBASE && tile <= (ushort)MapTileCharacters.VROADPOWER))
                                        && tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.ROADBASE)
                    {
                        adjTile |= 0x0001;
                    }
                }

                if (x < Constants.WorldWidth - 1)
                {
                    tile = effects.GetMapTile(x + 1, y);
                    tile = NeutralizeRoad(tile);
                    if ((tile == (ushort)MapTileCharacters.VRAILROAD || (tile >= (ushort)MapTileCharacters.ROADBASE && tile <= (ushort)MapTileCharacters.VROADPOWER))
                                        && tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.HRAILROAD
                                        && tile != (ushort)MapTileCharacters.VBRIDGE)
                    {
                        adjTile |= 0x0002;
                    }
                }

                if (y < Constants.WorldWidth - 1)
                {
                    tile = effects.GetMapTile(x, y + 1);
                    tile = NeutralizeRoad(tile);
                    if ((tile == (ushort)MapTileCharacters.HRAILROAD || (tile >= (ushort)MapTileCharacters.ROADBASE && tile <= (ushort)MapTileCharacters.VROADPOWER))
                                        && tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.ROADBASE)
                    {
                        adjTile |= 0x0004;
                    }
                }

                if (x > 0)
                {
                    tile = effects.GetMapTile(x - 1, y);
                    tile = NeutralizeRoad(tile);
                    if ((tile == (ushort)MapTileCharacters.VRAILROAD || (tile >= (ushort)MapTileCharacters.ROADBASE && tile <= (ushort)MapTileCharacters.VROADPOWER))
                                        && tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.HRAILROAD
                                        && tile != (ushort)MapTileCharacters.VBRIDGE)
                    {
                        adjTile |= 0x0008;
                    }
                }

                effects.SetMapValue(x, y, (ushort)(RoadTable[adjTile] | (ushort)MapTileBits.Bulldozable | (ushort)MapTileBits.Burnable));
                return;
            }

            if (tile >= (ushort)MapTileCharacters.LHRAIL && tile <= (ushort)MapTileCharacters.LVRAIL10)
            {         /* Cleanup Rail */

                if (y > 0)
                {
                    tile = effects.GetMapTile(x, y - 1);
                    tile = NeutralizeRoad(tile);
                    if (tile >= (ushort)MapTileCharacters.RAILHPOWERV && tile <= (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.RAILHPOWERV && tile != (ushort)MapTileCharacters.HRAILROAD
                                        && tile != (ushort)MapTileCharacters.HRAIL)
                    {
                        adjTile |= 0x0001;
                    }
                }

                if (x < Constants.WorldWidth - 1)
                {
                    tile = effects.GetMapTile(x + 1, y);
                    tile = NeutralizeRoad(tile);
                    if (tile >= (ushort)MapTileCharacters.RAILHPOWERV && tile <= (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.RAILVPOWERH && tile != (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.VRAIL)
                    {
                        adjTile |= 0x0002;
                    }
                }

                if (y < Constants.WorldWidth - 1)
                {
                    tile = effects.GetMapTile(x, y + 1);
                    tile = NeutralizeRoad(tile);
                    if (tile >= (ushort)MapTileCharacters.RAILHPOWERV && tile <= (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.RAILHPOWERV && tile != (ushort)MapTileCharacters.HRAILROAD
                                        && tile != (ushort)MapTileCharacters.HRAIL)
                    {
                        adjTile |= 0x0004;
                    }
                }

                if (x > 0)
                {
                    tile = effects.GetMapTile(x - 1, y);
                    tile = NeutralizeRoad(tile);
                    if (tile >= (ushort)MapTileCharacters.RAILHPOWERV && tile <= (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.RAILVPOWERH && tile != (ushort)MapTileCharacters.VRAILROAD
                                        && tile != (ushort)MapTileCharacters.VRAIL)
                    {
                        adjTile |= 0x0008;
                    }
                }

                effects.SetMapValue(x, y, (ushort)(RailTable[adjTile] | (ushort)MapTileBits.Bulldozable | (ushort)MapTileBits.Burnable));
                return;
            }

            if (tile >= (ushort)MapTileCharacters.LHPOWER && tile <= (ushort)MapTileCharacters.LVPOWER10)
            {         /* Cleanup Wire */

                if (y > 0)
                {
                    tile = effects.GetMapValue(x, y - 1);
                    if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        tile &= (ushort)MapTileBits.LowMask;
                        tile = NeutralizeRoad(tile);
                        if (tile != (ushort)MapTileCharacters.VPOWER && tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.RAILVPOWERH)
                        {
                            adjTile |= 0x0001;
                        }
                    }
                }

                if (x < Constants.WorldWidth - 1)
                {
                    tile = effects.GetMapValue(x + 1, y);
                    if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        tile &= (ushort)MapTileBits.LowMask;
                        tile = NeutralizeRoad(tile);
                        if (tile != (ushort)MapTileCharacters.HPOWER && tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.RAILHPOWERV)
                        {
                            adjTile |= 0x0002;
                        }
                    }
                }

                if (y < Constants.WorldHeight - 1)
                {
                    tile = effects.GetMapValue(x, y + 1);
                    if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        tile &= (ushort)MapTileBits.LowMask;
                        tile = NeutralizeRoad(tile);
                        if (tile != (ushort)MapTileCharacters.VPOWER && tile != (ushort)MapTileCharacters.VROADPOWER && tile != (ushort)MapTileCharacters.RAILVPOWERH)
                        {
                            adjTile |= 0x0004;
                        }
                    }
                }

                if (x > 0)
                {
                    tile = effects.GetMapValue(x - 1, y);
                    if ((tile & (ushort)MapTileBits.Conductivity).IsTrue())
                    {
                        tile &= (ushort)MapTileBits.LowMask;
                        tile = NeutralizeRoad(tile);
                        if (tile != (ushort)MapTileCharacters.HPOWER && tile != (ushort)MapTileCharacters.HROADPOWER && tile != (ushort)MapTileCharacters.RAILHPOWERV)
                        {
                            adjTile |= 0x0008;
                        }
                    }
                }

                effects.SetMapValue(x, y, (ushort)(WireTable[adjTile] | (ushort)MapTileBits.BurnableOrBulldozableOrConductive));
                return;
            }
        }

    }
}
