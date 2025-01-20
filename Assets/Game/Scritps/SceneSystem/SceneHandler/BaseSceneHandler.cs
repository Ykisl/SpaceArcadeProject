using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public abstract class BaseSceneHandler : ISceneHandler
{
    public abstract bool IsLoading { get; }

    public abstract bool IsLoaded { get; }

    public abstract bool IsActive {  get; }

    public UniTask WaitForLoading(CancellationToken cancellationToken = default)
    {
        if(IsLoaded || !IsLoading)
        {
            return UniTask.CompletedTask;
        }

        try
        {
            return UniTask.WaitUntil(() => !IsLoading, cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return UniTask.CompletedTask;
        }
    }

    public abstract UniTask LoadScene(bool isAutoActiveScene = true);

    public abstract UniTask UnloadScene();
    public abstract UniTask ActivateScene();

    public ICollection<T> GetAllComponents<T>()
    {
        var components = new List<T>();

        var gameObjects = GetRootGameObjects();
        if(gameObjects != null || gameObjects.Count > 0)
        {
            foreach(var gameObject in gameObjects)
            {
                var gameObjectComponents = gameObject.GetComponentsInChildren<T>();
                components.AddRange(gameObjectComponents);
            }
        }

        return components;
    }

    public T GetComponent<T>()
    {
        return GetAllComponents<T>().FirstOrDefault();
    }

    public abstract ICollection<GameObject> GetRootGameObjects();
}
