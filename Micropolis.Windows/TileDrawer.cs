using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micropolis.Basic
{
    public class TileDrawer
    {
        private const int TILE_SIZE = 16;

        private const int GRID_WIDTH = 256 / TILE_SIZE;
        private const int GRID_HEIGHT = 960 / TILE_SIZE;

        private Texture2D _tileSheet;

        public TileDrawer(Texture2D tileSheet)
        {
            _tileSheet = tileSheet;
        }

        public void DrawTile(int tileId, SpriteBatch batch, Vector2 drawPosition, Color overrideColor)
        {
            //Translate Tile Id to grid position
            int y = tileId / GRID_WIDTH;
            int x = tileId % GRID_WIDTH;

            if ((y < 0 || y > GRID_HEIGHT) || (x < 0 || x > GRID_WIDTH))
            {
                throw new Exception("Invalid Grid Tile");
            }

            batch.Draw(_tileSheet, drawPosition, new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE), overrideColor);
        }

        internal void DrawAnimatedTile(int tileId, int cycle, SpriteBatch spriteBatch, Vector2 vector2)
        {
            if ((tileId >= 128 && tileId < 143) || (tileId >= 192 && tileId <= 207))
            {
                DrawTile(tileId - (16 * cycle), spriteBatch, vector2, Color.White);
            }
        }
    }
}
