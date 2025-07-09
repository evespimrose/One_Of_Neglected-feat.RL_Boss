using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using static Enums;

public class Aug_LongBow : ConditionalAugment
{
    private float damageMultiplier = 1f;
    private float projectileSpeed = 1f;
    private float baseProjectileSize = 10f;
    private int penetration = 0;
    private float duration = 10f;
    private float maxDistance = 30f;
    private int Projectiles = 0;
    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    public Aug_LongBow(Player owner) : base(owner)
    {
        aguName = AugmentName.LongBow;
    }

    public override void Activate()
    {
        base.Activate();
        owner.attackDetect += OnAttackDetect;
    }

    private async void OnAttackDetect(Vector3 targetPosition)
    {
        int projAmount = owner.Stats.CurrentProjAmount - 1 + Projectiles;

        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        for (int i = 0; i < projAmount; i++)
        {
            SoundManager.Instance.Play("Bow Attack 1", SoundManager.Sound.Effect);
            SpawnProjectile(targetPosition);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
        
    }
    private void SpawnProjectile(Vector3 direction)
    {
        ProjectileManager.Instance.SpawnPlayerProjectile(
              "LongBowProjectile",
              owner.transform.position,
              direction,
              projectileSpeed,
              CurrentDamage,
              CurrentProjectileSize,
              maxDistance,
              penetration,
              duration
          );
    }
    public override bool CheckCondition()
    {
        return true;
    }

    public override void OnConditionDetect()
    {
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                break;
            case 2:
                damageMultiplier *= 1.1f;
                Projectiles += 1;
                break;
            case 3:
                damageMultiplier *= 1.2f;
                break;
            case 4:
                damageMultiplier *= 1.1f;
                Projectiles += 1;
                break;
            case 5:
                damageMultiplier *= 1.2f;
                break;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        owner.attackDetect -= OnAttackDetect;
    }
}
