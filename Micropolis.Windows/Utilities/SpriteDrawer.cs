using MicropolisSharp.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micropolis.Windows.Utilities
{
    public class SpriteDrawer
    {
        private Texture2D _spritesheet;

        public SpriteDrawer(Texture2D spritesheet)
        {
            _spritesheet = spritesheet;
        }

        public void Draw(SimSprite sprite, SpriteBatch spriteBatch, Point drawingOffset)
        {
            if(sprite.X - drawingOffset.X < 0 || sprite.Y - drawingOffset.Y < 0)
            {
                return;
            }
            spriteBatch.Draw(_spritesheet, new Vector2(sprite.X - drawingOffset.X, sprite.Y - drawingOffset.Y),
                GetFrameRect(sprite), Color.White);
        }

        private Rectangle GetFrameRect(SimSprite sprite)
        {
            return new Rectangle(sprite.Frame * sprite.Width, 0, sprite.Width, sprite.Height);
        }
    }
}
