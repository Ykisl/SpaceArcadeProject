using System;
using UnityEngine;

public interface ISpaceArcadeMoveSystem
{
    float Speed { get; set; }

    void Initialize(float startSpeed);
    void Deinitialize();

    void SetSpeed(float speed);
}
