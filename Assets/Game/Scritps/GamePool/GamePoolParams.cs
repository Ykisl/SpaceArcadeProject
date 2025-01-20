using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GamePoolParams<TComponent> where TComponent : Component
{
    public TComponent Prefab;
    public int ItemsCount = 20;
    public bool IsAllowToCreateNewItems = true;

    public Func<UniTask<TComponent>> CustomItemBuilder = null;
    public string PoolName = null;
}
