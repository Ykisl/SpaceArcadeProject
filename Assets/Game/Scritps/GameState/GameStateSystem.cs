using Cysharp.Threading.Tasks;
using Zenject;

public class GameStateSystem : IGameStateSystem
{
    private IGameState _activeState = null;

    public IGameState ActiveState => _activeState;
    public EGameStateType ActiveStateType => GetActiveGameStateType();

    public UniTask Initialize()
    {
        return ResetState();
    }

    public async UniTask ChangeState(IGameState newState)
    {
        if(_activeState != null)
        {
            await _activeState.UnloadState();
        }

        _activeState = newState;
        if(_activeState != null)
        {
            await _activeState.LoadState();
        }
    }

    public async UniTask ResetState()
    {
        await ChangeState(null);
    }

    private EGameStateType GetActiveGameStateType()
    {
        if (_activeState == null)
        {
            return EGameStateType.None;
        }

        return _activeState.GameStateType;
    }
}
