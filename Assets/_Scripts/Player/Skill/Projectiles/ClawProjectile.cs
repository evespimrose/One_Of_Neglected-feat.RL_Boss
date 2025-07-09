using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawProjectile : Projectile
{
    private SpriteRenderer myrenderer;

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        startPosition = startPos;
        targetPosition = targetPos;
        stats = projectileStats;

        CancelInvoke("DestroyProjectile");

        Invoke("DestroyProjectile", stats.finalDuration);

        direction = (targetPosition - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation *= Quaternion.AngleAxis(angle - 90, Vector3.forward);

        gameObject.transform.localScale = Vector3.one * stats.finalATKRange;
        DecideImage();
    }

    private void DecideImage()
    {
        myrenderer = GetComponentInChildren<SpriteRenderer>();
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        switch (stats.level)
        {
            case 1:
            case 2:
                myrenderer.sprite = Resources.Load<Sprite>("Using/Projectile/ClawLv1Sprite");
                Vector2 Lv1Offset = box.offset;
                Lv1Offset.x = 0.09f;
                box.offset = Lv1Offset;
                break;
            case 3:
            case 4:
            case 5:
                myrenderer.sprite = Resources.Load<Sprite>("Using/Projectile/ClawLv2Sprite");
                Vector2 Lv2Offset = box.offset;
                Lv2Offset.x = 0.15f;
                box.offset = Lv2Offset;
                break;
            case 6:
                myrenderer.sprite = Resources.Load<Sprite>("Using/Projectile/ClawLv3Sprite");
                Vector2 Lv3Offset = box.offset;
                Lv3Offset.x = 0.27f;
                box.offset = Lv3Offset;
                break;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<MonsterBase>(out var monster))
        {
            float finalFinalDamage = Random.value < stats.critical ? stats.finalDamage * stats.cATK : stats.finalDamage;

            monster.TakeDamage(finalFinalDamage);
            DataManager.Instance.AddDamageData(finalFinalDamage, stats.skillName);
            GameObject clawEffect = Instantiate(Resources.Load<GameObject>("Using/Projectile/ClawEffect"),monster.gameObject.transform.position, Quaternion.identity);
            Destroy(clawEffect, 1.083f);
        }
    }
}
