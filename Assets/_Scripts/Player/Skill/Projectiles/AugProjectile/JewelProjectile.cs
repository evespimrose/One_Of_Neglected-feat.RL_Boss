using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

public class JewelProjectile : PlayerProjectile
{
    private HashSet<MonsterBase> monstersInRange = new HashSet<MonsterBase>();
    private float tickInterval = 0.5f;

    protected override void Start()
    {
        isMoving = true;
        transform.position = startPosition;
        cts = new CancellationTokenSource();
        ApplyDamageLoop(cts.Token).Forget();
    }

    //public override void InitProjectile(Vector3 startPos, Vector3 targetPos, float spd, float dmg, float maxDist = 0f, int pierceCnt = 0, float lifetime = 1.1f)
    //{
    //    startPosition = startPos;
    //    targetPosition = targetPos;
    //    speed = spd;
    //    maxDistance = maxDist;
    //    pierceCount = pierceCnt;
    //    lifeTime = lifetime;

    //    stats = new ProjectileStats
    //    {
    //        finalDamage = dmg,
    //        critical = 0.1f,
    //        cATK = 1.5f
    //    };

    //    CancelInvoke("DestroyProjectile");
    //    Invoke("DestroyProjectile", lifeTime);
    //}

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;
        stats = projectileStats;

        // 크기 설정
        transform.localScale = Vector3.one * stats.finalATKRange;

        CancelInvoke("DestroyProjectile");
        Invoke("DestroyProjectile", stats.finalDuration);
    }

    protected override async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        await UniTask.CompletedTask;
    }

    private async UniTaskVoid ApplyDamageLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (!GameManager.Instance.isPaused && monstersInRange.Count > 0)
            {
                foreach (var monster in monstersInRange.ToList())
                {
                    if (monster != null)
                    {
                        ApplyDamageToMonster(monster);
                    }
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(tickInterval), cancellationToken: token);
        }
    }

    private void ApplyDamageToMonster(MonsterBase monster)
    {
        float finalDamage = CalculateFinalDamage();
        monster.TakeDamage(finalDamage);

        DataManager.Instance.AddDamageData(finalDamage, Enums.AugmentName.Orb);
    }

    private float CalculateFinalDamage()
    {
        return UnityEngine.Random.value < stats.critical 
            ? stats.finalDamage * stats.cATK 
            : stats.finalDamage;
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
