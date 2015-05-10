﻿using MicropolisSharp.Types;
using System;

/// <summary>
/// From Tool.cpp
/// </summary>
namespace MicropolisSharp
{
    public partial class Micropolis
    {
        static ushort[] idArray = {
            (ushort)MapTileCharacters.DIRT, (ushort)MapTileCharacters.RIVER, (ushort)MapTileCharacters.TREEBASE, (ushort)MapTileCharacters.RUBBLE,
            (ushort)MapTileCharacters.FLOOD, (ushort)MapTileCharacters.RADTILE, (ushort)MapTileCharacters.FIRE, (ushort)MapTileCharacters.ROADBASE,
            (ushort)MapTileCharacters.POWERBASE, (ushort)MapTileCharacters.RAILBASE, (ushort)MapTileCharacters.RESBASE, (ushort)MapTileCharacters.COMBASE,
            (ushort)MapTileCharacters.INDBASE, (ushort)MapTileCharacters.PORTBASE, (ushort)MapTileCharacters.AIRPORTBASE, (ushort)MapTileCharacters.COALBASE,
            (ushort)MapTileCharacters.FIRESTBASE, (ushort)MapTileCharacters.POLICESTBASE, (ushort)MapTileCharacters.STADIUMBASE, (ushort)MapTileCharacters.NUCLEARBASE,
            // FIXME: I think HBRDG_END should be HBRDG0...?
            (ushort)MapTileCharacters.HBRDG0, (ushort)MapTileCharacters.RADAR0, (ushort)MapTileCharacters.FOUNTAIN, (ushort)MapTileCharacters.INDBASE2,
            // FIXME: What are tiles 952 and 956?
            (ushort)MapTileCharacters.FOOTBALLGAME1, (ushort)MapTileCharacters.VBRDG0, 952, 956,
            9999, // a huge short
        };

        public ToolResult PutDownPark(int x, int y, ToolEffects effects)
        {
            short value = GetRandom(4);
            ushort tile = (ushort)(MapTileBits.Burnable | MapTileBits.Bulldozable);

            if (value == 4)
            {
                tile |= (ushort)((ushort)MapTileCharacters.FOUNTAIN | (ushort)MapTileBits.Animated);
            }
            else
            {
                tile |= (ushort)(value + (ushort)MapTileCharacters.WOODS2);
            }

            if (effects.GetMapValue(x, y) != (ushort)MapTileCharacters.DIRT)
            {
                return ToolResult.NeedBulldoze;
            }

            effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Park]);
            effects.SetMapValue(x, y, tile);

