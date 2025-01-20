using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGamePool : IDisposable
{
    bool IsDisposed { get; }
    GameObject RootGameObject { get; }
    IList<GameObject> ActiveGameObjects { get; }

    event Action<IGamePool> OnDispose;

    UniTask<GameObject> Take();
    void Recycle(GameObject gameObject);
    void RecycleAll();

    void ClearAll();
}

public interface IGamePool<TComponent> : IGamePool where TComponent : Component
{
    IList<TComponent> ActiveComponents { get; }

    UniTask<TComponent> TakeComponent();
    void Recycle(TComponent component);
}
