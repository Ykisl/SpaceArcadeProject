using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class MeteorSystem : IMeteorSystem
{
    private IGamePoolSystem _gamePoolSystem;
    private ISpaceArcadeMoveSystem _moveSystem;

    private Rect _gameZoneRect;
    private GameObject _meteorPrefab;

    private IGamePool<MeteorController> _meteorPool;
    private List<MeteorController> _activeMeteors;

    private float _spawnTimer = 0f;
    private bool _isSpawnTimerActive = false;

    private const string METEOR_ASSET_NAME = "Game/GameAssets/SpaceArcade/Meteor/Meteor01";
    private const float METEROR_SPAWN_TIME = 1f;
    private const float METEOR_BASE_HP = 150f;

    [Inject]
    private void Consturct(
        IGamePoolSystem gamePoolSystem,
        ISpaceArcadeMoveSystem moveSystem
        )
    {
        _gamePoolSystem = gamePoolSystem;
        _moveSystem = moveSystem;
    }

    public async UniTask Initialize(Rect gameRect)
    {
        _gameZoneRect = gameRect;
        _meteorPrefab = await Addressables.LoadAssetAsync<GameObject>(METEOR_ASSET_NAME);

        var poolParams = new GamePoolParams<MeteorController>()
        {
            Prefab = _meteorPrefab.GetComponent<MeteorController>(),
            IsAllowToCreateNewItems = true,
            ItemsCount = 20
        };

        _meteorPool = await _gamePoolSystem.CreatePool<MeteorController>(poolParams);
        _activeMeteors = new List<MeteorController>();

        _spawnTimer = 0f;
        _isSpawnTimerActive = true;
    }

    public void Deinitialize()
    {
        _isSpawnTimerActive = false;
        _spawnTimer = 0f;

        _activeMeteors = null;
        _gamePoolSystem.RemovePool(_meteorPool);

        Addressables.Release(_meteorPrefab);
        _meteorPrefab = null;

        _gameZoneRect = default;
    }

    public void Update(float delta)
    {
        UpdateSpawnTimer(delta);
        UpdateMeteors(delta);
    }

    private void SpawnMeteor()
    {
        SpawnMeteorAsync().Forget();
    }

    private void UpdateMeteors(float delta)
    {
        for(int i = _activeMeteors.Count - 1; i >= 0; i--)
        {
            var meteor = _activeMeteors[i];
            var meteorTransform = meteor.transform;

            var position = meteorTransform.position;
            position.z -= _moveSystem.Speed * delta;
            meteorTransform.position = position;

            if (position.z < _gameZoneRect.yMin - 5f)
            {
                RemoveMeteor(meteor);
            }
        }
    }

    private async UniTask SpawnMeteorAsync()
    {
        var spawnPosition = new Vector3()
        {
            y = 0f,
            z = _gameZoneRect.yMax,
            x = UnityEngine.Random.Range(_gameZoneRect.xMin, _gameZoneRect.xMax)
        };

        var newMeteor = await _meteorPool.TakeComponent();
        newMeteor.OnMeteorDestory += HandleMeteorDestory;

        var meteorTransofrm = newMeteor.transform;
        meteorTransofrm.position = spawnPosition;

        var meteorRotaion = new Vector3(UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f));
        meteorTransofrm.rotation = Quaternion.Euler(meteorRotaion);

        var meteorScale = UnityEngine.Random.Range(0.9f, 1.4f);
        meteorTransofrm.localScale = new Vector3(meteorScale, meteorScale, meteorScale);

        newMeteor.Initialize(METEOR_BASE_HP);
        newMeteor.gameObject.SetActive(true);

        _activeMeteors.Add(newMeteor);
    }

    private void RemoveMeteor(MeteorController meteor)
    {
        meteor.OnMeteorDestory -= HandleMeteorDestory;
        _activeMeteors.Remove(meteor);
        _meteorPool.Recycle(meteor);
    }

    private void UpdateSpawnTimer(float delta)
    {
        if (!_isSpawnTimerActive)
        {
            return;
        }

        _spawnTimer += delta;
        if(_spawnTimer > METEROR_SPAWN_TIME)
        {
            SpawnTimerFinished();
        }
    }

    private void SpawnTimerFinished()
    {
        _spawnTimer = 0;
        SpawnMeteor();
    }

    private void HandleMeteorDestory(MeteorController meteor)
    {
        RemoveMeteor(meteor);
    }
}
