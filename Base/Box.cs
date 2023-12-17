using Microsoft.Xna.Framework;

namespace ProjectZ.Base;

public struct Box(float x, float y, float z, float width, float height, float depth)
{
    public static readonly Box Empty = new();

    public float X = x;
    public float Y = y;
    public float Z = z;

    public float Width = width;
    public float Height = height;
    public float Depth = depth;

    public float Left => X;
    public float Right => X + Width;

    public float Back => Y;
    public float Front => Y + Height;

    public float Top => Z + Depth;
    public float Bottom => Z;

    public Vector2 Center => new(X + Width / 2, Y + Height / 2);

    public RectangleF Rectangle() => new(X, Y, Width, Height);

    public bool Intersects(Box value)
    {
        return value.Left < Right && Left < value.Right &&
               value.Back < Front && Back < value.Front &&
               value.Bottom < Top && Bottom < value.Top;
    }

    public bool Contains(Box value)
    {
        return Left <= value.Left && value.Right <= Right &&
               Back <= value.Back && value.Front <= Front &&
               Bottom <= value.Bottom && value.Top <= Top;
    }

    public bool Contains(Vector2 value)
    {
        return Left <= value.X && value.X <= Right &&
               Back <= value.Y && value.Y <= Front;
    }

    public static bool operator ==(Box a, Box b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z &&
               a.Width == b.Width && a.Height == b.Height && a.Depth == b.Depth;
    }

    public static bool operator !=(Box a, Box b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z &&
               a.Width != b.Width || a.Height != b.Height || a.Depth != b.Depth;
    }
}
