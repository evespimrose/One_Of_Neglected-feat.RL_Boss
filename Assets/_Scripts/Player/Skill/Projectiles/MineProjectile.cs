using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MineProjectile : Projectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            GameObject explosion = Instantiate(Resources.Load<GameObject>("Using/Projectile/MineExplosion"), transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * stats.finalATKRange;
            Destroy(explosion, 1.0f);

            float explosionRadius = 0.5f * stats.finalATKRange;

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent<MonsterBase>(out var hitMonster))
                {
                    float finalFinalDamage = UnityEngine.Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

                    hitMonster.TakeDamage(finalFinalDamage);
                    GameObject dmgEffect = Instantiate(Resources.Load<GameObject>("Using/Projectile/MineDamageEffect"), hitMonster.gameObject.transform.position, Quaternion.identity);
                    dmgEffect.transform.localScale = Vector3.one * stats.finalATKRange;
                    Destroy(dmgEffect, 0.917f);

                    DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);
                }
            }
            DestroyProjectile();
        }
    }


    protected override async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        try
        {
            float elapsedTime = 0f;
            float exitTime = UnityEngine.Random.Range(0.7f, 1.05f);
            float gravity = 9.81f;
            float initialSpeed = stats.projectileSpeed;

            Vector3 displacement = targetPosition - startPosition;
            float distance = displacement.magnitude;

            float angle;
            if (UnityEngine.Random.value < 0.5f)
                angle = UnityEngine.Random.Range(24f, 30f);
            else
                angle = UnityEngine.Random.Range(110f, 120f);

            float radianAngle = angle * Mathf.Deg2Rad;

            float vx = Mathf.Cos(radianAngle) * initialSpeed;
            float vy = Mathf.Sin(radianAngle) * initialSpeed;
            if (angle < 100) vy *= 10f;
            else vy *= 5f;

            while (!token.IsCancellationRequested)
            {
                if (gameObject == null)
                {
                    break;
                }

                if (!GameManager.Instance.isPaused)
                {
                    elapsedTime += Time.deltaTime;

                    float newX = startPosition.x + vx * elapsedTime;
                    float newY = startPosition.y + vy * elapsedTime - 0.5f * gravity * elapsedTime * elapsedTime;

                    transform.position = new Vector3(newX, newY, startPosition.z);

                    if (elapsedTime >= exitTime)
                    {
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
            print(ex.ToString() + " " + ex.StackTrace.ToString());
        }
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

    public override void DestroyProjectile()
    {
        SoundManager.Instance.Play(stats.skillName.ToString(), SoundManager.Sound.Effect, 1f, false, 0.8f);

        GameObject explosion = Instantiate(Resources.Load<GameObject>("Using/Projectile/MineExplosion"), transform.position, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * stats.finalATKRange;
        Destroy(explosion, 1.0f);

        float explosionRadius = 0.5f * stats.finalATKRange;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<MonsterBase>(out var hitMonster))
            {
                float finalFinalDamage = UnityEngine.Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

                hitMonster.TakeDamage(finalFinalDamage);
                GameObject dmgEffect = Instantiate(Resources.Load<GameObject>("Using/Projectile/MineDamageEffect"), hitMonster.gameObject.transform.position, Quaternion.identity);
                dmgEffect.transform.localScale = Vector3.one * stats.finalATKRange;
                Destroy(dmgEffect, 0.917f);

                DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);
            }
        }
        base.DestroyProjectile();
    }
}
