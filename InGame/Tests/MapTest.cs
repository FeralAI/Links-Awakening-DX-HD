using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ProjectZ.Base;
using ProjectZ.InGame.GameObjects.Base;
using ProjectZ.InGame.GameObjects.Dungeon;
using ProjectZ.InGame.GameObjects.Things;
using ProjectZ.InGame.Map;
using ProjectZ.InGame.SaveLoad;
using ProjectZ.InGame.Things;

namespace ProjectZ.InGame.Tests;

public class MapTest
{
    private readonly Dictionary<string, int> _keyList = [];

    private readonly List<string> _doorSaveList = [];
    private readonly List<string> _doorList = [];

    private readonly List<string> _mapList = [];
    private const int StartIndex = 5;
    private int _currentMapIndex = StartIndex;

    private Vector2 _cameraPosition;

    private float _counter;
    private readonly float ChangeTime = 25;
    private readonly bool _hasDug;
    private readonly bool _hasBombed;
    private readonly bool _hasSpawnedBalls;

    private bool _isRunning;
    private readonly bool _paused = true;

    public MapTest()
    {
        var mapPaths = Directory.GetFiles(Values.PathMapsFolder);

        for (var i = 0; i < mapPaths.Length; i++)
        {
            if (mapPaths[i].EndsWith(".map") && !mapPaths[i].Contains("test map"))
            {
                _mapList.Add(mapPaths[i]);
            }
        }
    }

    private void Start()
    {
        _isRunning = true;
        _counter = ChangeTime;

        Game1.ScreenManager.ChangeScreen(Values.ScreenNameGame);

        LoadMap(_mapList[_currentMapIndex]);
    }

    public void Update()
    {
        if (InputHandler.KeyPressed(Keys.V))
            SpawnCurrentTester();
        if (InputHandler.KeyPressed(Keys.B))
            SpawnBalls();

        return;
    }

    private void OffsetMap(int offset)
    {
        _currentMapIndex = (_currentMapIndex + offset) % _mapList.Count;
        if (_currentMapIndex < 0)
            _currentMapIndex += _mapList.Count;

        LoadMap(_mapList[_currentMapIndex]);
    }

    private bool UpdateView()
    {
        _cameraPosition.X += 160;
        if (_cameraPosition.X > Game1.GameManager.MapManager.CurrentMap.MapWidth * Values.TileSize)
        {
            _cameraPosition.X = 80;
            _cameraPosition.Y += 128;
        }

        if (_cameraPosition.Y - 64 > Game1.GameManager.MapManager.CurrentMap.MapHeight * Values.TileSize)
            return false;

        Game1.GameManager.MapManager.CurrentMap.CameraTarget = _cameraPosition;
        MapManager.Camera.ForceUpdate(Game1.GameManager.MapManager.GetCameraTarget());

        return true;
    }

    private void LoadMap(string path)
    {
        if (_currentMapIndex == StartIndex)
        {
            _keyList.Clear();
            _doorSaveList.Clear();
            _doorList.Clear();
        }

        var mapFileName = Path.GetFileName(path);

        // load the map file
        SaveLoadMap.LoadMap(mapFileName, Game1.GameManager.MapManager.NextMap);

        // create the objects
        Game1.GameManager.MapManager.NextMap.Objects.LoadObjects();

        var oldMap = Game1.GameManager.MapManager.CurrentMap;
        Game1.GameManager.MapManager.CurrentMap = Game1.GameManager.MapManager.NextMap;
        Game1.GameManager.MapManager.NextMap = oldMap;

        // center the camera
        _cameraPosition = new Vector2(
            80 + Game1.GameManager.MapManager.CurrentMap.MapOffsetX * Values.TileSize,
            64 + Game1.GameManager.MapManager.CurrentMap.MapOffsetY * Values.TileSize);
        Game1.GameManager.MapManager.CurrentMap.CameraTarget = _cameraPosition;
        MapManager.Camera.ForceUpdate(Game1.GameManager.MapManager.GetCameraTarget());

        //CheckMusic();

        GetDoorList();

        //CheckKeys();
    }

    private void GetDoorList()
    {
        var doors = new List<GameObject>();
        Game1.GameManager.MapManager.CurrentMap.Objects.GetObjectsOfType(doors, typeof(ObjDoor), 0, 0,
            Game1.GameManager.MapManager.CurrentMap.MapWidth * Values.TileSize,
            Game1.GameManager.MapManager.CurrentMap.MapHeight * Values.TileSize);
        foreach (var door in doors)
        {
            var doorObj = ((ObjDoor)door);
            if (doorObj._savePosition && doorObj._entryId != null)
            {
                _doorSaveList.Add(Game1.GameManager.MapManager.CurrentMap.MapName + " : " + doorObj._entryId);
            }
            if (!doorObj._savePosition && doorObj._entryId != null)
            {
                _doorList.Add(Game1.GameManager.MapManager.CurrentMap.MapName + " : " + doorObj._entryId);
            }
        }
    }

    private void SpawnBalls()
    {
        for (var i = 0; i < 100; i++)
        {
            var ball = new ObjTestObject(Game1.GameManager.MapManager.CurrentMap, (int)_cameraPosition.X, (int)_cameraPosition.Y);
            Game1.GameManager.MapManager.CurrentMap.Objects.SpawnObject(ball);
        }
    }

    private void SpawnCurrentTester()
    {
        for (var y = -10; y < 10; y++)
            for (var x = -10; x < 10; x++)
            {
                var posX = (int)(MapManager.ObjLink.EntityPosition.X / 16) * 16 + 8;
                var posY = (int)(MapManager.ObjLink.EntityPosition.Y / 16) * 16 + 8;
                var ball = new ObjWaterCurrentTester(Game1.GameManager.MapManager.CurrentMap, posX + x * 16, posY + y * 16);
                Game1.GameManager.MapManager.CurrentMap.Objects.SpawnObject(ball);
            }
    }
}
