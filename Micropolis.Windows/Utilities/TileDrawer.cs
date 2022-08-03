using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Micropolis.Windows.Utilities;

public class TileDrawer
{
    private const int TileSize = 16;

    private const int GridWidth = 256 / TileSize;
    private const int GridHeight = 960 / TileSize;

    private readonly Texture2D _tileSheet;

    public TileDrawer(Texture2D tileSheet)
    {
        _tileSheet = tileSheet;
    }

    public void DrawTile(int tileId, SpriteBatch batch, Vector2 drawPosition, Color overrideColor)
    {
        //Translate Tile Id to grid position
        var y = tileId / GridWidth;
        var x = tileId % GridWidth;

        if (y < 0 || y > GridHeight || x < 0 || x > GridWidth) throw new Exception("Invalid Grid Tile");

        batch.Draw(_tileSheet, Normalise(drawPosition),
            ClippedRectange(drawPosition, new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize)),
            overrideColor);
    }

    private Rectangle ClippedRectange(Vector2 drawPosition, Rectangle original)
    {
        var x = original.X;
        var y = original.Y;
        var w = original.Width;
        var h = original.Height;

        if (drawPosition.X < 0)
        {
            x = (int)(x - drawPosition.X); // x - dp.X == x + Abs(dp.X) because dp.X < 0
            w = (int)(w + drawPosition.X);
        }

        if (drawPosition.Y < 0)
        {
            y = (int)(y - drawPosition.Y); // x - dp.X == x + Abs(dp.X) because dp.X < 0
            h = (int)(h + drawPosition.Y);
        }

        return new Rectangle(x, y, w, h);
    }

    private Vector2 Normalise(Vector2 drawPosition)
    {
        return new Vector2(Math.Max(drawPosition.X, 0), Math.Max(drawPosition.Y, 0));
    }

    internal void DrawAnimatedTile(int tileId, int cycle, SpriteBatch spriteBatch, Vector2 vector2)
    {
        if ((tileId >= 128 && tileId < 143) || (tileId >= 192 && tileId <= 207))
            DrawTile(tileId - 16 * cycle, spriteBatch, vector2, Color.White);
    }
}