using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class BulletTypeData
{
    public EFireBulletType BulletType;

    public bool IsLoaded;
    public AsyncOperationHandle<GameObject> AssetOperation;

    public GameObject Prefab;
}
public class FireBulletSystem : IFireBulletSystem
{
    private Dictionary<EFireBulletType, BulletTypeData> _bulletTypes = new Dictionary<EFireBulletType, BulletTypeData>();
    private List<FireSource> _fireSources = new List<FireSource>();

    private Rect _gameZoneRect;
    private bool _isInitialized = false;

    private IGamePoolSystem _gamePoolSystem;

    [Inject]
    private void Construct(
        IGamePoolSystem gamePoolSystem
        )
    {
        _gamePoolSystem = gamePoolSystem;
    }

    public void Initialize(Rect gameZoneRect)
    {
        if (_isInitialized)
        {
            return;
        }

        _gameZoneRect = gameZoneRect;

        _bulletTypes ??= new Dictionary<EFireBulletType, BulletTypeData>();
        _bulletTypes.Clear();

        _fireSources ??= new List<FireSource>();
        _fireSources.Clear();

        _isInitialized = true;
    }

    public void Deinitialize()
    {
        if (!_isInitialized)
        {
            return;
        }

        UnloadAllBulletTypes();
        _gameZoneRect = default;

        _isInitialized = false;
    }

    public void Update(float deltaTime)
    {
        if (!_isInitialized)
        {
            return;
        }

        for(int i = _fireSources.Count - 1; i >= 0; i--)
        {
            var fireSource = _fireSources[i];
            fireSource.Update(deltaTime);
        }
    }

    public async UniTask<IFireSource> CreateFireSourceAsync(Transform fireRoot, FireBulletModel bulletModel)
    {
        if (!_isInitialized)
        {
            return null;
        }

        var bulletTypeData = await GetOrLoadBulletType(bulletModel.BulletType);

        var poolParams = new GamePoolParams<Transform>()
        {
            IsAllowToCreateNewItems = true,
            ItemsCount = 20,
            Prefab = bulletTypeData.Prefab.transform,
        };

        var bulletPool = await _gamePoolSystem.CreatePool<Transform>(poolParams);
        var newFireSource = new FireSource(_gameZoneRect, fireRoot, bulletModel, bulletPool);
        _fireSources.Add(newFireSource);

        return newFireSource;

    }

    public void ClearAllFireSources()
    {
        foreach (var fireSource in _fireSources)
        {
            fireSource.OnDispose -= HandleFireSouceDispose;
            fireSource.Dispose();
        }

        _fireSources.Clear();
    }

    private void HandleFireSouceDispose(FireSource source)
    {
        source.OnDispose -= HandleFireSouceDispose;
        _fireSources.Remove(source);
    }

    private async UniTask<BulletTypeData> GetOrLoadBulletType(EFireBulletType fireType)
    {
        if (!_isInitialized)
        {
            return new BulletTypeData() { BulletType = fireType };
        }

        if(_bulletTypes.TryGetValue(fireType, out var bulletTypeData))
        {
            if(!bulletTypeData.IsLoaded)
            {
                await UniTask.WaitUntil(() => bulletTypeData.IsLoaded);
            }

            return bulletTypeData;
        }

        bulletTypeData = new BulletTypeData()
        {
            BulletType = fireType,
            IsLoaded = false,
            Prefab = null
        };

        _bulletTypes.Add(fireType, bulletTypeData);
        if (fireType == EFireBulletType.None)
        {
            bulletTypeData.IsLoaded = true;
            return bulletTypeData;
        }

        var assetPath = $"Game/GameAssets/SpaceArcade/Projectile/{fireType}";
        bulletTypeData.AssetOperation = Addressables.LoadAssetAsync<GameObject>(assetPath);
        bulletTypeData.Prefab = await bulletTypeData.AssetOperation;
        bulletTypeData.IsLoaded = true;

        return bulletTypeData;
    }

    private void UnloadAllBulletTypes()
    {
        foreach(var loadedBulletType in _bulletTypes)
        {
            var bulletType = loadedBulletType.Value;
            bulletType.Prefab = null;
            bulletType.IsLoaded = true;

            Addressables.Release(bulletType.AssetOperation);
        }

        _bulletTypes.Clear();
    }
}
