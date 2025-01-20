using Cysharp.Threading.Tasks;

public interface IGameStateSystem
{
    IGameState ActiveState { get; }
    EGameStateType ActiveStateType { get; }

    UniTask Initialize();
    UniTask ChangeState(IGameState newState);
    UniTask ResetState();
}
