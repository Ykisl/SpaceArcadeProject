using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GamePool<TComponent> : IGamePool<TComponent> where TComponent : Component
{
    private bool _disposed = false;
    private GameObject _rootGameObject;
    private GamePoolParams<TComponent> _poolParams;

    private List<GameObject> _poolObjects = new List<GameObject>();
    private List<GameObject> _inactiveObjects = new List<GameObject>();
    private List<GameObject> _activeObjects = new List<GameObject>();

    private List<TComponent> _activeComponents = new List<TComponent>();
    private Dictionary<GameObject, TComponent> _componentsGameObjects = new Dictionary<GameObject, TComponent>();

    public bool IsDisposed => _disposed;
    public GameObject RootGameObject => _rootGameObject;
    public IList<GameObject> ActiveGameObjects => _activeObjects;
    public IList<TComponent> ActiveComponents => _activeComponents;

    public event Action<IGamePool> OnDispose;

    public GamePool(GameObject rootGameObject, GamePoolParams<TComponent> poolParams)
    {
        _disposed = false;
        _rootGameObject = rootGameObject;
        _poolParams = poolParams;
    }

    public async UniTask Initialize()
    {
        for(int i = 0; i < _poolParams.ItemsCount; i++)
        {
            await CreateNewObject();
        }
    }

    public async UniTask<GameObject> Take()
    {
        GameObject takenGameObject = null;

        if(_inactiveObjects.Count <= 0)
        {
            if (!_poolParams.IsAllowToCreateNewItems)
            {
                return takenGameObject;
            }

            takenGameObject = await CreateNewObject();
        }
        else
        {
            takenGameObject = _inactiveObjects.FirstOrDefault();
        }

        if(takenGameObject == null)
        {
            return null;
        }

        _activeObjects.Add(takenGameObject);
        _inactiveObjects.Remove(takenGameObject);

        var component = _componentsGameObjects[takenGameObject];
        _activeComponents.Add(component);

        return takenGameObject;
    }

    public async UniTask<TComponent> TakeComponent()
    {
        GameObject takenGameObject = await Take();
        if(takenGameObject == null)
        {
            return null;
        }

        var component = _componentsGameObjects[takenGameObject];
        return component;
    }

    public void Recycle(TComponent component)
    {
        Recycle(component.gameObject);
    }

    public void Recycle(GameObject gameObject)
    {
        if (!_activeObjects.Contains(gameObject))
        {
            return;
        }

        var component = _componentsGameObjects[gameObject];
        _activeComponents.Remove(component);

        _activeObjects.Remove(gameObject);
        _inactiveObjects.Add(gameObject);
        DisableGameObject(gameObject);
    }

    public void RecycleAll()
    {
        var activeObjects = _activeObjects.ToArray();
        foreach(var poolObject in activeObjects)
        {
            Recycle(poolObject);
        }
    }

    public void ClearAll()
    {
        foreach(var poolObject in _poolObjects)
        {
            GameObject.Destroy(poolObject);
        }

        _poolObjects.Clear();
        _inactiveObjects.Clear();
        _activeObjects.Clear();
        _activeComponents.Clear();
        _componentsGameObjects.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        ClearAll();
        _disposed = true;
        OnDispose?.Invoke(this);
    }

    private async UniTask<GameObject> CreateNewObject()
    {
        Func<UniTask<TComponent>> buildAction = _poolParams.CustomItemBuilder;
        buildAction ??= PoolBuildAction;

        var newObject = await buildAction();
        var newGameObject = newObject.gameObject;
        DisableGameObject(newGameObject);

        _poolObjects.Add(newGameObject);
        _inactiveObjects.Add(newGameObject);
        _componentsGameObjects.TryAdd(newGameObject, newObject);

        return newGameObject;
    }

    private async UniTask<TComponent> PoolBuildAction()
    {
        var newObjects = await GameObject.InstantiateAsync(_poolParams.Prefab);
        if( newObjects.Length <= 0) 
        {
            return null;
        }

        return newObjects[0];
    }

    private void DisableGameObject(GameObject gameObject)
    {
        var transform = gameObject.transform;
        transform.parent = _rootGameObject.transform;
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }
}
