using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class SpaceArcadePlayerShipSystem : ISpaceArcadePlayerShipSystem
{
    private SpaceArcadeShipSettings _settings;

    private ITurretSystem _fireBulletSystem;

    private Transform _playerRoot;
    private float _gameAreaSize;

    private GameObject _playerPrefab;

    private PlayerShipController _player;
    private List<ITurret> _fireSoruces;
    private bool _isInitialized;

    private const string BASE_SHIP_PREFAB_NAME = "Game/GameAssets/SpaceArcade/Ship/Ship02";

    public PlayerShipController PlayerShip => _player;

    [Inject]
    private void Construct(
        SpaceArcadeShipSettings shipSettings,
        ITurretSystem fireBulletSystem
        )
    {
        _settings = shipSettings;
        _fireBulletSystem = fireBulletSystem;
    }

    public async UniTask Initialize(Transform playerRoot, float gameAreaSize)
    {
        if(_isInitialized) 
            return;

        _playerRoot = playerRoot;
        _gameAreaSize = gameAreaSize;

        _playerPrefab = await Addressables.LoadAssetAsync<GameObject>(BASE_SHIP_PREFAB_NAME);

        _fireSoruces = new List<ITurret>();

        _isInitialized = true;
    }

    public async UniTask SpawnShip()
    {
        if(!_isInitialized || _player != null)
        {
            return;
        }

        var spawnOperation = GameObject.InstantiateAsync(_playerPrefab, _playerRoot, Vector3.zero, Quaternion.identity);
        await spawnOperation;

        var playerObject = spawnOperation.Result[0];
        _player = playerObject.GetComponent<PlayerShipController>();

        InitializeShip(_player);

        var bulletModel = new FireBulletModel()
        {
            Damage = 10,
            BulletType = EFireBulletType.BaseRed,
            Speed = 30f,
        };

        var turretModel = new TurretModel()
        {
            FireSpeed = 0.2f,
            BulletModel = bulletModel,
        };

        foreach(var shipFireSpot in _player.FireSources)
        {
            var newFireSource = await _fireBulletSystem.CreateFireSourceAsync(shipFireSpot, turretModel);
            _fireSoruces.Add(newFireSource);
        }
    }

    public void ClearShip()
    {
        if( _player != null )
        {
            DeinitializeShip(_player);
            GameObject.Destroy(_player);
            _player = null;
        }
    }

    public void Deinitialize()
    {
        if( !_isInitialized)
        {
            return;
        }

        ClearShip();

        if(_playerPrefab != null)
        {
            Addressables.Release(_playerPrefab);
            _playerPrefab = null;
        }

        foreach(var fireSource in _fireSoruces)
        {
            fireSource.Dispose();
        }

        _fireSoruces = null;

        _playerRoot = null;
        _gameAreaSize = 0;
    }

    private void InitializeShip(PlayerShipController playerShip)
    {
        var moveArea = new PlayerShipController.MoveArea
        {
            Center = _playerRoot.position,
            Size = _gameAreaSize,
        };

        playerShip.SetMoveArea(moveArea);
        playerShip.SetMovementSpeed(_settings.ShipMovementSpeed);
    }

    private void DeinitializeShip(PlayerShipController playerShip)
    {
        playerShip.SetMovementSpeed(0);
    }

    public void Update(float deltaTime)
    {
        if (!_isInitialized)
        {
            return;
        }

        UpdateShipInput(deltaTime);
    }

    private void UpdateShipInput(float deltaTime)
    {
        var moveVector = _settings.MoveInputAction.action.ReadValue<Vector2>();
        _player.UpdateMovement(deltaTime, moveVector);

        var isFireActionActive = _settings.FireInputAction.action.IsPressed();
        if(isFireActionActive)
        {
            UpdatePlayerFire();
        }
    }

    private void UpdatePlayerFire()
    {
        foreach (var fireSource in _fireSoruces)
        {
            fireSource.Fire();
        }
    }
}
