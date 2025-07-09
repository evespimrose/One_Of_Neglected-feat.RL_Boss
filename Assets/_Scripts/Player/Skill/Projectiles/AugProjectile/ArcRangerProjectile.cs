using UnityEngine;

public class ArcRangerProjectile : PlayerProjectile
{
    private bool isExplosive = false;
    private float baseExplosionRadius = 1f;
    
    [SerializeField] private GameObject explosionEffectPrefab;
    private static GameObject cachedEffectPrefab; 

    public void SetExplosive(bool explosive)
    {
        isExplosive = explosive;
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
                bool isCritical = UnityEngine.Random.value < stats.critical;
                float finalFinalDamage = isCritical ? stats.finalDamage * stats.cATK : stats.finalDamage;
                
                monster.TakeDamage(finalFinalDamage);
                DataManager.Instance.AddDamageData(finalFinalDamage, Enums.AugmentName.ArcRanger);

                if (isExplosive)
                {
                    Vector3 hitPoint = collision.ClosestPoint(transform.position);
                    Explode(finalFinalDamage, hitPoint);
                    SoundManager.Instance.Play("ArcBoom", SoundManager.Sound.Effect, 1f, false, 0.1f);
                }

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

    private void Explode(float damage, Vector3 position)
    {
        float explosionRadius = baseExplosionRadius * stats.finalATKRange;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, explosionRadius);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Monster") && collider.TryGetComponent(out MonsterBase monster))
            {
                
                float explosionDamage = damage * 0.3f;
                monster.TakeDamage(explosionDamage);
                DataManager.Instance.AddDamageData(explosionDamage, Enums.AugmentName.ArcRanger);
            }
        }

        SpawnExplosionEffect(position);
    }

    private void SpawnExplosionEffect(Vector3 position)
    {
        if (cachedEffectPrefab == null)
        {
            cachedEffectPrefab = Resources.Load<GameObject>("Using/Effect/ArcExEffect");
        }

        if (cachedEffectPrefab != null)
        {
            GameObject effect = Instantiate(cachedEffectPrefab, position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * (baseExplosionRadius * stats.finalATKRange);
            Destroy(effect, 1f);
        }
    }
}
