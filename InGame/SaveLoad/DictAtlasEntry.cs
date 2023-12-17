using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectZ.InGame.SaveLoad;

public class DictAtlasEntry(Texture2D texture, Rectangle sourceRectangle, Vector2 origin, int textureScale)
{
    public readonly Texture2D Texture = texture;

    public readonly Rectangle SourceRectangle = sourceRectangle;
    public readonly Rectangle ScaledRectangle = new Rectangle(
            sourceRectangle.X * textureScale, sourceRectangle.Y * textureScale,
            sourceRectangle.Width * textureScale, sourceRectangle.Height * textureScale);
    public readonly Vector2 Origin = origin;
    public readonly Vector2 ScaledOrigin = new Vector2(origin.X * textureScale, origin.Y * textureScale);

    public readonly int TextureScale = textureScale;
    public readonly float Scale = 1.0f / textureScale;
}
