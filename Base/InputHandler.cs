using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProjectZ.Base;

#region InputCharacter

internal class InputCharacter(string upper, string lower, string alt, Keys code)
{
    private readonly string _upper = upper;
    private readonly string _lower = lower;
    private readonly string _alt = alt;

    private readonly Keys _code = code;

    public InputCharacter(string upper, string lower, Keys code) :
        this(upper, lower, lower, code)
    { }

    public string ReturnCharacter(bool shiftDown, bool altDown)
    {
        return altDown ? _alt : shiftDown ? _upper : _lower;
    }

    public Keys ReturnKey()
    {
        return _code;
    }
}
#endregion

internal class InputHandler : GameComponent
{
    private static List<InputCharacter> _alphabet;
    
    public static KeyboardState KeyboardState => _keyboardState;

    public static KeyboardState LastKeyboardState => _lastKeyboardState;

    public static MouseState MouseState => _mouseState;

    public static MouseState LastMousState => _lastMouseState;

    private static KeyboardState _keyboardState;
    private static KeyboardState _lastKeyboardState;

    private static MouseState _mouseState;
    private static MouseState _lastMouseState;

    private static GamePadState _gamePadState;
    private static GamePadState _lastGamePadState;
    private static readonly float _gamePadAccuracy = 0.2f;
    
    #region Constructor Region

    public InputHandler(Game game)
        : base(game)
    {
        _keyboardState = Keyboard.GetState();
        _mouseState = Mouse.GetState();

        _alphabet =
        [
            // TODO_End: Important! Replace this method to support different keyboard layouts
            /* Alphabet. */
            new InputCharacter("A", "a", Keys.A),
            new InputCharacter("B", "b", Keys.B),
            new InputCharacter("C", "c", Keys.C),
            new InputCharacter("D", "d", Keys.D),
            new InputCharacter("E", "e", "€", Keys.E),
            new InputCharacter("F", "f", Keys.F),
            new InputCharacter("G", "g", Keys.G),
            new InputCharacter("H", "h", Keys.H),
            new InputCharacter("I", "i", Keys.I),
            new InputCharacter("J", "j", Keys.J),
            new InputCharacter("K", "k", Keys.K),
            new InputCharacter("L", "l", Keys.L),
            new InputCharacter("M", "m", "µ", Keys.M),
            new InputCharacter("N", "n", Keys.N),
            new InputCharacter("O", "o", Keys.O),
            new InputCharacter("P", "p", Keys.P),
            new InputCharacter("Q", "q", "@", Keys.Q),
            new InputCharacter("R", "r", Keys.R),
            new InputCharacter("S", "s", Keys.S),
            new InputCharacter("T", "t", Keys.T),
            new InputCharacter("U", "u", Keys.U),
            new InputCharacter("V", "v", Keys.V),
            new InputCharacter("W", "w", Keys.W),
            new InputCharacter("X", "x", Keys.X),
            new InputCharacter("Y", "y", Keys.Y),
            new InputCharacter("Z", "z", Keys.Z),
            /* Dezimalzahlen. */
            new InputCharacter("!", "1", Keys.D1),
            new InputCharacter("\"", "2", "²", Keys.D2),
            new InputCharacter("§", "3", "³", Keys.D3),
            new InputCharacter("$", "4", Keys.D4),
            new InputCharacter("%", "5", Keys.D5),
            new InputCharacter("&", "6", "|", Keys.D6),
            new InputCharacter("/", "7", "{", Keys.D7),
            new InputCharacter("(", "8", "[", Keys.D8),
            new InputCharacter(")", "9", "]", Keys.D9),
            new InputCharacter("=", "0", "}", Keys.D0),
            /* Sonderelemente. */
            new InputCharacter(" ", " ", Keys.Space),
            //InputHandler.alphabet.Add(new InputCharacter("Ü", "ü", Keys.OemSemicolon));
            //InputHandler.alphabet.Add(new InputCharacter("Ö", "ö", Keys.OemTilde));
            //InputHandler.alphabet.Add(new InputCharacter("Ä", "ä", Keys.OemQuotes));
            new InputCharacter(";", ",", Keys.OemComma),
            new InputCharacter("*", "+", "~", Keys.OemPlus),
            new InputCharacter("'", "#", Keys.OemQuestion),
            new InputCharacter(":", ".", Keys.OemPeriod),
            new InputCharacter("_", "-", Keys.OemMinus),
            new InputCharacter("?", "", Keys.OemOpenBrackets),
            new InputCharacter(">", "<", "|", Keys.OemBackslash),
        ];
        //InputHandler.alphabet.Add(new InputCharacter("`", "´", Keys.OemCloseBrackets));

        //InputHandler.alphabet.Add(new InputCharacter("°", "^", Keys.OemPipe));
    }

