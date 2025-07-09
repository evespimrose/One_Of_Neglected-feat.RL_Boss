using UnityEngine;

public class PlayerProjectile : Projectile
{

    protected enum projType
    {
        Melee,
        Normal,
        None,
    }

    public void InitProjectile(Vector3 startPos, Vector3 targetPos, float spd, float dmg, float maxDist = 10f, int pierceCnt = 0, float lifetime = 5f, float size = 1f)
    {
        stats = new ProjectileStats
        {
            projectileSpeed = spd,
            finalDamage = dmg,
            pierceCount = pierceCnt,
            finalDuration = lifetime,
            critical = 0.1f,
            cATK = 1.5f
        };
        
        base.InitProjectile(startPos, targetPos, stats);
        transform.localScale = Vector3.one * size;  // 크기는 직접 설정
    }
} 