            return ToolResult.Ok;
        }

		public ToolResult PutDownNetwork(int x, int y, ToolEffects effects)
        {
            ushort tile = effects.GetMapTile(x, y);

            if (tile != (ushort)MapTileCharacters.DIRT && Tally(tile))
            {
                effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Bulldozer]);
                effects.SetMapValue(x, y, (ushort)MapTileCharacters.DIRT);
                tile = (ushort)MapTileCharacters.DIRT;
            }

            if (tile != (ushort)MapTileCharacters.DIRT) return ToolResult.NeedBulldoze;

            effects.SetMapValue(x, y,
                                 (ushort)MapTileCharacters.TELEBASE 
								 | (ushort)MapTileBits.Conductivity 
								 | (ushort)MapTileBits.Burnable 
								 | (ushort)MapTileBits.Bulldozable
                                 | (ushort)MapTileBits.Animated);

            effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Network]);

            return ToolResult.Ok;
        }

        public ToolResult PutDownWater(int x, int y,
                                    ToolEffects effects)
        {
            ushort tile = effects.GetMapTile(x, y);

            if (tile == (ushort)MapTileCharacters.RIVER) return ToolResult.Failed;

            effects.SetMapValue(x, y, (ushort)MapTileCharacters.RIVER);

            effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Water]);

            return ToolResult.Ok;
        }

        public ToolResult PutDownLand(int x, int y, ToolEffects effects)
        {
            ushort tile = effects.GetMapTile(x, y);

            if (tile == (ushort)MapTileCharacters.DIRT) return ToolResult.Failed;

            effects.SetMapValue(x, y, (ushort)MapTileCharacters.DIRT);

            effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Land]);

            return ToolResult.Ok;
        }

        public ToolResult PutDownForest(int x, int y, ToolEffects effects)
        {
            short[] dx = { -1, 0, 1, -1, 1, -1, 0, 1, };
            short[] dy = { -1, -1, -1, 0, 0, 1, 1, 1, };

            effects.SetMapValue(x, y, (ushort)MapTileCharacters.WOODS | (ushort)MapTileBits.BulldozableOrBurnable);

            int i;
            for (i = 0; i < 8; i++)
            {
                int xx = x + dx[i];
                int yy = y + dy[i];
                if ((new Position(xx,yy)).TestBounds())
                {
                    SmoothTreesAt(xx, yy, true, effects);
                }
            }

            effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Forest]);

            return ToolResult.Ok;
        }

        public short CheckBigZone(ushort tileId, ref int deltaHPtr, ref int deltaVPtr)
        {
            switch ((MapTileCharacters)tileId)
            {

                case MapTileCharacters.POWERPLANT:      /* check coal plant */
                case MapTileCharacters.PORT:            /* check sea port */
                case MapTileCharacters.NUCLEAR:         /* check nuc plant */
                case MapTileCharacters.STADIUM:         /* check stadium */
                    deltaHPtr = 0;
                    deltaVPtr = 0;
                    return 4;

                case MapTileCharacters.POWERPLANT + 1:  /* check coal plant */
                case MapTileCharacters.COALSMOKE3:      /* check coal plant, smoke */
                case MapTileCharacters.COALSMOKE3 + 1:  /* check coal plant, smoke */
                case MapTileCharacters.COALSMOKE3 + 2:  /* check coal plant, smoke */
                case MapTileCharacters.PORT + 1:        /* check sea port */
                case MapTileCharacters.NUCLEAR + 1:     /* check nuc plant */
                case MapTileCharacters.STADIUM + 1:     /* check stadium */
                    deltaHPtr = -1;
                    deltaVPtr = 0;
                    return 4;

                case MapTileCharacters.POWERPLANT + 4:  /* check coal plant */
                case MapTileCharacters.PORT + 4:        /* check sea port */
                case MapTileCharacters.NUCLEAR + 4:     /* check nuc plant */
                case MapTileCharacters.STADIUM + 4:     /* check stadium */
                    deltaHPtr = 0;
                    deltaVPtr = -1;
                    return 4;

                case MapTileCharacters.POWERPLANT + 5:  /* check coal plant */
                case MapTileCharacters.PORT + 5:        /* check sea port */
                case MapTileCharacters.NUCLEAR + 5:     /* check nuc plant */
                case MapTileCharacters.STADIUM + 5:     /* check stadium */
                    deltaHPtr = -1;
                    deltaVPtr = -1;
                    return 4;

                case MapTileCharacters.AIRPORT:         /* check airport */
                    deltaHPtr = 0;
                    deltaVPtr = 0;
                    return 6;

                case MapTileCharacters.AIRPORT + 1:
                    deltaHPtr = -1;
                    deltaVPtr = 0;
                    return 6;

                case MapTileCharacters.AIRPORT + 2:
                    deltaHPtr = -2;
                    deltaVPtr = 0;
                    return 6;

                case MapTileCharacters.AIRPORT + 3:
                    deltaHPtr = -3;
                    deltaVPtr = 0;
                    return 6;

                case MapTileCharacters.AIRPORT + 6:
                    deltaHPtr = 0;
                    deltaVPtr = -1;
                    return 6;

                case MapTileCharacters.AIRPORT + 7:
                    deltaHPtr = -1;
                    deltaVPtr = -1;
                    return 6;

                case MapTileCharacters.AIRPORT + 8:
                    deltaHPtr = -2;
                    deltaVPtr = -1;
                    return 6;

                case MapTileCharacters.AIRPORT + 9:
                    deltaHPtr = -3;
                    deltaVPtr = -1;
                    return 6;

                case MapTileCharacters.AIRPORT + 12:
                    deltaHPtr = 0;
                    deltaVPtr = -2;
                    return 6;

                case MapTileCharacters.AIRPORT + 13:
                    deltaHPtr = -1;
                    deltaVPtr = -2;
                    return 6;

                case MapTileCharacters.AIRPORT + 14:
                    deltaHPtr = -2;
                    deltaVPtr = -2;
                    return 6;

                case MapTileCharacters.AIRPORT + 15:
                    deltaHPtr = -3;
                    deltaVPtr = -2;
                    return 6;

                case MapTileCharacters.AIRPORT + 18:
                    deltaHPtr = 0;
                    deltaVPtr = -3;
                    return 6;

                case MapTileCharacters.AIRPORT + 19:
                    deltaHPtr = -1;
                    deltaVPtr = -3;
                    return 6;

                case MapTileCharacters.AIRPORT + 20:
                    deltaHPtr = -2;
                    deltaVPtr = -3;
                    return 6;

                case MapTileCharacters.AIRPORT + 21:
                    deltaHPtr = -3;
                    deltaVPtr = -3;
                    return 6;

                default:
                    deltaHPtr = 0;
                    deltaVPtr = 0;
                    return 0;
            }
        }

        public bool Tally(ushort tileValue)
        {
            return (tileValue >= (ushort)MapTileCharacters.FIRSTRIVEDGE && tileValue <= (ushort)MapTileCharacters.LASTRUBBLE) ||
           (tileValue >= (ushort)MapTileCharacters.POWERBASE + 2 && tileValue <= (ushort)MapTileCharacters.POWERBASE + 12) ||
           (tileValue >= (ushort)MapTileCharacters.TINYEXP && tileValue <= (ushort)MapTileCharacters.LASTTINYEXP + 2);
        }

        public int CheckSize(ushort tileValue)
        {
            // check for the normal com, resl, ind 3x3 zones & the fireDept & PoliceDept
            if ((tileValue >= (ushort)MapTileCharacters.RESBASE - 1 && tileValue <= (ushort)MapTileCharacters.PORTBASE - 1) ||
                (tileValue >= (ushort)MapTileCharacters.LASTPOWERPLANT + 1 && tileValue <= (ushort)MapTileCharacters.POLICESTATION + 4) ||
                (tileValue >= (ushort)MapTileCharacters.CHURCH1BASE && tileValue <= (ushort)MapTileCharacters.CHURCH7LAST))
            {
                return 3;
            }

            if ((tileValue >= (ushort)MapTileCharacters.PORTBASE && tileValue <= (ushort)MapTileCharacters.LASTPORT) ||
                (tileValue >= (ushort)MapTileCharacters.COALBASE && tileValue <= (ushort)MapTileCharacters.LASTPOWERPLANT) ||
                (tileValue >= (ushort)MapTileCharacters.STADIUMBASE && tileValue <= (ushort)MapTileCharacters.LASTZONE))
            {
                return 4;
            }

            return 0;
        }

        public void CheckBorder(int xMap, int yMap, int sizeX, int sizeY, ToolEffects effects)
        {
            short cnt;

            /* this will do the upper bordering row */
            for (cnt = 0; cnt < sizeX; cnt++)
            {
                ConnectTile(xMap + cnt, yMap - 1, ConnectTileCommand.Fix, effects);
            }

            /* this will do the left bordering row */
            for (cnt = 0; cnt < sizeY; cnt++)
            {
                ConnectTile(xMap - 1, yMap + cnt, ConnectTileCommand.Fix, effects);
            }

            /* this will do the bottom bordering row */
            for (cnt = 0; cnt < sizeX; cnt++)
            {
                ConnectTile(xMap + cnt, yMap + sizeY, ConnectTileCommand.Fix, effects);
            }

            /* this will do the right bordering row */
            for (cnt = 0; cnt < sizeY; cnt++)
            {
                ConnectTile(xMap + sizeX, yMap + cnt, ConnectTileCommand.Fix, effects);
            }
        }

        public void PutBuilding(int leftX, int topY, int sizeX, int sizeY, ushort baseTile, bool aniFlag, ToolEffects effects)
        {
            for (int dy = 0; dy < sizeY; dy++)
            {
                int posY = topY + dy;

                for (int dx = 0; dx < sizeX; dx++)
                {
                    int posX = leftX + dx;

                    ushort tileValue = (ushort)(baseTile | (ushort)MapTileBits.BurnableOrConductive);
                    if (dx == 1)
                    {
                        if (dy == 1)
                        {
                            tileValue |= (ushort)MapTileBits.CenterOfZone;
                        }
                        else if (dy == 2 && aniFlag)
                        {
                            tileValue |= (ushort)MapTileBits.Animated;
                        }
                    }

                    effects.SetMapValue(posX, posY, tileValue);

                    baseTile++;
                }
            }
        }

        public ToolResult PrepareBuildingSite(int leftX, int topY, int sizeX, int sizeY, ToolEffects effects)
        {
            // Check that the entire site is on the map
            if (leftX < 0 || leftX + sizeX > Constants.WorldWidth)
            {
                return ToolResult.Failed;
            }
            if (topY < 0 || topY + sizeY > Constants.WorldHeight)
            {
                return ToolResult.Failed;
            }

            // Check whether the tiles are clear
            for (int dy = 0; dy < sizeY; dy++)
            {
                int posY = topY + dy;

                for (int dx = 0; dx < sizeX; dx++)
                {
                    int posX = leftX + dx;

                    ushort tileValue = effects.GetMapTile(posX, posY);

                    if (tileValue == (ushort)MapTileCharacters.DIRT)
                    { // DIRT tile is buidable
                        continue;
                    }

                    if (!AutoBulldoze)
                    {
                        // No DIRT and no bull-dozer => not buildable
                        return ToolResult.NeedBulldoze;
                    }
                    if (!Tally(tileValue))
                    {
                        // tilevalue cannot be auto-bulldozed
                        return ToolResult.NeedBulldoze;
                    }

                    effects.SetMapValue(posX, posY, (ushort)MapTileCharacters.DIRT);
                    effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Bulldozer]);
                }
            }

            return ToolResult.Ok;
        }

        public ToolResult BuildBuilding(int mapH, int mapV, BuildingProperties buildingProps, ToolEffects effects)
        {
            mapH--; mapV--; // Move position to top-left

            ToolResult prepareResult = PrepareBuildingSite(mapH, mapV,
                                                        buildingProps.SizeX,
                                                        buildingProps.SizeY,
                                                        effects);
            if (prepareResult != ToolResult.Ok) {
                return prepareResult;
            }

            /* Preparation was ok, put down the building. */
            effects.AddCost(Constants.CostOfBuildings[(int)buildingProps.Tool]);

            PutBuilding(mapH, mapV, buildingProps.SizeX, buildingProps.SizeY,
                        (ushort)buildingProps.BaseTile, buildingProps.IsAnimated,
                        effects);

            CheckBorder(mapH, mapV,
                        buildingProps.SizeX, buildingProps.SizeY,
                        effects);

            return ToolResult.Ok;
        }

        public int GetDensity(short catNo, short mapH, short mapV)
        {
            int z;

            switch (catNo)
            {

                case 0:
                default:
                    z = PopulationDensityMap.WorldGet(mapH, mapV);
                    z = z >> 6;
                    z = z & 3;
                    return z + (int)ScoreCardMessages.STR202_POPULATIONDENSITY_LOW;

                case 1:
                    z = LandValueMap.WorldGet(mapH, mapV);
                    if (z < 30) return (int)ScoreCardMessages.STR202_LANDVALUE_SLUM;
                    if (z < 80) return (int)ScoreCardMessages.STR202_LANDVALUE_LOWER_CLASS;
                    if (z < 150) return (int)ScoreCardMessages.STR202_LANDVALUE_MIDDLE_CLASS;
                    return (int)ScoreCardMessages.STR202_LANDVALUE_HIGH_CLASS;

                case 2:
                    z = CrimeRateMap.WorldGet(mapH, mapV);
                    z = z >> 6;
                    z = z & 3;
                    return z + (int)ScoreCardMessages.STR202_CRIME_NONE;

                case 3:
                    z = PollutionDensityMap.WorldGet(mapH, mapV);
                    if (z < 64 && z > 0) return 13;
                    z = z >> 6;
                    z = z & 3;
                    return z + (int)ScoreCardMessages.STR202_POLLUTION_NONE;

                case 4:
                    z = RateOfGrowthMap.WorldGet(mapH, mapV);
                    if (z < 0) return (int)ScoreCardMessages.STR202_GROWRATE_DECLINING;
                    if (z == 0) return (int)ScoreCardMessages.STR202_GROWRATE_STABLE;
                    if (z > 100) return (int)ScoreCardMessages.STR202_GROWRATE_FASTGROWTH;
                    return (int)ScoreCardMessages.STR202_GROWRATE_SLOWGROWTH;

            }
        }

        public void DoZoneStatus(short mapH, short mapV)
        {
            int tileCategory;
            int[] status = new int[5];

            ushort tileNum = (ushort)(Map[mapH,mapV] & (ushort)MapTileBits.LowMask);

            if (tileNum >= (ushort)MapTileCharacters.COALSMOKE1 && tileNum < (ushort)MapTileCharacters.FOOTBALLGAME1)
            {
                tileNum = (ushort)MapTileCharacters.COALBASE;
            }

            // Find the category where the tile belongs to
            // Note: If 'tileNum < idArray[i]', it belongs to category i-1
            short i;
            for (i = 1; i < 29; i++)
            {
                if (tileNum < idArray[i])
                {
                    break;
                }
            }

            i--;
            // i contains the category that the tile belongs to (in theory 0..27).
            // However, it is 0..26, since 956 is the first unused tile

            // FIXME: This needs to be fixed to support plug-in churches.

            // TODO: This should also return the bounding box and hot spot of
            // the zone to the user interface, as well as other interesting
            // information.

            // Code below looks buggy, 0 is a valid value (namely 'dirt'), and upper
            // limit is not correctly checked either ('stri.219' has only 27 lines).

            // FIXME: This is strange... Normalize to zero based index.
            if (i < 1 || i > 28)
            {
                i = 28;  // This breaks the program (when you click 'dirt')
            }

            // Obtain the string index of the tile category.
            // 'stri.219' has only 27 lines, so 0 <= i <= 26 is acceptable.
            tileCategory = i + 1;

            for (i = 0; i < 5; i++)
            {
                int id = Utilities.Restrict(GetDensity(i, mapH, mapV) + 1, 1, 20);
                status[i] = id;
            }

            DoShowZoneStatus(
                tileCategory,
                status[0], status[1], status[2],
                status[3], status[4],
                mapH, mapV);
        }

        public void DoShowZoneStatus(
            int tileCategory,
            int s0, int s1, int s2, int s3, int s4,
            int x, int y)
        {
            Callback("showZoneStatus", "dddddddd", tileCategory.ToString(), s0.ToString(), s1.ToString(), s2.ToString(), s3.ToString(), s4.ToString(), x.ToString(), y.ToString());
        }

        public void PutRubble(int x, int y, int size, ToolEffects effects)
        {
            for (int xx = x; xx < x + size; xx++)
            {
                for (int yy = y; yy < y + size; yy++)
                {

                    if ((new Position(xx,yy)).TestBounds())
                    {
                        ushort tile = effects.GetMapTile(xx, yy);

                        if (tile != (ushort)MapTileCharacters.RADTILE && tile != (ushort)MapTileCharacters.DIRT)
                        {
                            tile = (ushort)(DoAnimation ? ((ushort)MapTileCharacters.TINYEXP + GetRandom(2)) : (ushort)MapTileCharacters.SOMETINYEXP);
                            effects.SetMapValue(xx, yy, (ushort)(tile | (ushort)MapTileBits.Animated | (ushort)MapTileBits.Bulldozable));
                        }
                    }
                }
            }
        }

        public void DidTool(String name, short x, short y)
        {
            Callback("didTool", "sdd", name.ToString(), x.ToString(), y.ToString());
        }

        public ToolResult QueryTool(short x, short y)
        {
            if (!(new Position(x,y)).TestBounds())
            {
                return ToolResult.Failed;
            }

            DoZoneStatus(x, y);
            DidTool("Qry", x, y);

            return ToolResult.Ok;
        }

        public ToolResult BulldozerTool(short x, short y)
        {
            ToolEffects effects = new ToolEffects(this);

            ToolResult result = BulldozerTool(x, y, effects);

            if (result == ToolResult.Ok)
            {
                effects.ModifyWorld();
            }

            return result;
        }

        public ToolResult BulldozerTool(short x, short y, ToolEffects effects)
        {
            ToolResult result = ToolResult.Ok;

            if (!(new Position(x,y).TestBounds()))
            {
                return ToolResult.Failed;
            }

            ushort mapVal = effects.GetMapValue(x, y);
            ushort tile = (ushort)(mapVal & (ushort)MapTileBits.LowMask);

            int zoneSize = 0; // size of the zone, 0 means invalid.
            int deltaH = 0; // Amount of horizontal shift to the center tile of the zone.
            int deltaV = 0; // Amount of vertical shift to the center tile of the zone.
            FrontendMessage frontendMsg;

            if ((mapVal & (ushort)MapTileBits.CenterOfZone).IsTrue())
            { /* zone center bit is set */
                zoneSize = CheckSize(tile);
                deltaH = 0;
                deltaV = 0;
            }
            else
            {
                zoneSize = CheckBigZone(tile, ref deltaH, ref deltaV);
            }

            if (zoneSize > 0)
            {
                effects.AddCost(Constants.CostOfBuildings[(int)EditingTool.Bulldozer]);

                int dozeX = x;
                int dozeY = y;
                int centerX = x + deltaH;
                int centerY = y + deltaV;

                switch (zoneSize)
                {

                    case 3:
                        frontendMsg = new FrontendMessageMakeSound(
                                            "city", "Explosion-High", dozeX, dozeY);
                        effects.AddFrontendMessage(frontendMsg);

                        PutRubble(centerX - 1, centerY - 1, 3, effects);
                        break;

                    case 4:
                        frontendMsg = new FrontendMessageMakeSound(
                                            "city", "Explosion-Low", dozeX, dozeY);
                        effects.AddFrontendMessage(frontendMsg);

                        PutRubble(centerX - 1, centerY - 1, 4, effects);
                        break;

                    case 6:
                        frontendMsg = new FrontendMessageMakeSound(
                                            "city", "Explosion-High", dozeX, dozeY);
                        effects.AddFrontendMessage(frontendMsg);

                        frontendMsg = new FrontendMessageMakeSound(
                                            "city", "Explosion-Low", dozeX, dozeY);
                        effects.AddFrontendMessage(frontendMsg);

                        PutRubble(centerX - 1, centerY - 1, 6, effects);
                        break;
                }


                if (result == ToolResult.Ok)
                {
                    /* send 'didtool' message */
                    frontendMsg = new FrontendMessageDidTool("Dozr", x, y);
                    effects.AddFrontendMessage(frontendMsg);
                }

                return result;

            }


            if (tile == (ushort)MapTileCharacters.RIVER || tile == (ushort)MapTileCharacters.REDGE || tile == (ushort)MapTileCharacters.CHANNEL)
            {

                result = ConnectTile(x, y, ConnectTileCommand.Bulldoze, effects);

                if (tile != effects.GetMapTile(x, y))
                {
                    effects.AddCost(5);
                }
            }
            else
            {
                result = ConnectTile(x, y, ConnectTileCommand.Bulldoze, effects);
            }

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                frontendMsg = new FrontendMessageDidTool("Dozr", x, y);
                DidTool("Dozr", x, y);
                effects.AddFrontendMessage(frontendMsg);
            }

            return result;
        }

        public ToolResult RoadTool(short x, short y, ToolEffects effects)
        {
            if (!(new Position(x,y)).TestBounds())
            {
                return ToolResult.Failed;
            }

            ToolResult result = ConnectTile(x, y, ConnectTileCommand.Road, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Road", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult RailroadTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            ToolResult result = ConnectTile(x, y, ConnectTileCommand.RailRoad, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Rail", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult WireTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            ToolResult result = ConnectTile(x, y, ConnectTileCommand.Wire, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Wire", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult ParkTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            ToolResult result = PutDownPark(x, y, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Park", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult BuildBuildingTool(short x, short y,
                                    BuildingProperties buildingProps,
                                    ToolEffects effects)
        {
            ToolResult result = BuildBuilding(x, y, buildingProps, effects);

            if (result == ToolResult.Ok) {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool(buildingProps.Name, x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult NetworkTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Ok;
            }

            ToolResult result = PutDownNetwork(x, y, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Net", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult WaterTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            ToolResult result = BulldozerTool(x, y, effects);

            if (result == ToolResult.Ok)
            {
                result = PutDownWater(x, y, effects);
            }

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Water", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        ToolResult LandTool(short x, short y, ToolEffects effects)
        {
            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Failed;
            }

            /// @todo: Is this good? It is not auto-bulldoze!!
            /// @todo: Handle result value (probably)
            ToolResult result = BulldozerTool(x, y, effects);

            result = PutDownLand(x, y, effects);

            if (result == ToolResult.Ok)
            {
                /* send 'didtool' message */
                FrontendMessage didToolMsg;
                didToolMsg = new FrontendMessageDidTool("Land", x, y);
                effects.AddFrontendMessage(didToolMsg);
            }

            return result;
        }

        public ToolResult ForestTool(short x, short y, ToolEffects effects)
        {
            ToolResult result = ToolResult.Ok;

            if (!Position.TestBounds(x, y))
            {
                return ToolResult.Ok;
            }

            ushort tile = effects.GetMapValue(x, y);

            if (IsTree(tile))
            {
                return ToolResult.Ok;
            }

            if ((tile & (ushort)MapTileBits.LowMask) != (ushort)MapTileCharacters.DIRT)
            {
                /// @todo bulldozer should be free in terrain mode or from a free tool.
                result = BulldozerTool(x, y, effects);
            }

            tile = effects.GetMapValue(x, y);

            if (tile == (ushort)MapTileCharacters.DIRT)
            {
                result = PutDownForest(x, y, effects);

                if (result == ToolResult.Ok)
                {
                    /* send 'didtool' message */
                    FrontendMessage didToolMsg;
                    didToolMsg = new FrontendMessageDidTool("Forest", x, y);
                    effects.AddFrontendMessage(didToolMsg);
                }

            }
            else
            {
                result = ToolResult.Failed;
            }

            return result;
        }

        public ToolResult DoTool(EditingTool tool, short tileX, short tileY)
        {
            ToolEffects effects = new ToolEffects(this);
            ToolResult result;

            switch (tool)
            {

                case EditingTool.Residential:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.ResidentialZone,effects);
                    break;

                case EditingTool.Commercial:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.CommericialZone,
                                               effects);
                    break;

                case EditingTool.Industrial:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.IndustrialZone,
                                               effects);
                    break;

                case EditingTool.FireStation:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.FireStation,
                                               effects);
                    break;

                case EditingTool.PoliceStation:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.PoliceStation,
                                               effects);
                    break;

                case EditingTool.Query:
                    return QueryTool(tileX, tileY);

                case EditingTool.Wire:
                    result = WireTool(tileX, tileY, effects);
                    break;

                case EditingTool.Bulldozer:
                    result = BulldozerTool(tileX, tileY, effects);
                    break;

                case EditingTool.RailRoad:
                    result = RailroadTool(tileX, tileY, effects);
                    break;

                case EditingTool.Road:
                    result = RoadTool(tileX, tileY, effects);
                    break;

                case EditingTool.Stadium:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.Stadium,
                                               effects);
                    break;

                case EditingTool.Park:
                    result = ParkTool(tileX, tileY, effects);
                    break;

                case EditingTool.Seaport:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.Seaport,
                                               effects);
                    break;

                case EditingTool.CoalPower:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.CoalPower,
                                               effects);
                    break;

                case EditingTool.NuclearPower:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.NuclearPower,
                                               effects);
                    break;

                case EditingTool.Airport:
                    result = BuildBuildingTool(tileX, tileY, BuildingProperties.Airport,
                                               effects);
                    break;

                case EditingTool.Network:
                    result = NetworkTool(tileX, tileY, effects);
                    break;

                case EditingTool.Water:
                    result = WaterTool(tileX, tileY, effects);
                    break;

                case EditingTool.Land:
                    result = LandTool(tileX, tileY, effects);
                    break;

                case EditingTool.Forest:
                    result = ForestTool(tileX, tileY, effects);
                    break;

                default:
                    return ToolResult.Failed;

            }

            // Perform the effects of applying the tool if enough funds.
            if (result == ToolResult.Ok)
            {
                if (!effects.ModifyIfEnoughFunding())
                {
                    return ToolResult.NoMoney;
                }
            }

            return result;
        }

        public void ToolDown(EditingTool tool, short tileX, short tileY)
        {
            ToolResult result = DoTool(tool, tileX, tileY);

            if (result == ToolResult.NeedBulldoze)
            {
                SendMessage(GeneralMessages.MESSAGE_BULLDOZE_AREA_FIRST, Constants.NoWhere, Constants.NoWhere, false, true);
                /// @todo: Multi player: This sound should only be heard by the user
                ///        who called this function.
                MakeSound("interface", "UhUh", tileX << 4, tileY << 4);

            }
            else if (result == ToolResult.NoMoney)
            {
                SendMessage(GeneralMessages.MESSAGE_NOT_ENOUGH_FUNDS, Constants.NoWhere, Constants.NoWhere, false, true);
                /// @todo: Multi player: This sound should only be heard by the user
                ///        who called this function.
                MakeSound("interface", "Sorry", tileX << 4, tileY << 4);
            }

            SimPass = 0;
            InvalidateMaps();
        }

        public void ToolDrag(EditingTool tool,
                            short fromX, short fromY, short toX, short toY)
        {
            // Do not drag big tools.
            int toolSize = Constants.ToolSizes[(int)tool];
            if (toolSize > 1)
            {
                DoTool(tool, toX, toY);

                SimPass = 0; // update editors overlapping this one
                InvalidateMaps();
                return;
            }

            short dirX = (toX > fromX) ? (short)1 : (short)-1; // Horizontal step direction.
            short dirY = (toY > fromY) ? (short)1 : (short)-1; // Vertical step direction.


            if (fromX == toX && fromY == toY)
            {
                return;
            }

            DoTool(tool, fromX, fromY); // Ensure the start position is done.

            // Vertical line up or down
            if (fromX == toX && fromY != toY)
            {

                while (fromY != toY)
                {
                    fromY += dirY;
                    DoTool(tool, fromX, fromY);
                }

                SimPass = 0; // update editors overlapping this one
                InvalidateMaps();
                return;
            }

            // Horizontal line left/right
            if (fromX != toX && fromY == toY)
            {

                while (fromX != toX)
                {
                    fromX += dirX;
                    DoTool(tool, fromX, fromY);
                }

                SimPass = 0; // update editors overlapping this one
                InvalidateMaps();
                return;
            }

            // General case: both X and Y change.

            short dx = (short)Math.Abs(fromX - toX); // number of horizontal steps.
            short dy = (short)Math.Abs(fromY - toY); // number of vertical steps.

            short subX = 0; // Each X step is dy sub-steps.
            short subY = 0; // Each Y step is dx sub-steps.
            short numSubsteps = Math.Min(dx, dy); // Number of sub-steps we can do.

            while (fromX != toX || fromY != toY)
            {
                subX += numSubsteps;
                if (subX >= dy)
                {
                    subX -= dy;
                    fromX += dirX;
                    DoTool(tool, fromX, fromY);
                }

                subY += numSubsteps;
                if (subY >= dx)
                {
                    subY -= dx;
                    fromY += dirY;
                    DoTool(tool, fromX, fromY);
                }
            }

            SimPass = 0;
            InvalidateMaps();
        }
    }
}