    #endregion

    public override void Update(GameTime gameTime)
    {
        _lastKeyboardState = _keyboardState;
        _keyboardState = Keyboard.GetState();

        _lastMouseState = _mouseState;
        _mouseState = Mouse.GetState();

        _lastGamePadState = _gamePadState;
        _gamePadState = GamePad.GetState(0);

        // if the game was not active the last mousestate is uninteresting
        if (!Game1.WasActive)
            ResetInputState();
    }

    /// <summary>
    /// set the last input state to the current state
    /// </summary>
    public static void ResetInputState()
    {
        _lastKeyboardState = _keyboardState;
        _lastMouseState = _mouseState;
        _lastGamePadState = _gamePadState;
    }

    public static bool LastKeyDown(Keys key)
    {
        return _lastKeyboardState.IsKeyDown(key);
    }

    public static bool KeyDown(Keys key)
    {
        return _keyboardState.IsKeyDown(key);
    }

    public static bool KeyPressed(Keys key)
    {
        return _keyboardState.IsKeyDown(key) &&
            _lastKeyboardState.IsKeyUp(key);
    }

    public static bool KeyReleased(Keys key)
    {
        return _keyboardState.IsKeyUp(key) &&
            _lastKeyboardState.IsKeyDown(key);
    }

    public static List<Keys> GetPressedKeys()
    {
        var pressedKeys = new List<Keys>();
        var downKeys = _keyboardState.GetPressedKeys();

        for (var i = 0; i < downKeys.Length; i++)
        {
            if (KeyPressed(downKeys[i]))
                pressedKeys.Add(downKeys[i]);
        }

        return pressedKeys;
    }

    public static List<Buttons> GetPressedButtons()
    {
        var pressedKeys = new List<Buttons>();

        foreach (Buttons button in Enum.GetValues(typeof(Buttons)))
        {
            if (GamePadPressed(button))
                pressedKeys.Add(button);
        }

        return pressedKeys;
    }

    public static bool LastGamePadDown(Buttons button)
    {
        return _lastGamePadState.IsButtonDown(button);
    }

    public static bool GamePadDown(Buttons button)
    {
        return _gamePadState.IsButtonDown(button);
    }

    public static bool GamePadPressed(Buttons button)
    {
        return _gamePadState.IsButtonDown(button) &&
            _lastGamePadState.IsButtonUp(button);
    }

    public static bool GamePadReleased(Buttons button)
    {
        return _gamePadState.IsButtonUp(button) &&
            _lastGamePadState.IsButtonDown(button);
    }


    public static bool GamePadLeftStick(Vector2 dir)
    {
        return ((dir.X < 0 && _gamePadState.ThumbSticks.Left.X < -_gamePadAccuracy) || (dir.X > 0 && _gamePadState.ThumbSticks.Left.X > _gamePadAccuracy) ||
            (dir.Y < 0 && _gamePadState.ThumbSticks.Left.Y < -_gamePadAccuracy) || (dir.Y > 0 && _gamePadState.ThumbSticks.Left.Y > _gamePadAccuracy));
    }
    public static bool LastGamePadLeftStick(Vector2 dir)
    {
        return ((dir.X < 0 && _lastGamePadState.ThumbSticks.Left.X < -_gamePadAccuracy) || (dir.X > 0 && _lastGamePadState.ThumbSticks.Left.X > _gamePadAccuracy) ||
            (dir.Y < 0 && _lastGamePadState.ThumbSticks.Left.Y < -_gamePadAccuracy) || (dir.Y > 0 && _lastGamePadState.ThumbSticks.Left.Y > _gamePadAccuracy));
    }

