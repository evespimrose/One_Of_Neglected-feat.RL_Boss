using UnityEngine;

public class RushProjectile : PlayerProjectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision.TryGetComponent(out MonsterBase monster))
            {
                bool isCritical = UnityEngine.Random.value < stats.critical;
                float finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;
                monster.TakeDamage(finalFinalDamage);
                DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.Shielder);
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
