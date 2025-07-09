using System.Collections;
using UnityEngine;

public class Aug_Shielder : ConditionalAugment
{
    private float damageMultiplier = 1f;  
    private float baseProjectileSize = 1f;
    private int penetration = 100;
    private float duration = 5f;
    private float maxDistance = 10f;

    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    private PlayerProjectile currentPathProjectile;

    public Aug_Shielder(Player owner) : base(owner)
    {
        aguName = Enums.AugmentName.Shielder;
    }

    public override void Activate()
    {
        base.Activate();

        owner.dashDetect += OnDashDetect;
        owner.dashCompleted += OnDashCompleted;
        owner.DamageReduction += 10f;
    }

    private void OnDashDetect()
    {
        if (CheckCondition())
        {
            OnConditionDetect();
            SpawnRushProjectile();
        }
    }

    public override bool CheckCondition()
    {
        return true;
    }

    public override void OnConditionDetect()
    {
    }

    private void SpawnRushProjectile()
    {
        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
        }

        Vector2 dashStart = owner.transform.position;
        currentPathProjectile = ProjectileManager.Instance.SpawnPlayerProjectile(
            "RushProjectile",
            dashStart,
            dashStart,
            0f,
            CurrentDamage,
            CurrentProjectileSize,
            maxDistance,
            penetration,
            duration
            );

        if (currentPathProjectile != null)
        {
            currentPathProjectile.transform.SetParent(owner.transform);
            currentPathProjectile.transform.rotation = Quaternion.identity;
            currentPathProjectile.transform.localPosition = Vector3.zero;
        }
    }

    private void SpawnshockwaveProjectile()
    {
        SoundManager.Instance.Play("ShockWave", SoundManager.Sound.Effect, 1.0f, false, 0.3f);
        Vector2 dashEnd = owner.targetPosition;
        ProjectileManager.Instance.SpawnPlayerProjectile(
            "RushEndProjectile",
            dashEnd,
            dashEnd,
            0f,
            CurrentDamage / 2,
            CurrentProjectileSize *2,
            maxDistance,
            penetration,
            0.5f); 
    }

    private void OnDashCompleted()
    {
        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
            currentPathProjectile = null;
        }

        if (level >= 5)
        {
            SpawnshockwaveProjectile();
        }
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                break;
            case 2:
                owner.dashRechargeTime *= 0.9f;
                break;
            case 3:
                owner.DamageReduction += 10f;
                owner.Stats.CurrentDashCount++;
                break;
            case 4:
                owner.dashRechargeTime *= 0.8f;
                break;
            case 5:
                break;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        owner.dashDetect -= OnDashDetect;
        owner.dashCompleted -= OnDashCompleted;
        owner.DamageReduction = 0f; 

        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
            currentPathProjectile = null;
        }
    }
}
