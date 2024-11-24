using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectZ.Base;
using ProjectZ.Base.UI;
using ProjectZ.Editor;
using ProjectZ.InGame.Controls;
using ProjectZ.InGame.GameObjects;
using ProjectZ.InGame.Map;
using ProjectZ.InGame.Pages;
using ProjectZ.InGame.SaveLoad;
using ProjectZ.InGame.Screens;
using ProjectZ.InGame.Things;

#if WINDOWS
using Forms = System.Windows.Forms;
#endif

#if DEBUG
using ProjectZ.InGame.Tests;
#endif

namespace ProjectZ
{
    public class Game1 : Game
    {
        // Membros estáticos
        public static GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch;
        public static UiManager EditorUi = new UiManager();
        public static ScreenManager ScreenManager = new ScreenManager();
        public static PageManager UiPageManager = new PageManager();
        public static GameManager GameManager = new GameManager();
        public static Language LanguageManager = new Language();
        public static GbsPlayer.GbsPlayer GbsPlayer = new GbsPlayer.GbsPlayer();
        public static StopWatchTracker StopWatchTracker = new StopWatchTracker(120);
        public static Random RandomNumber = new Random();
        public static RenderTarget2D MainRenderTarget;

        private bool _wasMinimized;
        private DoubleAverage _avgTotalMs = new DoubleAverage(30);
        private DoubleAverage _avgTimeMult = new DoubleAverage(30);
        private int _currentFrameTimeIndex;
        private double[] _debugFrameTimes = 
        {
            1000 / 30.0,
            1000 / 60.0,
            1000 / 90.0,
            1000 / 120.0,
            1000 / 144.0,
            1000 / 288.0,
            1
        };
        private string _consoleLine;
        private bool _stopConsoleThread;
        private bool _finishedLoading;
        private bool _initRenderTargets;
        private float _blurValue = 0.2f;
        private readonly SimpleFps _fpsCounter = new SimpleFps();
        private Vector2 _debugTextSize;
        private string _lastGameScreen = Values.ScreenNameGame;
        private string _lastEditorScreen = Values.ScreenNameEditor;
        private string _debugLog;
        private bool _isResizing;

        // Propriedades estáticas
        public static float GameScaleChange => gameScale / gameScaleStart;
        public static float DebugTimeScale = 1.0f;
        public static int WindowWidth;
        public static int WindowHeight;
        public static int WindowWidthEnd;
        public static int WindowHeightEnd;
        public static int ScreenScale;
        public static int UiScale;
        public static int UiRtScale;
        public static int RenderWidth;
        public static int RenderHeight;
        public static bool ScaleSettingChanged;
        public static bool FpsSettingChanged;
        public static bool DebugStepper;
        public static bool EditorMode;
        public static bool WasActive;
        public static bool UpdateGame;
        public static bool ForceDialogUpdate;
        public static bool DebugMode;
        public static bool ShowDebugText;
        public static double FreezeTime;
        public static bool LoadFirstSave;

        public Game1(bool editorMode, bool loadFirstSave)
        {
#if WINDOWS
            var windowForm = (Forms.Form)Forms.Control.FromHandle(Window.Handle);
            windowForm.Icon = Properties.Resources.Icon;
            windowForm.MinimumSize = new System.Drawing.Size(Values.MinWidth + windowForm.ClientSize.Width, Values.MinHeight + windowForm.ClientSize.Height);
#endif

            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferWidth = 1500;
            Graphics.PreferredBackBufferHeight = 1000;
            Window.AllowUserResizing = true;
            IsMouseVisible = editorMode;
            EditorMode = editorMode;
            LoadFirstSave = loadFirstSave;

            var thread = new Thread(ConsoleReaderThread);
            thread.Start();
        }

