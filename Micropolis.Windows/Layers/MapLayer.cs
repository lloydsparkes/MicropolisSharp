using Micropolis.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Micropolis.Windows.Layers
{
    /// <summary>
    /// This class should JUST draw the main map
    /// </summary>
    public class MapLayer : ILayer
    {
        private MicropolisSharp.Micropolis _simulator;

        private TileDrawer tiles;
        private Dictionary<int, AnimatedTileDrawer> animatedTiles = new Dictionary<int, AnimatedTileDrawer>();
        private SpriteLayer _spriteLayer;

        private Point drawingOffset;

        public MapLayer(MicropolisSharp.Micropolis simulator)
        {
            _simulator = simulator;
            _spriteLayer = new SpriteLayer(simulator);
            _spriteLayer.DrawingOffset = drawingOffset;
        }

        public void LoadContent(ContentManager contentManager)
        {
            tiles = new TileDrawer(contentManager.Load<Texture2D>("tiles"));
            for (int i = 81; i < 96; i++)
            {
                var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
                foreach (var id in anim.FrameIndex)
                {
                    animatedTiles.Add(id, anim);
                }
            }

            for (int i = 145; i < 160; i++)
            {
                var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
                foreach (var id in anim.FrameIndex)
                {
                    animatedTiles.Add(id, anim);
                }
            }

            _spriteLayer.LoadContent(contentManager);
        }

        public void Resize(int width, int height)
        {

        }

        public void MoveWindow(int dx, int dy)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = drawingOffset.X; x < _simulator.Map.GetLength(0); x++)
            {
                for (int y = drawingOffset.Y; y < _simulator.Map.GetLength(1); y++)
                {
                    int tileId = _simulator.Map[x, y] & 1023;

                    if ((_simulator.Map[x, y] & 2048) == 2048 && animatedTiles.ContainsKey(tileId))
                    {
                        animatedTiles[tileId].DrawTile(spriteBatch, new Vector2((x - drawingOffset.X) * 16, (y - drawingOffset.Y) * 16));
                    }
                    else
                    {
                        tiles.DrawTile(tileId, spriteBatch, new Vector2((x - drawingOffset.X) * 16, (y - drawingOffset.Y) * 16), Color.White);
                    }
                }
            }

            //_spriteLayer.Draw(spriteBatch);
        }

        public void Update()
        {
            foreach (var a in animatedTiles.Values)
            {
                a.Update();
            }
        }
    }
}
