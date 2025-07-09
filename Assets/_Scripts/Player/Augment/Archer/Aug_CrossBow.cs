using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Aug_CrossBow : TimeBasedAugment
{
    private float baseProjectileSize = 0.5f;
    private float damageMultiplier = 1f;
    private float projectileSpeed = 2f;
    private float projectileSize = 0.5f;
    private int penetration = 0;
    private float duration = 5f;
    private float maxDistance = 10f;
    private int baseProjectiles = 6;

    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    public Aug_CrossBow(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.CrossBow;
    }

    protected override async void OnTrigger()
    {
        Vector3 targetPos = UnitManager.Instance.GetNearestMonster().transform.position;
        Vector3 direction = (targetPos - owner.transform.position).normalized;

        int projAmount = owner.Stats.CurrentProjAmount - 1 + baseProjectiles;

        for (int i = 0; i < projAmount; i++)
        {
            SoundManager.Instance.Play("CrossBow", SoundManager.Sound.Effect);
            SpawnProjectile(direction);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
    }

    private void SpawnProjectile(Vector3 direction)
    {
        Vector3 targetPosition = owner.transform.position + direction * 10f;

        ProjectileManager.Instance.SpawnPlayerProjectile(
            "CrossBowProjectile",
            owner.transform.position,
            targetPosition,
            projectileSpeed,
            CurrentDamage,
            CurrentProjectileSize,
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
                break;
            case 2:
                ModifyBaseInterval(-2f);
                break;
            case 3:
                baseProjectiles += 2;
                damageMultiplier *= 1.2f;
                break;
            case 4:
                ModifyBaseInterval(-2f);
                break;
            case 5:
                baseProjectiles += 4;
                damageMultiplier *= 1.3f;
                break;
        }
    }
}
