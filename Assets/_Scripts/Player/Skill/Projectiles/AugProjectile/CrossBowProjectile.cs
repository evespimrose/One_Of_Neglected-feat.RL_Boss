using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowProjectile : PlayerProjectile
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision.TryGetComponent(out MonsterBase monster))
            {
                bool isCritical = UnityEngine.Random.value < stats.critical;
                float finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;
                monster.TakeDamage(finalFinalDamage);
                DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.CrossBow);

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
}
