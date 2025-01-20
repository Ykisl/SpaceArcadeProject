using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISpaceArcadePlayerShipSystem
{
    PlayerShipController PlayerShip {  get; }

    UniTask Initialize(Transform playerRoot, float gameAreaSize);
    UniTask SpawnShip();
    void ClearShip();
    void Deinitialize();

    void Update(float deltaTime);
}
