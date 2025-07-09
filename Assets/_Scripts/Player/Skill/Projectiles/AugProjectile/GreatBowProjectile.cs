using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class GreatBowProjectile : PlayerProjectile
{
    private int additionalHits = 0;
    private float additionalHitDelay = 0.1f;

    public void SetAdditionalHits(int hits)
    {
        additionalHits = hits;
    }

    protected override void Start()
    {
        base.Start();
        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision.TryGetComponent(out MonsterBase monster))
            {
                ApplyDamage(monster);

                if (stats.pierceCount > 0)
                {
                    stats.pierceCount--;
                }
                else
                {
                    DestroyProjectile();
                }
            }
        }
    }

    private async void ApplyDamage(MonsterBase monster)
    {
        bool isCritical = UnityEngine.Random.value < stats.critical;
        float finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;
        
        monster.TakeDamage(finalFinalDamage);
        DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.GreatBow);

        for (int i = 0; i < additionalHits; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(additionalHitDelay));
            if (monster != null && monster.gameObject != null) //살아있으면 추가타 들어가게
            {
                isCritical = UnityEngine.Random.value < stats.critical;
                finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;
                
                monster.TakeDamage(finalFinalDamage);
                DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.GreatBow);
            }
        }
    }
}
