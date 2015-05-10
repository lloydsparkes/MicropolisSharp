namespace MicropolisSharp
{
    public static class Constants
    {
        //From generate.h
        public const int IslandRadius = 18;

        //From map_type.h
        public const int WorldWidth = 120;
        public const int WorldHeight = 100;

        //From micopolis.h
        public const int BitsPerTile = 16;
        public const int BytesPerTile = 16;

        public const int WorldWidth2 = WorldWidth / 2;
        public const int WorldHeight2 = WorldHeight / 2;

        public const int WorldWidth4 = WorldWidth / 4;
        public const int WorldHeight4 = WorldHeight / 4;

        public const int WorldWidth8 = WorldWidth / 8;
        public const int WorldHeight8 = (WorldHeight + 7) / 8;
 
        public const int EditorTileSize = 16;

        //Time
        public const int PassesPerCityTime = 16;
        public const int CityTimesPerMonth = 4;
        public const int CityTimesPerYear = CityTimesPerMonth * 12;

        //File Loading / Saving
        public const int HistoryLength = 480;
        public const int MiscHistoryLength = 240;
        public const int HistoryCount = 120;

        public const int PowerStackSize = (WorldWidth * WorldHeight) / 4;
        
        public const int NoWhere = -1;
      
        //Traffic
        public const int MaxTrafficDistance = 30;
        public const int MaxRoadEffect = 32;
        public const int MaxPoliceStationEffect = 1000;
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
