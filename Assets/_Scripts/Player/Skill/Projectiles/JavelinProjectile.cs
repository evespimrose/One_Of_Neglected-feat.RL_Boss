using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinProjectile : Projectile
{

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);
            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);

            if (stats.pierceCount > 0) stats.pierceCount--;
            else DestroyProjectile();
        }

        if(stats.canParry)
        {
            if (collision.TryGetComponent<MonsterProjectile>(out var monsterProjectile))
            {
                var reflectedStats = new ProjectileStats
                {
                    projectileSpeed = stats.projectileSpeed,
                    finalDamage = UnitManager.Instance.GetPlayer().Stats.CurrentATK,
                    finalATKRange = stats.finalATKRange,
                    pierceCount = stats.pierceCount,
                    finalDuration = stats.finalDuration,
                    critical = stats.critical,
                    cATK = stats.cATK,
                    skillName = stats.skillName
                };

                ProjectileManager.Instance.SpawnPlayerProjectile(
                    "ReflectedMonsterProjectile",
                    monsterProjectile.gameObject.transform.position,
                    monsterProjectile.StartPosition,
                    reflectedStats
                );

                monsterProjectile.DestroyProjectile();
            }
            if (collision.TryGetComponent<SlashProjectile>(out var slashProjectile))
            {
                var reflectedStats = new ProjectileStats
                {
                    projectileSpeed = stats.projectileSpeed,
                    finalDamage = UnitManager.Instance.GetPlayer().Stats.CurrentATK,
                    finalATKRange = stats.finalATKRange,
                    pierceCount = stats.pierceCount,
                    finalDuration = stats.finalDuration,
                    critical = stats.critical,
                    cATK = stats.cATK,
                    skillName = stats.skillName
                };

                ProjectileManager.Instance.SpawnPlayerProjectile(
                    "ReflectedSlashProjectile",
                    slashProjectile.gameObject.transform.position,
                    slashProjectile.StartPosition,
                    reflectedStats
                );

                slashProjectile.DestroyProjectile();
            }
        }
    }
}
