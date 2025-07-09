using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float lifetime = 5f;  // 투사체 최대 지속 시간
    private float timer = 0f;

    public void Initialize(Vector3 direction, float speed, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // 최대 지속 시간 초과시 제거
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 지정된 방향으로 이동
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                // Debug.Log($"[ProjectileMovement] 플레이어에게 {damage} 데미지!");
            }
            Destroy(gameObject);
        }
    }
}
