using UnityEngine;

public class SwordShieldProjectile : PlayerProjectile
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

                float finalDamage = finalFinalDamage * ((UnitManager.Instance.GetPlayer().Stats.CurrentDefense * 10) + 100) / 100;
                monster.TakeDamage(finalDamage);
                DataManager.Instance.AddDamageData(finalDamage, Enums.AugmentName.SwordShield);

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
