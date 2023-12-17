using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectZ.InGame.Things;

namespace ProjectZ.Base.UI;

public class UiElement(string elementId, string screen)
{
    public delegate void UiFunction(UiElement uiElement);

    public UiFunction ClickFunction;
    public UiFunction UpdateFunction;
    public UiFunction SizeUpdate;

    public SpriteFont Font = Resources.EditorFont;
    public Rectangle Rectangle;
    public Color BackgroundColor = Values.ColorUiEditor;
    public Color FontColor = new(255, 255, 255);

    public string[] Screens = screen.ToUpper().Split(':');
    public string ElementId = elementId;
    public virtual string Label { get; set; }
    public bool IsVisible = true;
    public bool Selected;
    public bool Remove;

    public virtual void Update()
    {
        // select the element if the mouse if cursor is hovering over it
        Selected = InputHandler.MouseIntersect(Rectangle);
        // call the update function of the element
        UpdateFunction?.Invoke(this);
    }

    public virtual void Draw(SpriteBatch spriteBatch) { }

    public virtual void DrawBlur(SpriteBatch spriteBatch) { }
}