        private void ConsoleReaderThread()
        {
            while (true)
            {
                if (_stopConsoleThread)
                    return;

                if (Console.In.Peek() != -1)
                    _consoleLine = Console.ReadLine();

                Thread.Sleep(20);
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _stopConsoleThread = true;
            GbsPlayer.OnExit();
            base.OnExiting(sender, args);
        }

        protected override void LoadContent()
        {
            OnUpdateScale();
            ControlHandler.Initialize();
            SettingsSaveLoad.LoadSettings();
            GbsPlayer.LoadFile(Values.PathContentFolder + "Music/awakening.gbs");
            GbsPlayer.StartThread();
            ThreadPool.QueueUserWorkItem(LoadContentThreaded);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Components.Add(new InputHandler(this));
            Resources.LoadIntro(Graphics.GraphicsDevice, Content);
            ScreenManager.LoadIntro(Content);
            if (GameSettings.IsFullscreen)
            {
                GameSettings.IsFullscreen = false;
                ToggleFullscreen();
            }
            UpdateFpsSettings();
        }

        private void LoadContentThreaded(Object obj)
        {
            Resources.LoadTextures(Graphics.GraphicsDevice, Content);
            Resources.LoadSounds(Content);
            GameManager.Load(Content);
            GameObjectTemplates.SetUpGameObjects();
            ScreenManager.Load(Content);
            LanguageManager.Load();
            UiPageManager.Load();
            if (EditorMode)
                SetUpEditorUi();
        }

        private void UpdateConsoleInput()
        {
            if (_consoleLine == null)
                return;

            if (_consoleLine.Contains(".map"))
                SaveLoadMap.EditorLoadMap(_consoleLine, GameManager.MapManager.CurrentMap);
            else if (_consoleLine.Contains(".ani"))
            {
                var animationScreen = (AnimationScreen)ScreenManager.GetScreen(Values.ScreenNameEditorAnimation);
                animationScreen.EditorLoadAnimation(_consoleLine);
            }
            else if (_consoleLine.Contains(".png"))
            {
                var spriteAtlasScreen = (SpriteAtlasScreen)ScreenManager.GetScreen(Values.ScreenNameSpriteAtlasEditor);
                spriteAtlasScreen.LoadSpriteEditor(_consoleLine);
            }

            _consoleLine = null;
        }

        protected override void Update(GameTime gameTime)
        {
            WasActive = IsActive;
            UpdateConsoleInput();
            _fpsCounter.Update(gameTime);
            ToggleFullscreenIfNecessary();
            UpdateRenderTargets();
            UpdateScaleIfNecessary();
            ControlHandler.Update();
            HandleDebugKeys();
            HandleEditorKeys();
            UpdateGameLogic(gameTime);
            UpdateDebugInfo();
            base.Update(gameTime);
        }

        private void Draw(GameTime gameTime)
        {
            if (!_finishedLoading)
            {
                ScreenManager.Draw(SpriteBatch);
                return;
            }

            DrawMainContent();
            DrawBlurEffects();
            DrawTopLayer();
            base.Draw(gameTime);
        }

        private void DrawMainContent()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap);
            SpriteBatch.Draw(MainRenderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), Color.White);
            SpriteBatch.End();
        }

        private void DrawBlurEffects()
        {
            Resources.BlurEffectH.Parameters["pixelX"].SetValue(1.0f / _renderTarget1.Width);
            Resources.BlurEffectV.Parameters["pixelY"].SetValue(1.0f / _renderTarget1.Height);
            var mult0 = _blurValue;
            var mult1 = (1 - _blurValue * 2) / 2;
            Resources.BlurEffectH.Parameters["mult0"].SetValue(mult0);
            Resources.BlurEffectH.Parameters["mult1"].SetValue(mult1);
            Resources.BlurEffectV.Parameters["mult0"].SetValue(mult0);
            Resources.BlurEffectV.Parameters["mult1"].SetValue(mult1);

            Graphics.GraphicsDevice.SetRenderTarget(_renderTarget2);
            SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, null, null, null, null);
            SpriteBatch.Draw(MainRenderTarget, new Rectangle(0, 0, _renderTarget2.Width, _renderTarget2.Height), Color.White);
            SpriteBatch.End();

            for (var i = 0; i < 2; i++)
            {
                Graphics.GraphicsDevice.SetRenderTarget(_renderTarget1);
                SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, null, null, Resources.BlurEffectV, null);
                SpriteBatch.Draw(_renderTarget2, Vector2.Zero, Color.White);
                SpriteBatch.End();

                Graphics.GraphicsDevice.SetRenderTarget(_renderTarget2);
                SpriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, null, null, Resources.BlurEffectH, null);
                SpriteBatch.Draw(_renderTarget1, Vector2.Zero, Color.White);
                SpriteBatch.End();
            }
        }

        private void DrawTopLayer()
        {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointWrap, null, null, null, GetMatrix);
            EditorUi.Draw(SpriteBatch);
            UiPageManager.Draw(SpriteBatch);
            ScreenManager.DrawTop(SpriteBatch);
            DrawDebugText();
            DebugText = "";
