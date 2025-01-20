using System;

public interface IFireTarget
{
    bool TryHit(EFireBulletType bulletType, float damage, float hitSpeed);
}
