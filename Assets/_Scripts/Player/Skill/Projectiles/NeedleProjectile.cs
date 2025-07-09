using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class NeedleProjectile : Projectile
{

    protected override void Start()
    {
        isMoving = true;
        transform.position = targetPosition;
    }

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;

        stats = projectileStats;

        CancelInvoke("DestroyProjectile");
        Invoke("DestroyProjectile", stats.finalDuration);

        direction = (targetPosition - startPos).normalized;
        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;

    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = UnityEngine.Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);
            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);
        }
    }
}