    public static bool GamePadRightStick(Vector2 dir)
    {
        return ((dir.X < 0 && _gamePadState.ThumbSticks.Right.X < -_gamePadAccuracy) || (dir.X > 0 && _gamePadState.ThumbSticks.Right.X > _gamePadAccuracy) ||
                (dir.Y < 0 && _gamePadState.ThumbSticks.Right.Y < -_gamePadAccuracy) || (dir.Y > 0 && _gamePadState.ThumbSticks.Right.Y > _gamePadAccuracy));
    }

    #region Mouse Region

    //scroll
    public static bool MouseWheelUp()
    {
        return _mouseState.ScrollWheelValue > _lastMouseState.ScrollWheelValue;
    }
    public static bool MouseWheelDown()
    {
        return _mouseState.ScrollWheelValue < _lastMouseState.ScrollWheelValue;
    }

    //down
    public static bool MouseLeftDown()
    {
        return _mouseState.LeftButton == ButtonState.Pressed;
    }
    public static bool MouseLeftDown(Rectangle rectangle)
    {
        return MouseIntersect(rectangle) && MouseLeftDown();
    }
    public static bool MouseRightDown()
    {
        return _mouseState.RightButton == ButtonState.Pressed;
    }
    public static bool MouseMiddleDown()
    {
        return _mouseState.MiddleButton == ButtonState.Pressed;
    }

    //start
    public static bool MouseLeftStart()
    {
        return _mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
    }
    public static bool MouseRightStart()
    {
        return _mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
    }
    public static bool MouseMiddleStart()
    {
        return _mouseState.MiddleButton == ButtonState.Pressed && _lastMouseState.MiddleButton == ButtonState.Released;
    }

    //released
    public static bool MouseLeftReleased()
    {
        return _mouseState.LeftButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Pressed;
    }
    public static bool MouseRightReleased()
    {
        return _mouseState.RightButton == ButtonState.Released && _lastMouseState.RightButton == ButtonState.Pressed;
    }

    //pressed
    public static bool MouseLeftPressed()
    {
        return _mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
    }
    public static bool MouseLeftPressed(Rectangle rectangle)
    {
        return rectangle.Contains(MousePosition()) && MouseLeftPressed();
    }

    public static bool MouseRightPressed()
    {
        return _mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released;
    }
    public static bool MouseRightPressed(Rectangle rectangle)
    {
        return MouseIntersect(rectangle) && MouseRightPressed();
    }

    public static bool MouseIntersect(Rectangle rectangle)
    {
        return rectangle.Contains(MousePosition());
    }

    public static Point MousePosition()
    {
        return _mouseState.Position;
    }
    public static Point LastMousePosition()
    {
        return _lastMouseState.Position;
    }

    #endregion

    #region return text + return number

    /// <summary>
    /// returns the pressed keys if they are in the InputHandler.alphabet
    /// only returns one key at a time
    /// </summary>
    /// <returns></returns>
    public static string ReturnCharacter()
    {
        var shiftDown = _keyboardState.IsKeyDown(Keys.LeftShift) || _keyboardState.IsKeyDown(Keys.RightShift);
        var altDown = _keyboardState.IsKeyDown(Keys.LeftAlt) || _keyboardState.IsKeyDown(Keys.RightAlt);

        //var pressedKeys = _keyboardState.GetPressedKeys();

        foreach (var character in _alphabet)
        {
            if (KeyPressed(character.ReturnKey()))
                return character.ReturnCharacter(shiftDown, altDown);
        }

        return "";
    }

    /// <summary>
    /// returns pressed number from d0-d9 and numpad0-numpad9
    /// </summary>
    /// <returns></returns>
    public static int ReturnNumber()
    {
        for (var i = 0; i < 10; i++)
            if (KeyPressed(Keys.D0 + i) || KeyPressed(Keys.NumPad0 + i))
                return i;

        return -1;
    }
    #endregion
}
