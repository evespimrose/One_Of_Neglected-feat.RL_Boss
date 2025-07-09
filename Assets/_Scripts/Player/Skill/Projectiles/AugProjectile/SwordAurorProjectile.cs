using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAurorProjectile : PlayerProjectile
{
    private bool isProjBreak = false;
    protected override void Start()
    {
        base.Start();
    }

    public void SetProjBreak(bool projBreak)
    {
        isProjBreak = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.TryGetComponent<MonsterProjectile>(out var monsterProjectile))
        {
            if (isProjBreak)
            {
                monsterProjectile.Invoke("DestroyProjectile", 0f);
            }
        }

        if (collision.CompareTag("Monster"))
        {
            if (collision.TryGetComponent(out MonsterBase monster))
            {
                bool isCritical = UnityEngine.Random.value < stats.critical;
                float finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;

                monster.TakeDamage(finalFinalDamage);
                DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.TwoHandSword);
                

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
