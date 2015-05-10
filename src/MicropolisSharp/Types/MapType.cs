/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum MapType
    {
        All,                 ///< All zones
        Res,                   ///< Residential zones
        Com,                   ///< Commercial zones
        Ind,                   ///< Industrial zones
        Power,                 ///< Power connectivity
        Road,                  ///< Roads
        PopulationDensity,    ///< Population density
        RateOfGrowth,        ///< Rate of growth
        TrafficDensity,       ///< Traffic
        Pollution,             ///< Pollution
        Crime,                 ///< Crime rate
        LandValue,            ///< Land value
        FireRadius,           ///< Fire station coverage radius
        PoliceRadius,         ///< Police station coverage radius
        Dynamic,               ///< Dynamic filter

        Count,                 ///< Number of map types
    }
}
