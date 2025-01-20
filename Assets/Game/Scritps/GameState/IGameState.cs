using Cysharp.Threading.Tasks;

public interface IGameState
{
    EGameStateType GameStateType { get; }

    UniTask LoadState();

    UniTask UnloadState();
}
