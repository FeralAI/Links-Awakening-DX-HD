using Microsoft.Xna.Framework;
using ProjectZ.InGame.GameObjects.Base.CObjects;

namespace ProjectZ.InGame.GameObjects.Base.Components;

public class CarriableComponent(CRectangle rectangle, CarriableComponent.InitFunction init, CarriableComponent.UpdatePositionFunction updatePosition, CarriableComponent.ThrowFunction @throw) : Component
{
    public delegate void StartFunction();
    public StartFunction StartGrabbing;

    public delegate Vector3 InitFunction();
    public InitFunction Init = init;

    public delegate bool UpdatePositionFunction(Vector3 position);
    public UpdatePositionFunction UpdatePosition = updatePosition;

    public delegate void ThrowFunction(Vector2 direction);
    public ThrowFunction Throw = @throw;
    
    public delegate bool PullFunction(Vector2 direction);
    public PullFunction Pull;

    public CRectangle Rectangle = rectangle;

    public int CarryHeight = 13;

    public bool IsHeavy;
    public bool IsPickedUp;
    public bool IsActive = true;

    public new static int Index = 3;
    public static int Mask = 0x01 << Index;
}
