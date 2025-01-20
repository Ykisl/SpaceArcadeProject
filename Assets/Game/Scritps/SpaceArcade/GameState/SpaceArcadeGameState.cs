using Cysharp.Threading.Tasks;
using Zenject;

public class SpaceArcadeGameState : IGameState
{
    public class Factory : PlaceholderFactory<StateParams, SpaceArcadeGameState> { }

    public class StateParams
    {

    }

    private StateParams _stateParams;

    private ISpaceArcadeSystem _spaceArcadeSystem;

    public EGameStateType GameStateType => EGameStateType.SpaceArcade;

    public SpaceArcadeGameState(StateParams stateParams)
    {
        _stateParams = stateParams;
    }

    [Inject]
    private void Construct(
        ISpaceArcadeSystem spaceArcadeSystem
        )
    {
        _spaceArcadeSystem = spaceArcadeSystem;
    }

    public async UniTask LoadState()
    {
        await _spaceArcadeSystem.Show();
    }

    public async UniTask UnloadState()
    {
        await _spaceArcadeSystem.Close();
    }
}
