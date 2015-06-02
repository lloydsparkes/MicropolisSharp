using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micropolis.Basic
{
    public class AnimatedTileDrawer
    {
        private TileDrawer _tileDrawer;
        private int _frame = 0;
        private int _frameCount = 0;
        private int[] _frameIndex;

        public int[] FrameIndex { get { return _frameIndex; } }

        public AnimatedTileDrawer(TileDrawer tileDrawer, int startingTileId, int frameOffset, int frames)
        {
            _tileDrawer = tileDrawer;
            _frameCount = (frames - 1) * 50;
            List<int> index = new List<int>();
            for (int i = 0; i < frames; i++)
            {
                index.Add(startingTileId + (frameOffset * i));
            }
            _frameIndex = index.ToArray();
        }

        public void Update()
        {
            _frame++;
            if (_frame > _frameCount)
            {
                _frame = 0;
            }
        }

        public void DrawTile(SpriteBatch batch, Vector2 drawPosition)
        {
            _tileDrawer.DrawTile(_frameIndex[_frame/50], batch, drawPosition, Color.White);
        }


    }
}
