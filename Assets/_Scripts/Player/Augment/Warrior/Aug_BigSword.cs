using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class Aug_BigSword : TimeBasedAugment
{
    private float baseProjectileSize = 3f;
    private float damageMultiplier = 1f;
    private float projectileSpeed = 1f;
    private float projectileSize = 3f;
    private int penetration = 100;
    private float duration = 0.5f;
    private float maxDistance = 0f;

    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;
    private float SubDamage => CurrentDamage * 0.3f; 

    public Aug_BigSword(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.BigSword;
    }

    protected override async void OnTrigger()
    {
        if (owner == null) return;

        if (level >= 5)
        {
            SpawnProjectile();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f / owner.Stats.CurrentAspd));
            if (owner == null) return;
            SpawnProjectile();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f / owner.Stats.CurrentAspd));
            if (owner == null) return;
            SpawnSubProjectile();
        }
        else
        {
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        if (owner == null) return;
        
        SoundManager.Instance.Play("ShockWave", SoundManager.Sound.Effect, 1.0f, false, 0.3f);
        ProjectileManager.Instance.SpawnPlayerProjectile(
            "EarthquakeProjectile",
            owner.transform.position,
            owner.transform.position,
            projectileSpeed,
            CurrentDamage,
            CurrentProjectileSize,
            maxDistance,
            penetration,
            duration);
    }
    private void SpawnSubProjectile()
    {
        if (owner == null) return;
        
        SoundManager.Instance.Play("ShockWave", SoundManager.Sound.Effect, 1.0f, false, 0.3f);
        ProjectileManager.Instance.SpawnPlayerProjectile(
            "SubEarthquakeProjectile",
            owner.transform.position,
            owner.transform.position,
            projectileSpeed,
            SubDamage,
            projectileSize,
            maxDistance,
            penetration,
            duration);
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                damageMultiplier = 1f;
                break;
            case 2:
                projectileSize += 0.2f;
                break;
            case 3:
                ModifyBaseInterval(-2f);
                break;
            case 4:
                damageMultiplier *= 1.3f;
                break;
            case 5:
                projectileSize += 0.3f;
                break;
        }
    }
}
