using UnityEngine;

public class Aug_Staff : TimeBasedAugment
{
    private float damageMultiplier = 1f;
    private float projectileSpeed = 2f;
    private float baseProjectileSize = 2f;
    private int penetration = 0;
    private float duration = .8f;
    private float maxDistance = 10f;

    private PlayerProjectile currentPathProjectile;
    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    public Aug_Staff(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.Staff;
    }

    protected override void OnTrigger()
    {
        var activeMonsters = UnitManager.Instance.GetMonstersInRange(0f, float.MaxValue);
        float finalFinalDamage = UnityEngine.Random.value < owner.Stats.CurrentCriRate ? CurrentDamage * owner.Stats.CurrentCriDamage : CurrentDamage;
        if (activeMonsters != null)
        {
            SoundManager.Instance.Play("Staff", SoundManager.Sound.Effect, 1f, false, 0.6f);
            foreach (MonsterBase monster in activeMonsters)
            {
                if (monster != null)
                {
                    monster.TakeDamage(finalFinalDamage * 2);
                    DataManager.Instance.AddDamageData(finalFinalDamage * 2, Enums.AugmentName.Staff);
                }
            }
        }

        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
        }

        Vector2 position = owner.transform.position;
        currentPathProjectile = ProjectileManager.Instance.SpawnPlayerProjectile(
            "PowerEffect",
            position,
            position,
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

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                damageMultiplier = 1f;
                break;
            case 2:
                baseProjectileSize += 0.3f;
                break;
            case 3:
                damageMultiplier *= 1.2f;
                break;
            case 4:
                ModifyBaseInterval(-2f);
                break;
            case 5:
                damageMultiplier *= 1.3f;
                break;
        }
    }
}
