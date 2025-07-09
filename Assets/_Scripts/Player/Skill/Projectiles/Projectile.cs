using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using static Enums;
using System;

public class Projectile : MonoBehaviour
{
    public float speed;
    protected float damage;
    public float maxDistance = 10f;
    protected Vector3 startPosition;
    protected Vector3 targetPosition;
    protected bool isMoving;
    protected float lifeTime = 5f;

    protected ProjectileStats stats;

    protected Vector3 direction;

    protected CancellationTokenSource cts;
    protected virtual void Start()
    {
        isMoving = true;
        transform.position = startPosition;
        cts = new CancellationTokenSource();
        MoveProjectileAsync(cts.Token).Forget();
    }

    protected virtual async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        try
        {
            float traveledDistance = 0f;

            while (isMoving && !token.IsCancellationRequested)
            {
                if (gameObject == null || !isMoving)
                {
                    break;
                }

                if (!GameManager.Instance.isPaused)
                {
                    transform.position += stats.projectileSpeed * Time.deltaTime * direction;

                    traveledDistance = (transform.position - startPosition).magnitude;

                    if (traveledDistance >= maxDistance)
                    {
                        DestroyProjectile();
                        break;
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
                else
                {
                    await UniTask.Yield(token);
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    public virtual void InitProjectile(Vector3 startPos, Vector3 targetPos, float spd, float dmg, float maxDist = 0f, int pierceCnt = 0, float lifetime = 5f)
    {
        startPosition = startPos;
        targetPosition = targetPos;
        speed = spd;
        maxDistance = maxDist;
        damage = dmg;
        lifeTime = lifetime;

        CancelInvoke("DestroyProjectile");

        Invoke("DestroyProjectile", lifeTime);

        direction = (targetPosition - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public virtual void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;

        stats = projectileStats;

        CancelInvoke("DestroyProjectile");

        Invoke("DestroyProjectile", stats.finalDuration);

        direction = (targetPosition - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = UnityEngine.Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);

            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);

            if (stats.pierceCount > 0) stats.pierceCount--;
            else DestroyProjectile();
        }
    }

    public virtual void DestroyProjectile()
    {
        if (!isMoving || gameObject == null)
        {
            return;
        }

        isMoving = false;

        CancelInvoke("DestroyProjectile");

        cts?.Cancel();
        cts?.Dispose();
        cts = null;

        ProjectileManager.Instance?.RemoveProjectile(this);

        Destroy(gameObject);
    }
}