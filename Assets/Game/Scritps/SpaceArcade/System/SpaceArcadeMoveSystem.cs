using System;
using UnityEngine;

public class SpaceArcadeMoveSystem : ISpaceArcadeMoveSystem
{
    private float _moveSpeed;

    public float Speed
    {
        get => _moveSpeed; set => SetSpeed(value);
    }

    public void Initialize(float startSpeed)
    {
        _moveSpeed = startSpeed;
    }

    public void Deinitialize()
    {
        _moveSpeed = 0;
    }

    public void SetSpeed(float speed)
    {
        _moveSpeed = speed;
    }
}
