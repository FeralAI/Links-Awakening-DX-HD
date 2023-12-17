﻿using Microsoft.Xna.Framework.Graphics;
using ProjectZ.InGame.Things;

namespace ProjectZ.InGame.GameObjects.Base.Components;

public class BlurDrawComponent : Component
{
    public delegate void DrawTemplate(SpriteBatch spriteBatch);
    public DrawTemplate Draw;

    public new static int Index = 16;
    public static int Mask = 0x01 << Index;

    public int Layer = Values.LightLayer0;

    protected BlurDrawComponent() { }
    
    public BlurDrawComponent(DrawTemplate draw)
    {
        Draw = draw;
    }
}
