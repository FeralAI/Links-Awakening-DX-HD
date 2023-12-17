using System;
using Microsoft.Xna.Framework;

namespace ProjectZ.InGame.GameObjects.Base;

public class SheetAnimation(string id, int loopCount, params AFrame[] frames)
{
    public AFrame[] Frames = frames;

    public string Id = id;
    public string NextAnimation;
    public int LoopCount = loopCount;
}

public class AFrame(int frameTimeFps, params ASprite[] sprites)
{
    public ASprite[] Sprites = sprites;

    public int FrameTime
    {
        get => (int)Math.Round(FrameTimeFps * 1000 / 60f);
        set => FrameTimeFps = (int)Math.Round(value * 60 / 1000f);
    }

    // 1 => shown for 1 frame (if the game runs with 60fps)
    // 2 => shown for 2 frames
    public int FrameTimeFps { get; set; } = frameTimeFps;
}

public class ASprite(int offsetX, int offsetY, bool mirroredV = false, bool mirroredH = false)
{
    public Point Offset = new Point(offsetX, offsetY);

    public bool MirroredV = mirroredV;
    public bool MirroredH = mirroredH;
}
