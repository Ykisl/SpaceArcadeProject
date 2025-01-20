using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ITurretSystem
{
    void Initialize(Rect gameZoneRect);
    void Deinitialize();

    void Update(float deltaTime);

    UniTask<ITurret> CreateFireSourceAsync(Transform fireRoot, TurretModel turretModel);
    void ClearAllFireSources();
}
