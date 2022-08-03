using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Micropolis.Windows.Utilities;

public class AnimatedTileDrawer
{
    private int _frame;
    private readonly int _frameCount;
    private readonly TileDrawer _tileDrawer;

    public AnimatedTileDrawer(TileDrawer tileDrawer, int startingTileId, int frameOffset, int frames)
    {
        _tileDrawer = tileDrawer;
        _frameCount = (frames - 1) * 50;
        var index = new List<int>();
        for (var i = 0; i < frames; i++) index.Add(startingTileId + frameOffset * i);
        FrameIndex = index.ToArray();
    }

    public int[] FrameIndex { get; }

    public void Update()
    {
        _frame++;
        if (_frame > _frameCount) _frame = 0;
    }

    public void DrawTile(SpriteBatch batch, Vector2 drawPosition)
    {
        _tileDrawer.DrawTile(FrameIndex[_frame / 50], batch, drawPosition, Color.White);
    }
}