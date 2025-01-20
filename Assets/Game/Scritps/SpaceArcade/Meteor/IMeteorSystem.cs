using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public interface IMeteorSystem
{
    UniTask Initialize(Rect gameRect);

    void Deinitialize();

    void Update(float delta);
}
