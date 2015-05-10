/// <summary>
/// From micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public enum MapTileBits
    {
        Power = 0x8000, ///< bit 15, tile has power.
        Conductivity = 0x4000, ///< bit 14. tile can conduct electricity.
        Burnable = 0x2000, ///< bit 13, tile can be lit.
        Bulldozable = 0x1000, ///< bit 12, tile is bulldozable.
        Animated = 0x0800, ///< bit 11, tile is animated.
        CenterOfZone = 0x0400, ///< bit 10, tile is the center tile of the zone.

        /// Mask for the bits-part of the tile
        AllBits = CenterOfZone | Animated | Bulldozable | Burnable | Conductivity | Power,
        LowMask = 0x03ff, ///< Mask for the #MapTileCharacters part of the tile

        BulldozableOrBurnable = Bulldozable | Burnable,
        BurnableOrBulldozableOrConductive = Burnable | Bulldozable | Conductivity,
        BurnableOrConductive = Burnable | Conductivity,
    }
}
