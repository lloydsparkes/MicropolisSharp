/// <summary>
/// From tool.h
/// </summary>

namespace MicropolisSharp.Types
{
    public class BuildingProperties
    {
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public MapTileCharacters BaseTile { get; private set; }
        public EditingTool Tool { get; private set; }
        public string Name { get; private set; }
        public bool IsAnimated { get; private set; }

        public BuildingProperties(int xs, int ys, MapTileCharacters baseTile, EditingTool tool, string name, bool anim)
        {
            this.SizeX = xs;
            this.SizeY = ys;
            this.BaseTile = baseTile;
            this.Tool = tool;
            this.Name = name;
            this.IsAnimated = anim;
        }

        //From Tool.cpp
        public static BuildingProperties ResidentialZone = new BuildingProperties(3, 3, MapTileCharacters.RESBASE, EditingTool.Residential, "Res", false);
        public static BuildingProperties CommericialZone = new BuildingProperties(3, 3, MapTileCharacters.COMBASE, EditingTool.Commercial, "Com", false);
        public static BuildingProperties IndustrialZone  = new BuildingProperties(3, 3, MapTileCharacters.INDBASE, EditingTool.Industrial, "Ind", false);
        public static BuildingProperties PoliceStation   = new BuildingProperties(3, 3, MapTileCharacters.POLICESTBASE, EditingTool.PoliceStation, "Pol", false);
        public static BuildingProperties FireStation     = new BuildingProperties(3, 3, MapTileCharacters.FIREBASE, EditingTool.FireStation, "Fire", false);
        public static BuildingProperties Stadium         = new BuildingProperties(4, 4, MapTileCharacters.STADIUMBASE, EditingTool.Stadium, "Stad", false);
        public static BuildingProperties CoalPower       = new BuildingProperties(4, 4, MapTileCharacters.COALBASE, EditingTool.CoalPower, "Coal", false);
        public static BuildingProperties NuclearPower    = new BuildingProperties(4, 4, MapTileCharacters.NUCLEARBASE, EditingTool.NuclearPower, "Nuc", true);
        public static BuildingProperties Seaport         = new BuildingProperties(4, 4, MapTileCharacters.PORTBASE, EditingTool.Seaport, "Seap", false);
        public static BuildingProperties Airport         = new BuildingProperties(6, 6, MapTileCharacters.AIRPORTBASE, EditingTool.Airport, "Airp", false);
    }
}
