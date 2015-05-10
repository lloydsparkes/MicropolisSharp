/// <summary>
/// From Micropolis.h
/// </summary>

namespace MicropolisSharp.Types
{
    public class SimSprite
    {
        public SimSprite Next { get; set; } ///< Pointer to next #SimSprite object in the list.
        public string Name{ get; set; } ///< Name of the sprite.
        public SpriteType Type{ get; set; } ///< Type of the sprite (TRA -- BUS).
        public int Frame{ get; set; } ///< Frame (\c 0 means non-active sprite)
        public int X{ get; set; } ///< X coordinate of the sprite in pixels?
        public int Y{ get; set; } ///< Y coordinate of the sprite in pixels?
        public int Width{ get; set; }
        public int Height{ get; set; }
        public int XOffset{ get; set; }
        public int YOffset{ get; set; }
        public int XHot{ get; set; } ///< Offset of the hot-spot relative to SimSprite::x?
        public int YHot{ get; set; } ///< Offset of the hot-spot relative to SimSprite::y?
        public int OrigX{ get; set; }
        public int OrigY{ get; set; }
        public int DestX{ get; set; } ///< Destination X coordinate of the sprite.
        public int DestY{ get; set; } ///< Destination Y coordinate of the sprite.
        public int Count{ get; set; }
        public int SoundCount{ get; set; }
        public int Dir{ get; set; }
        public int NewDir{ get; set; }
        public int Step{ get; set; }
        public int Flag{ get; set; }
        public int Control{ get; set; }
        public int Turn{ get; set; }
        public int Accel{ get; set; }
        public int Speed{ get; set; }
    }
}
