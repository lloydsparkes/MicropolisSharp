/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum ConnectTileCommand
    {
        Fix, ///< Fix zone (connect wire, road, and rail).
        Bulldoze, ///< Bulldoze and fix zone.
        Road, ///< Lay road and fix zone.
        RailRoad, ///< Lay rail and fix zone.
        Wire, ///< Lay wire and fix zone.
    }
}
