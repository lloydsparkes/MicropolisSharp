using System;
using MicropolisSharp.Types;

/// <summary>
/// From Connect.cpp
/// </summary>

namespace MicropolisSharp
{
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

        private static ushort NeutralizeRoad(ushort tile)
        {
            if (tile >= 64 && tile <= 207)
            {
                tile = (ushort)((tile & 0x000F) + 64);
            }
            return tile;
        }

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
