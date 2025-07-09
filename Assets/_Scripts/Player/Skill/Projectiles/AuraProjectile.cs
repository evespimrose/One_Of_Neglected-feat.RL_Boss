using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using System.Linq;

public class AuraProjectile : Projectile
{
    private HashSet<MonsterBase> monstersInRange = new HashSet<MonsterBase>();
    private float tickInterval = 1f;

    protected override void Start()
    {
        isMoving = true;
        transform.SetParent(UnitManager.Instance.GetPlayer().transform);
        transform.localPosition = Vector3.zero;
        cts = new CancellationTokenSource();
        MoveProjectileAsync(cts.Token).Forget();
        ApplyDamageLoop(cts.Token).Forget();
    }
    protected override async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        while (isMoving)
        {
            if (!GameManager.Instance.isPaused)
            {
                transform.Rotate(0, 0, 360 * 0.03f * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    private async UniTaskVoid ApplyDamageLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            SoundManager.Instance.Play(stats.skillName.ToString(), SoundManager.Sound.Effect, 1f, false, 0.5f);

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
            if (monstersInRange.Contains(monster))
            {
                RemoveMonsterFromSet(monster);
            }
        }
    }

    private void RemoveMonsterFromSet(MonsterBase monster)
    {
        monstersInRange.Remove(monster);
        monster.OnDeath -= RemoveMonsterFromSet;
    }

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;
        stats = projectileStats;

        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;
    }
}
