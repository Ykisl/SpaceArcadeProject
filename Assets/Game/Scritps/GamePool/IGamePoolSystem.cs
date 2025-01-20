using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IGamePoolSystem
{
    UniTask Initialize();
    UniTask Deinitialize();

    UniTask<IGamePool<TComponent>> CreatePool<TComponent>(GamePoolParams<TComponent> poolParams) where TComponent : Component;

    void RemovePool(IGamePool gamePool);
    void RemoveAllPools();
}
