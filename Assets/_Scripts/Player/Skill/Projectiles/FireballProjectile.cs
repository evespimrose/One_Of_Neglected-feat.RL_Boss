using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FireballProjectile : Projectile
{
    private float knockbackForce;

    protected override void Start()
    {
        base.Start();
        isMoving = true;
        transform.position = startPosition;
        cts = new CancellationTokenSource();
        MoveProjectileAsync(cts.Token).Forget();
        knockbackForce = 1f;
    }

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;

        stats = projectileStats;

        print($"InitProjectile - lifetime : {stats.finalDuration}");

        CancelInvoke("DestroyProjectile");

        Invoke("DestroyProjectile", stats.finalDuration);

        direction = (targetPosition - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);

            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);

            monster.ApplyKnockback(transform.position, knockbackForce);

            if (stats.pierceCount > 0) stats.pierceCount--;
            else DestroyProjectile();
        }
    }
}
