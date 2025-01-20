using System;
using UnityEngine;

public class MeteorController : MonoBehaviour, IFireTarget
{
    private Transform _transform;

    private float _heathPoints = 0;
    private float _maxHealthPoints = 0;
    private Vector3 _rotationSpeed = Vector3.zero;

    public new Transform transform
    {
        get => _transform ??= base.transform;
    }

    public event Action<MeteorController> OnMeteorHit;
    public event Action<MeteorController> OnMeteorDestory;

    public void Initialize(float healthPoints)
    {
        _maxHealthPoints = healthPoints;
        _heathPoints = healthPoints;
    }

    public void SetRotationSpeed(Vector3 speed)
    {
        _rotationSpeed = speed;
    }

    public bool TryHit(EFireBulletType bulletType, float damage, float hitSpeed)
    {
        OnMeteorHit?.Invoke(this);

        _heathPoints = Mathf.Clamp(_heathPoints - damage, 0, _maxHealthPoints);

        if (_heathPoints <= 0)
        {
            OnMeteorDestory?.Invoke(this);
        }

        return true;
    }

    private void Update()
    {
        transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}
