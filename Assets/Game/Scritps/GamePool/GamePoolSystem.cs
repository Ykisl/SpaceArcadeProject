using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GamePoolSystem : IGamePoolSystem
{
    private bool _isInitilized;
    private GameObject _rootPoolsGameObject;
    private List<IGamePool> _gamePools;

    private const string ROOT_GAME_OBJECT_NAME = "[GamePools]";

    #region IGamePoolService

    public async UniTask Initialize()
    {
        await CreateRootGameObject();
        _gamePools = new List<IGamePool>();

        _isInitilized = true;
    }

    public async UniTask Deinitialize()
    {
        _isInitilized = false;

        RemoveAllPools();

        _gamePools = null;
        await RemoveRootGameObject();
    }

    public async UniTask<IGamePool<TComponent>> CreatePool<TComponent>(GamePoolParams<TComponent> poolParams) where TComponent : Component
    {
        if (!_isInitilized)
        {
            return null;
        }

        var poolName = poolParams.PoolName;
        if (string.IsNullOrEmpty(poolName))
        {
            poolName = $"PoolOf:({poolParams?.Prefab?.name})";
        }

        var gameObjectName = $"[Pool] {poolName}";
        var poolGameObject = new GameObject(gameObjectName);

        var objectTransform = poolGameObject.transform;
        objectTransform.parent = _rootPoolsGameObject.transform;
        objectTransform.position = Vector3.zero;

        var newPool = new GamePool<TComponent>(poolGameObject, poolParams);
        newPool.OnDispose += HandleGamePoolDispose;
        _gamePools.Add(newPool);

        await newPool.Initialize();
        return newPool;
    }

    public void RemovePool(IGamePool gamePool)
    {
        if (!_isInitilized)
        {
            return;
        }

        if (gamePool.IsDisposed || !_gamePools.Contains(gamePool))
        {
            return;
        }

        gamePool.Dispose();
    }

    private void HandleGamePoolDispose(IGamePool gamePool)
    {
        gamePool.OnDispose -= HandleGamePoolDispose;

        _gamePools.Remove(gamePool);
        GameObject.Destroy(gamePool.RootGameObject);
    }

    public void RemoveAllPools()
    {
        if (!_isInitilized)
        {
            return;
        }

        foreach (var gamePool in _gamePools)
        {
            gamePool.OnDispose -= HandleGamePoolDispose;
            gamePool.Dispose();
            GameObject.Destroy(gamePool.RootGameObject);
        }

        _gamePools.Clear();
    }

    #endregion

    private UniTask CreateRootGameObject()
    {
        var rootObject = new GameObject();
        rootObject.name = ROOT_GAME_OBJECT_NAME;
        rootObject.transform.position = Vector3.zero;
        GameObject.DontDestroyOnLoad(rootObject);

        _rootPoolsGameObject = rootObject;

        return UniTask.CompletedTask;
    }

    private UniTask RemoveRootGameObject()
    {
        GameObject.Destroy(_rootPoolsGameObject);
        _rootPoolsGameObject = null;

        return UniTask.CompletedTask;
    }
}
