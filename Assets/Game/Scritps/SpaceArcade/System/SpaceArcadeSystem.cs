using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

public class SpaceArcadeSystem : ISpaceArcadeSystem, ITickable
{
    private ISceneSystem _sceneSystem;
    private ICameraSystem _cameraSystem;
    private ISpaceArcadePlayerShipSystem _playerShipSystem;
    private ISpaceArcadeMoveSystem _moveSystem;
    private IMeteorSystem _meteorSystem;
    private IFireBulletSystem _fireBulletSystem;

    private ISceneHandler _arcadeScene;
    private SpaceArcadeController _spaceArcadeScreen;

    private const string SCENE_ASSET_NAME = "Game/Scenes/SpaceArcade";
    private const float GAME_SPEED = 5f;

    private Rect _gameRect;
    private bool _isInitialized = false;

    [Inject]
    private void Construct(
        ISceneSystem sceneSystem,
        ICameraSystem cameraSystem,
        ISpaceArcadePlayerShipSystem playerShipSystem,
        ISpaceArcadeMoveSystem moveSystem,
        IMeteorSystem meteorSystem,
        IFireBulletSystem fireBulletSystem
        )
    {
        _sceneSystem = sceneSystem;
        _cameraSystem = cameraSystem;
        _playerShipSystem = playerShipSystem;
        _moveSystem = moveSystem;
        _meteorSystem = meteorSystem;
        _fireBulletSystem = fireBulletSystem;
    }

    public async UniTask Show()
    {
        if(_isInitialized)
        {
            return;
        }

        var loadedScene = await _sceneSystem.LoadScene(SCENE_ASSET_NAME);
        _arcadeScene = loadedScene;

        var spaceArcadeScreen = _arcadeScene.GetComponent<SpaceArcadeController>();
        _spaceArcadeScreen = spaceArcadeScreen;
        InitializeGameRect(_spaceArcadeScreen);

        _moveSystem.Initialize(GAME_SPEED);
        await _meteorSystem.Initialize(_gameRect);
        _fireBulletSystem.Initialize(_gameRect);

        await _playerShipSystem.Initialize(_spaceArcadeScreen.PlayerShipRoot, _gameRect.width);
        await _playerShipSystem.SpawnShip();

        _cameraSystem.SetCamera(spaceArcadeScreen.GameCamera);
        _isInitialized = true;
    } 

    public async UniTask Close()
    {
        if(!_isInitialized)
        {
            return;
        }

        _isInitialized = false;
        _cameraSystem.ResetCamera();
        _moveSystem.Deinitialize();

        _playerShipSystem.Deinitialize();
        _fireBulletSystem.Deinitialize();
        _meteorSystem.Deinitialize();

        _gameRect = default;
        await _sceneSystem.UnloadScene(_arcadeScene);
    }

    public void Tick()
    {
        if (!_isInitialized)
        {
            return;
        }

        var deltaTime = Time.deltaTime;

        _playerShipSystem.Update(deltaTime);
        _fireBulletSystem.Update(deltaTime);
        _meteorSystem.Update(deltaTime);
    }

    private void InitializeGameRect(SpaceArcadeController screen)
    {
        var zoneHeight = 30f;

        var newGameRect = new Rect()
        {
            height = zoneHeight,
            width = screen.GameAreaSize
        };

        newGameRect.center = new Vector2(screen.GameCenter.x, screen.GameCenter.z + zoneHeight / 2);

        _gameRect = newGameRect;
    }
}

