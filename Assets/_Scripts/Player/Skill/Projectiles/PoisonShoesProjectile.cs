using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System;

public class PoisonShoesProjectile : Projectile
{
    private HashSet<MonsterBase> monstersInRange = new HashSet<MonsterBase>();
    private float tickInterval = 0.3f;

    protected override void Start()
    {
        isMoving = true;
        transform.position = startPosition;
        cts = new CancellationTokenSource();
        MoveProjectileAsync(cts.Token).Forget();
    }

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;
        stats = projectileStats;

        CancelInvoke("DestroyProjectile");

        Invoke("DestroyProjectile", stats.finalDuration);
        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;

    }

    protected override async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!GameManager.Instance.isPaused)
            {
                foreach (var monster in monstersInRange.ToList())
                {
                    float finalFinalDamage = UnityEngine.Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

                    monster.TakeDamage(finalFinalDamage);

                    DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(tickInterval), cancellationToken: token);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster) && monstersInRange.Add(monster))
        {
            monster.OnDeath += RemoveMonsterFromSet;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            RemoveMonsterFromSet(monster);
        }
    }

    private void RemoveMonsterFromSet(MonsterBase monster)
    {
        monstersInRange.Remove(monster);
        monster.OnDeath -= RemoveMonsterFromSet;
    }
}
