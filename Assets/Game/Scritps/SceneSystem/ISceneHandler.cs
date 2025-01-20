using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ISceneHandler
{
    bool IsLoading { get; }
    bool IsLoaded { get; }
    bool IsActive { get; }

    UniTask WaitForLoading(CancellationToken cancellationToken = default);
    UniTask LoadScene(bool isAutoActiveScene = true);
    UniTask UnloadScene();
    UniTask ActivateScene();

    ICollection<GameObject> GetRootGameObjects();
    ICollection<T> GetAllComponents<T>();
    T GetComponent<T>();
}
