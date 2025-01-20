using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FireSource : IFireSource
{
    private Rect _gameZoneRect;
    private Transform _fireRoot;
    private FireBulletModel _model;
    private IGamePool<Transform> _viewPool;

    private bool _isDisposed;
    private List<Transform> _activeBullets;

    public event Action<FireSource> OnDispose;

    public FireSource(Rect gameZoneRect, Transform fireRoot, FireBulletModel model, IGamePool<Transform> viewPool)
    {
        _isDisposed = false;
        _activeBullets = new List<Transform>();

        _model = model;
        _gameZoneRect = gameZoneRect;
        _fireRoot = fireRoot;
        _viewPool = viewPool;
    }

    public void Fire()
    {
        if (_isDisposed)
        {
            return;
        }

        SpawnBullet().Forget();
    }

    public void Update(float deltaTime)
    {
        if (_isDisposed)
        {
            return;
        }

        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            var bullet = _activeBullets[i];
            UpdateBullet(deltaTime, bullet);
        }
    }

    public void UpdateBullet(float deltaTime, Transform bulletTransofrm)
    {
        var lastPosition = bulletTransofrm.position;
        var newPosition = lastPosition + bulletTransofrm.forward * _model.Speed * deltaTime;

        RaycastBulletPath(lastPosition, newPosition, out var hitTarget, out var movePosition);
       

        if(hitTarget != null && hitTarget.TryHit(_model.BulletType, _model.Damage, _model.Speed))
        {
            RemoveBullet(bulletTransofrm);
            return;
        }

        bulletTransofrm.position = movePosition;

        if (newPosition.y > _gameZoneRect.yMax + 10f)
        {
            RemoveBullet(bulletTransofrm);
            return;
        }
    }

    private void RaycastBulletPath(Vector3 lastPosition, Vector3 newPosition, out IFireTarget hittedObject, out Vector3 movePosition)
    {
        hittedObject = null;
        movePosition = newPosition;

        var direction = newPosition - lastPosition;
        var castedObjects = Physics.CapsuleCastAll(lastPosition, newPosition, 0.3f, direction, direction.magnitude, -1, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < castedObjects.Length; i++)
        {
            RaycastHit castedObject = castedObjects[i];
            var hitColider = castedObject.collider;
            var hitTarget = hitColider.GetComponent<IFireTarget>();
            if(hitTarget == null)
            {
                continue;
            }

            if(hittedObject == null || Vector3.Distance(lastPosition, castedObject.point) < Vector3.Distance(lastPosition, movePosition))
            {
                hittedObject = hitTarget;
                movePosition = castedObject.point;
            }
        }
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        ClearAllBullets();
        _viewPool.Dispose();

        _gameZoneRect = default;

        _isDisposed = true;
        OnDispose?.Invoke(this);
    }

    private async UniTask SpawnBullet()
    {
        var newBullet = await _viewPool.TakeComponent();
        newBullet.rotation = _fireRoot.rotation;
        newBullet.position = _fireRoot.position;
        newBullet.gameObject.SetActive(true);

        _activeBullets.Add(newBullet);
    }

    private void RemoveBullet(Transform bullet)
    {
        _activeBullets.Remove(bullet);
        _viewPool.Recycle(bullet);
    }

    private void ClearAllBullets()
    {
        _activeBullets.Clear();
        _viewPool.ClearAll();
    }
}
