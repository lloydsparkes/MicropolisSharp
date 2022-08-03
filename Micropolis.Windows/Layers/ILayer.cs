using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Micropolis.Windows.Layers;

public interface ILayer
{
    void Draw(SpriteBatch spriteBatch);
    void LoadContent(ContentManager contentManager);
}