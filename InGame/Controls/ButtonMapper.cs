using Microsoft.Xna.Framework.Input;

namespace ProjectZ.InGame.Controls;

public class ButtonMapper(Keys[] keys, Buttons[] buttons)
{
    public Keys[] Keys = keys;
    public Buttons[] Buttons = buttons;
}