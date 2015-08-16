using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MicropolisSharp.Types;
using Micropolis.Windows.Utilities;
using Microsoft.Xna.Framework;

namespace Micropolis.Windows.Layers
{
    public class SpriteLayer : ILayer
    {
        private Dictionary<SpriteType, SpriteDrawer> _spriteDrawers = new Dictionary<SpriteType, SpriteDrawer>();
        private MicropolisSharp.Micropolis _simulator;

        public Point DrawingOffset;

        public SpriteLayer(MicropolisSharp.Micropolis simulator)
        {
            _simulator = simulator;
        }

        public void LoadContent(ContentManager contentManager)
        {
            _spriteDrawers.Add(SpriteType.Airplane, new SpriteDrawer(contentManager.Load<Texture2D>("plane")));
            _spriteDrawers.Add(SpriteType.Train, new SpriteDrawer(contentManager.Load<Texture2D>("train")));
            _spriteDrawers.Add(SpriteType.Helicopter, new SpriteDrawer(contentManager.Load<Texture2D>("helicopter")));
            _spriteDrawers.Add(SpriteType.Ship, new SpriteDrawer(contentManager.Load<Texture2D>("boat")));
            _spriteDrawers.Add(SpriteType.Monster, new SpriteDrawer(contentManager.Load<Texture2D>("monster")));
            _spriteDrawers.Add(SpriteType.Tornado, new SpriteDrawer(contentManager.Load<Texture2D>("tornado")));
            _spriteDrawers.Add(SpriteType.Explosion, new SpriteDrawer(contentManager.Load<Texture2D>("explosion")));
            _spriteDrawers.Add(SpriteType.Bus, new SpriteDrawer(contentManager.Load<Texture2D>("train2")));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_simulator.SpriteList == null) { return; }
           
            foreach(SimSprite sprite in _simulator.SpriteList)
            {
                _spriteDrawers[sprite.Type].Draw(sprite, spriteBatch, DrawingOffset);
            }
        }
    }
}
