using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IFireBulletSystem
{
    void Initialize(Rect gameZoneRect);
    void Deinitialize();

    void Update(float deltaTime);

    UniTask<IFireSource> CreateFireSourceAsync(Transform fireRoot, FireBulletModel bulletModel);
    void ClearAllFireSources();
}
