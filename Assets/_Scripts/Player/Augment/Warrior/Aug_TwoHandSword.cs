using UnityEngine;

public class Aug_TwoHandSword : TimeBasedAugment
{
    private float damageMultiplier = 1f;  
    private float projectileSpeed = 2f;
    private float baseProjectileSize = 1f; 
    private int penetration = 0;
    private float duration = 5f;
    private float maxDistance = 10f;

    private bool isProjBreak = false;
    
    //private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;  

    public Aug_TwoHandSword(Player owner, float interval) : base(owner, interval)
    {
        aguName = Enums.AugmentName.TwoHandSword;
        damageMultiplier = 1.5f;
    }

    protected override void OnTrigger()
    {
        Vector3 direction;
        MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
        
        if (nearestMonster != null)
        {
            Vector3 targetPos = nearestMonster.transform.position;
            direction = (targetPos - owner.transform.position).normalized;
        }
        else
        {
            direction = owner.transform.right; 
        }

        int projAmount = owner.Stats.CurrentProjAmount;
        float angleStep = 15f;

        int totalProjectiles = projAmount;

        if (level >= 5)
        {
            totalProjectiles += 2;
        }
        else if (level >= 3)
        {
            totalProjectiles += 1;
        }

        float totalAngleSpread = (totalProjectiles - 1) * angleStep;
        float startAngle = -totalAngleSpread / 2f;
        
        SoundManager.Instance.Play("SwordAuror", SoundManager.Sound.Effect);
        for (int i = 0; i < totalProjectiles; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            SpawnProjectile(RotateVector(direction, currentAngle));

         
        }
    }

    private void SpawnProjectile(Vector3 direction)
    {
        Vector3 targetPosition = owner.transform.position + direction * 10f;
        float currentDamage = owner.Stats.CurrentATK * damageMultiplier;  
        
        PlayerProjectile proj = ProjectileManager.Instance.SpawnPlayerProjectile(
            "SwordAurorProjectile",
            owner.transform.position,
            targetPosition,
            projectileSpeed,
            currentDamage,  
            CurrentProjectileSize,  
            maxDistance,
            penetration,
            duration);

        if (isProjBreak && proj is SwordAurorProjectile aurorProj)
        {
            aurorProj.SetProjBreak(true);
        }
    }

    private Vector3 RotateVector(Vector3 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        
        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;
        
        return new Vector3(x, y, 0);
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                break;
            case 2:
                baseProjectileSize += 0.3f;  
                break;
            case 3:
                break;
            case 4:
                ModifyBaseInterval(-2f);
                break;
            case 5:
                isProjBreak = true;
                break;
        }
    }
}