#if DEBUG
            if (GameManager.SaveManager.HistoryEnabled)
                SpriteBatch.Draw(Resources.SprWhite, new Rectangle(0, WindowHeight - 6, WindowWidth, 6), Color.Red);
#endif
            SpriteBatch.End();
        }

        private void DrawDebugText()
        {
            if (!ShowDebugText)
                return;

            DebugTextBackground();
            SpriteBatch.DrawString(Resources.GameFont, DebugText, new Vector2(10), Color.White, 0, Vector2.Zero, new Vector2(2f), SpriteEffects.None, 0);
        }

        private void DebugTextBackground()
        {
            if (!ShowDebugText)
                return;

            _debugTextSize = Resources.GameFont.MeasureString(DebugText);
            SpriteBatch.Draw(_renderTarget2, new Rectangle(0, 0, (int)(_debugTextSize.X * 2) + 20, (int)(_debugTextSize.Y * 2) + 20), Color.White);
        }

        private void UpdateGameLogic(GameTime gameTime)
        {
            if (!DebugStepper)
            {
                TimeMultiplier = gameTime.ElapsedGameTime.Ticks / 166667f * DebugTimeScale;
                TotalGameTimeLast = TotalGameTime;
                if (TimeMultiplier > 2.0f)
                {
                    TimeMultiplier = 2.0f;
                    DeltaTime = (TimeMultiplier * 1000.0f) / 60.0f;
                    TotalTime += (TimeMultiplier * 1000.0) / 60.0;
                    DebugText += "\nLow Framerate";
                    if (UpdateGame)
                        TotalGameTime += (TimeMultiplier * 1000.0) / 60.0;
                }
                else
                {
                    DeltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds * DebugTimeScale;
                    TotalTime += gameTime.ElapsedGameTime.TotalMilliseconds * DebugTimeScale;
                    if (UpdateGame)
                        TotalGameTime += gameTime.ElapsedGameTime.TotalMilliseconds * DebugTimeScale;
                }
            }
            if (_finishedLoading)
            {
                if (EditorMode)
                {
                    EditorUi.Update();
                    EditorUpdate(gameTime);
                }
                EditorUi.CurrentScreen = "";
                UiPageManager.Update(gameTime);
            }
        }

        private void UpdateRenderTargets()
        {
            if (_isResizing || WindowWidthEnd == WindowWidth && WindowHeightEnd == WindowHeight)
                return;

            UpdateRenderTargetSizes(WindowWidth, WindowHeight);
            ScreenManager.OnResizeEnd(WindowWidth, WindowHeight);
        }

        private void UpdateRenderTargetSizes(int width, int height)
        {
            MainRenderTarget?.Dispose();
            MainRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
            Resources.BlurEffect.Parameters["width"].SetValue(width);
            Resources.BlurEffect.Parameters["height"].SetValue(height);
            Resources.RoundedCornerBlurEffect.Parameters["textureWidth"].SetValue(width);
            Resources.RoundedCornerBlurEffect.Parameters["textureHeight"].SetValue(height);
            _renderTarget1?.Dispose();
            _renderTarget2?.Dispose();
            _renderTarget1 = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
            _renderTarget2 = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
        }

        private void ToggleFullscreenIfNecessary()
        {
            if (InputHandler.KeyDown(Keys.LeftAlt) && InputHandler.KeyPressed(Keys.Enter))
            {
                ToggleFullscreen();
                InputHandler.ResetInputState();
                SettingsSaveLoad.SaveSettings();
            }
        }

        private void UpdateScaleIfNecessary()
        {
            if (ScaleSettingChanged)
            {
                ScaleSettingChanged = false;
                OnUpdateScale();
            }
        }

        private void HandleDebugKeys()
        {
            if (!DebugStepper)
                return;

            if (InputHandler.KeyPressed(Keys.M))
            {
                TimeMultiplier = TargetElapsedTime.Ticks / 166667f;
                DeltaTime = (float)TargetElapsedTime.TotalMilliseconds;
                TotalGameTimeLast = TotalTime;
                TotalTime += TargetElapsedTime.Milliseconds;
                TotalGameTime += TargetElapsedTime.Milliseconds;
            }
        }

        private void HandleEditorKeys()
        {
            if (InputHandler.KeyPressed(Keys.N))
                DebugStepper = !DebugStepper;
            if (ScreenManager.CurrentScreenId != Values.ScreenNameGame)
                DebugStepper = false;

            if (InputHandler.KeyPressed(Keys.Q))
                GameManager.MapManager.ReloadMap();

            if (InputHandler.KeyPressed(Keys.Add))
                DebugTimeScale += 0.125f;
            if (InputHandler.KeyPressed(Keys.Subtract) && DebugTimeScale > 0)
                DebugTimeScale -= 0.125f;

            if (InputHandler.KeyPressed(Values.DebugShadowKey))
                GameSettings.EnableShadows = !GameSettings.EnableShadows;

            if (ScreenManager.CurrentScreenId != Values.ScreenNameEditor &&
                ScreenManager.CurrentScreenId != Values.ScreenNameEditorTileset &&
                ScreenManager.CurrentScreenId != Values.ScreenNameEditorTilesetExtractor &&
                ScreenManager.CurrentScreenId != Values.ScreenNameEditorAnimation &&
                ScreenManager.CurrentScreenId != Values.ScreenNameSpriteAtlasEditor)
            {
                if (InputHandler.KeyPressed(Keys.D0))
                    TriggerFpsSettings();

                if (InputHandler.KeyPressed(Keys.D1))
                {
                    _currentFrameTimeIndex--;
                    if (_currentFrameTimeIndex < 0)
                        _currentFrameTimeIndex = _debugFrameTimes.Length - 1;
                    TargetElapsedTime = new TimeSpan((long)Math.Ceiling(_debugFrameTimes[_currentFrameTimeIndex] * 10000));
                }

                if (InputHandler.KeyPressed(Keys.D2))
                {
                    _currentFrameTimeIndex = (_currentFrameTimeIndex + 1) % _debugFrameTimes.Length;
                    TargetElapsedTime = new TimeSpan((long)Math.Ceiling(_debugFrameTimes[_currentFrameTimeIndex] * 10000));
                }
            }

            if (InputHandler.KeyPressed(Keys.Escape) || InputHandler.KeyPressed(Keys.OemPeriod))
            {
                if (ScreenManager.CurrentScreenId != Values.ScreenNameEditor &&
                    ScreenManager.CurrentScreenId != Values.ScreenNameEditorTileset &&
                    ScreenManager.CurrentScreenId != Values.ScreenNameEditorTilesetExtractor &&
                    ScreenManager.CurrentScreenId != Values.ScreenNameEditorAnimation &&
                    ScreenManager.CurrentScreenId != Values.ScreenNameSpriteAtlasEditor)
                {
                    UiPageManager.PopAllPages(PageManager.TransitionAnimation.TopToBottom, PageManager.TransitionAnimation.TopToBottom);
                    _lastGameScreen = ScreenManager.CurrentScreenId;
                    ScreenManager.ChangeScreen(_lastEditorScreen);
                }
                else
                {
                    _lastEditorScreen = ScreenManager.CurrentScreenId;
                    ScreenManager.ChangeScreen(_lastGameScreen);
                    var editorScreen = (MapEditorScreen)ScreenManager.GetScreen(Values.ScreenNameEditor);
                    if (_lastEditorScreen == Values.ScreenNameEditor)
                        MapManager.ObjLink.SetPosition(new Vector2(
                            editorScreen.MousePixelPosition.X,
                            editorScreen.MousePixelPosition.Y));
                }
            }

            if (InputHandler.KeyPressed(Values.DebugToggleDebugModeKey))
                DebugMode = !DebugMode;

            if (InputHandler.KeyPressed(Values.DebugBox))
                DebugBoxMode = (DebugBoxMode + 1) % 6;

            if (InputHandler.KeyPressed(Values.DebugSaveKey))
            {
                MapManager.ObjLink.SaveMap = GameManager.MapManager.CurrentMap.MapName;
                MapManager.ObjLink.SavePosition = MapManager.ObjLink.EntityPosition.Position;
                MapManager.ObjLink.SaveDirection = MapManager.ObjLink.Direction;
                SaveGameSaveLoad.SaveGame(GameManager);
                GameManager.InGameOverlay.InGameHud.ShowSaveIcon();
            }
            if (InputHandler.KeyPressed(Values.DebugLoadKey))
                GameManager.LoadSaveFile(GameManager.SaveSlot);

            if (InputHandler.KeyDown(Keys.H))
                _debugLog += "\n" + DebugText;
#if WINDOWS
            else if (InputHandler.KeyReleased(Keys.H))
                Forms.Clipboard.SetText(_debugLog);
#endif
        }

        private void UpdateDebugInfo()
        {
            DebugText += _fpsCounter.Msg;
            _avgTotalMs.AddValue(gameTime.ElapsedGameTime.TotalMilliseconds);
            _avgTimeMult.AddValue(TimeMultiplier);
            DebugText += $"\ntotal ms:      {_avgTotalMs.Average,6:N3}"
                         + $"\ntime mult:     {_avgTimeMult.Average,6:N3}"
                         + $"\ntime scale:    {DebugTimeScale}"
                         + $"\ntime:          {TotalGameTime}";
            DebugText += "\nHistory Enabled: " + GameManager.SaveManager.HistoryEnabled + "\n";
        }

        private void ToggleFullscreen()
        {
#if WINDOWS
            GameSettings.IsFullscreen = !GameSettings.IsFullscreen;
            var screenBounds = System.Windows.Forms.Screen.GetBounds(Window.Handle);
            if (!GameSettings.BorderlessWindowed)
            {
                if (!Graphics.IsFullScreen)
                {
                    _lastWindowWidth = Graphics.PreferredBackBufferWidth;
                    _lastWindowHeight = Graphics.PreferredBackBufferHeight;
                    _lastWindowRestoreBounds = Window.RestoreBounds;
                    Graphics.PreferredBackBufferWidth = screenBounds.Width;
                    Graphics.PreferredBackBufferHeight = screenBounds.Height;
                    _lastWindowState = Window.WindowState;
                }
                else
                {
                    if (_lastWindowState != Forms.FormWindowState.Maximized)
                    {
                        Graphics.PreferredBackBufferWidth = _lastWindowWidth;
                        Graphics.PreferredBackBufferHeight = _lastWindowHeight;
                    }
                }
                Graphics.ToggleFullScreen();
                if (_lastWindowState == Forms.FormWindowState.Maximized)
                {
                    Window.Bounds = _lastWindowRestoreBounds;
                    Window.WindowState = _lastWindowState;
                }
            }
            else
            {
                _isFullscreen = !_isFullscreen;
                if (_isFullscreen)
                {
                    _lastWindowState = Window.WindowState;
                    _lastWindowBounds = Window.Bounds;
                    Window.FormBorderStyle = Forms.FormBorderStyle.None;
                    Window.WindowState = Forms.FormWindowState.Normal;
                    Window.Bounds = screenBounds;
                }
                else
                {
                    Window.FormBorderStyle = Forms.FormBorderStyle.Sizable;
                    if (_lastWindowState == Forms.FormWindowState.Maximized)
                    {
                        Window.Bounds = _lastWindowRestoreBounds;
                        Window.WindowState = _lastWindowState;
                    }
                    else
                    {
                        Window.WindowState = _lastWindowState;
                        Window.Bounds = _lastWindowBounds;
                    }
                }
            }
#endif
        }

        private void SetUpEditorUi()
        {
            var strScreen = $"{Values.EditorUiObjectEditor}:{Values.EditorUiObjectSelection}:{Values.EditorUiTileEditor}:{Values.EditorUiTileSelection}:{Values.EditorUiDigTileEditor}:{Values.EditorUiMusicTileEditor}:{Values.EditorUiTileExtractor}:{Values.EditorUiTilesetEditor}:{Values.EditorUiAnimation}:{Values.EditorUiSpriteAtlas}";
            EditorUi.AddElement(new UiRectangle(new Rectangle(0, 0, WindowWidth, Values.ToolBarHeight), "top", strScreen, Values.ColorBackgroundDark, Color.White, ui => { ui.Rectangle = new Rectangle(0, 0, WindowWidth, Values.ToolBarHeight); }));
            var pos = 0;
            EditorUi.AddElement(new UiButton(new Rectangle(0, 0, 200, Values.ToolBarHeight), Resources.EditorFont, "Editor", "bt1", strScreen, ui => { ((UiButton)ui).Marked = ScreenManager.CurrentScreenId == Values.ScreenNameEditor; }, element => { ScreenManager.ChangeScreen(Values.ScreenNameEditor); }));
            EditorUi.AddElement(new UiButton(new Rectangle(pos += 205, 0, 200, Values.ToolBarHeight), Resources.EditorFont, "Tileset Editor", "bt1", strScreen, ui => { ((UiButton)ui).Marked = ScreenManager.CurrentScreenId == Values.ScreenNameEditorTileset; }, element => { ScreenManager.ChangeScreen(Values.ScreenNameEditorTileset); }));
            EditorUi.AddElement(new UiButton(new Rectangle(pos += 205, 0, 200, Values.ToolBarHeight), Resources.EditorFont, "Tileset Extractor", "bt1", strScreen, ui => { ((UiButton)ui).Marked = ScreenManager.CurrentScreenId == Values.ScreenNameEditorTilesetExtractor; }, element => { ScreenManager.ChangeScreen(Values.ScreenNameEditorTilesetExtractor); }));
            EditorUi.AddElement(new UiButton(new Rectangle(pos += 205, 0, 200, Values.ToolBarHeight), Resources.EditorFont, "Animation Editor", "bt1", strScreen, ui => { ((UiButton)ui).Marked = ScreenManager.CurrentScreenId == Values.ScreenNameEditorAnimation; }, element => { ScreenManager.ChangeScreen(Values.ScreenNameEditorAnimation); }));
            EditorUi.AddElement(new UiButton(new Rectangle(pos += 205, 0, 200, Values.ToolBarHeight), Resources.EditorFont, "Sprite Atlas Editor", "bt1", strScreen, ui => { ((UiButton)ui).Marked = ScreenManager.CurrentScreenId == Values.ScreenNameSpriteAtlasEditor; }, element => { ScreenManager.ChangeScreen(Values.ScreenNameSpriteAtlasEditor); }));
        }

        private void EditorUpdate(GameTime gameTime)
        {
            // Implementação do método EditorUpdate
        }

        private void UpdateFpsSettings()
        {
            IsFixedTimeStep = false;
            Graphics.SynchronizeWithVerticalRetrace = GameSettings.LockFps;
            Graphics.ApplyChanges();
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            _isResizing = true;
            gameScaleStart = gameScale;
        }

        private void OnResize(object sender, EventArgs e)
        {
#if WINDOWS
            if (_isFullscreen && Window.WindowState == Forms.FormWindowState.Maximized)
                _lastWindowRestoreBounds = Window.RestoreBounds;
            if (!GameSettings.BorderlessWindowed && Graphics.IsFullScreen && Window.WindowState == Forms.FormWindowState.Minimized && !_wasMinimized)
            {
                _wasMinimized = true;
                Graphics.ToggleFullScreen();
                Window.WindowState = Forms.FormWindowState.Minimized;
            }
            if (!GameSettings.BorderlessWindowed && Window.WindowState == Forms.FormWindowState.Normal && _wasMinimized)
            {
                _wasMinimized = false;
                ToggleFullscreen();
            }
#endif
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            _isResizing = false;
            gameScaleStart = gameScale;
        }

        private void OnUpdateScale()
        {
            ScreenScale = MathHelper.Clamp(Math.Min(WindowWidth / Values.MinWidth, WindowHeight / Values.MinHeight), 1, 25);
            gameScale = MathHelper.Clamp(Math.Min(WindowWidth / (float)Values.MinWidth, WindowHeight / (float)Values.MinHeight), 1, 25);
            MapManager.Camera.Scale = GameSettings.GameScale == 11 ? MathF.Ceiling(gameScale) : GameSettings.GameScale;
            if (MapManager.Camera.Scale < 1)
            {
                MapManager.Camera.Scale = 1 / (2 - MapManager.Camera.Scale);
                GameManager.SetGameScale(1);
            }
            else
            {
                GameManager.SetGameScale(GameSettings.GameScale == 11 ? gameScale : GameSettings.GameScale);
            }
            UiScale = GameSettings.UiScale == 0 ? ScreenScale : MathHelper.Clamp(GameSettings.UiScale, 1, ScreenScale);
            EditorUi.SizeChanged();
            ScreenManager.OnResize(WindowWidth, WindowHeight);
        }

        private void UpdateRenderTargetSizes(int width, int height)
        {
            MainRenderTarget?.Dispose();
            MainRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
            Resources.BlurEffect.Parameters["width"].SetValue(width);
            Resources.BlurEffect.Parameters["height"].SetValue(height);
            Resources.RoundedCornerBlurEffect.Parameters["textureWidth"].SetValue(width);
            Resources.RoundedCornerBlurEffect.Parameters["textureHeight"].SetValue(height);
            _renderTarget1?.Dispose();
            _renderTarget2?.Dispose();
            _renderTarget1 = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
            _renderTarget2 = new RenderTarget2D(Graphics.GraphicsDevice, width, height);
        }
    }
}
