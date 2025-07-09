using UnityEngine;

public class MagicianAttackProjectile : PlayerProjectile
{
    protected override void Start()
    {
        base.Start();
        Vector3 direction = (targetPosition - startPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
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

                DestroyProjectile();
            }
        }
    }
}
