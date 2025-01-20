using UnityEngine;
using Zenject;

public class GameMonoInstaller : MonoInstaller
{
    [SerializeField] private Camera _mainCamera;

    public override void InstallBindings()
    {
        RegisterSystems();
        RegisterGameStates();
        RegisterEntryPoint();
    }

    private void RegisterEntryPoint()
    {
        Container.BindInterfacesAndSelfTo<GameLoader>()
            .AsSingle()
            .NonLazy();
    }

    private void RegisterSystems()
    {
        Container.Bind<ICameraSystem>()
            .FromInstance(new CameraSystem(_mainCamera))
            .AsSingle();

        Container.Bind<IGamePoolSystem>()
            .To<GamePoolSystem>()
            .AsSingle();

        Container.Bind<ISceneSystem>()
            .To<SceneSystem>()
            .AsSingle();

        Container.Bind<IGameStateSystem>()
            .To<GameStateSystem>()
            .FromNew().AsSingle();

    }

    #region GameStates

    private void RegisterGameStates()
    {
        RegisterSpaceArcadeGameState();
    }

    private void RegisterSpaceArcadeGameState()
    {
        Container.BindInterfacesAndSelfTo<SpaceArcadeSystem>()
            .AsSingle();

        Container.BindFactory<SpaceArcadeGameState.StateParams, SpaceArcadeGameState, SpaceArcadeGameState.Factory>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<SpaceArcadePlayerShipSystem>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<SpaceArcadeMoveSystem>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<MeteorSystem>()
            .AsSingle();

        Container.BindInterfacesAndSelfTo<TurretSystem>()
            .AsSingle();
    }

    #endregion
}
