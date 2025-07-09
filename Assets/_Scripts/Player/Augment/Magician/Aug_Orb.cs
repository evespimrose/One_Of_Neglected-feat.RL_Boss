using UnityEngine;

public class Aug_Orb : TimeBasedAugment
{
    private float damageMultiplier = 1f;
    private float projectileSpeed = 2f;
    private float baseProjectileSize = 3f;
    private int penetration = 100;
    private float duration = 5f;
    private float maxDistance = 10f;
    
    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    public Aug_Orb(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.Orb;
    }

    protected override void OnTrigger()
    {
        SpawnJewelProjectile();
    }

    private void SpawnJewelProjectile()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        Vector3 spawnPosition = owner.transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        Vector2 dashStart = owner.transform.position;

        SoundManager.Instance.Play("Orb", SoundManager.Sound.Effect, 1f, false, 0.5f);
        ProjectileManager.Instance.SpawnPlayerProjectile(
            "JewelProjectile",
            dashStart,
            dashStart,
            projectileSpeed,
            CurrentDamage / 2,  
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
                damageMultiplier *= 1.2f;
                break;
            case 3:
                ModifyBaseInterval(-2f);
                damageMultiplier *= 1.2f;
                break;
            case 4:
                ModifyBaseInterval(-2f);
                damageMultiplier *= 1.2f;
                break;
            case 5:
                ModifyBaseInterval(-4f);
                damageMultiplier *= 1.4f;
                break;
        }
    }
}
