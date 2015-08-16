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
            //TODO: Optimise for things which are not on the screen
            Point realOffset = new Point(drawingOffset.X * 16, drawingOffset.Y * 16);

            Console.WriteLine("Sprite: {0} X:{1} Y:{2} - Act: X:{3} Y: {4}", sprite.Type, sprite.X, sprite.Y, sprite.X - realOffset.X, sprite.Y - realOffset.Y);
            spriteBatch.Draw(_spritesheet, new Vector2(sprite.X - realOffset.X, sprite.Y - realOffset.Y),
                GetFrameRect(sprite), Color.White);
        }

        private Rectangle GetFrameRect(SimSprite sprite)
        {
            return new Rectangle(sprite.Frame * sprite.Width, 0, sprite.Width, sprite.Height);
        }
    }
}
