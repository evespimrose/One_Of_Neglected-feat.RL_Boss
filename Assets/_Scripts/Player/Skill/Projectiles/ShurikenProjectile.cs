using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShurikenProjectile : Projectile
{
    public int spin = 1;
    public int mul = 1;

    protected override void Start()
    {
        base.Start();
        RotateProjectileAsync(cts.Token).Forget();
    }

    protected virtual async UniTaskVoid RotateProjectileAsync(CancellationToken token)
    {
        while (isMoving)
        {
            if (!GameManager.Instance.isPaused)
            {
                transform.Rotate(0, 0, 360 / spin * mul * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);

            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);

            if (stats.pierceCount > 0)
            {
                stats.pierceCount--;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, maxDistance);
                MonsterBase closestMonster = null;
                float closestDistance = float.MaxValue;

                foreach (var col in colliders)
                {
                    if (col.TryGetComponent<MonsterBase>(out var nearbyMonster) && col != collision)
                    {
                        float distance = Vector2.Distance(transform.position, col.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestMonster = nearbyMonster;
                        }
                    }
                }

                if (closestMonster != null)
                {
                    targetPosition = closestMonster.transform.position;
                    direction = (targetPosition - transform.position).normalized;
                }
                else
                {
                    DestroyProjectile();
                }
            }
            else
            {
                DestroyProjectile();
            }
        }
    }
}
