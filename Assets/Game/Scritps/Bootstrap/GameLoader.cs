using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zenject;

public class GameLoader : IInitializable
{
    private ICameraSystem _cameraSystem;
    private IGameStateSystem _gameStateSystem;
    private ISceneSystem _sceneSystem;
    private IGamePoolSystem _gamePoolSystem;

    private SpaceArcadeGameState.Factory _spaceArcadeFactory;

    [Inject]
    private void Construct(
        ICameraSystem cameraSystem,
        IGameStateSystem gameStateSystem,
        ISceneSystem sceneSystem,
        IGamePoolSystem gamePoolSystem,

        SpaceArcadeGameState.Factory spaceArcadeFactory
        )
    {
        _cameraSystem = cameraSystem;
        _gameStateSystem = gameStateSystem;
        _sceneSystem = sceneSystem;
        _gamePoolSystem = gamePoolSystem;

        _spaceArcadeFactory = spaceArcadeFactory;
    }

    public void Initialize()
    {
        InitializeAsync().Forget();
    }

    private async UniTask InitializeAsync()
    {
        await InitializeSystems();
        await StartGame();
    }

    private async UniTask InitializeSystems()
    {
        Application.targetFrameRate = 60;

        _cameraSystem.Initialize();
        await _gamePoolSystem.Initialize();
        await _sceneSystem.Initialize();
        await _gameStateSystem.Initialize();
    }

    private async UniTask StartGame()
    {
        var gameState = _spaceArcadeFactory.Create(new SpaceArcadeGameState.StateParams());
        await _gameStateSystem.ChangeState(gameState);
    }
}
