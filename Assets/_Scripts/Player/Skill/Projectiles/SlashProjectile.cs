 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashProjectile : Projectile
{
    public Vector3 StartPosition { get; private set; }
    public void Initialize(Vector2 dir, float spd, float dmg)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)(dir * 10f); // 10f는 충분히 먼 거리
        StartPosition = startPos;
        stats.projectileSpeed = spd;

        // 기본 프로젝타일 초기화
        InitProjectile(
            startPos,           // 시작 위치
            targetPos,          // 목표 위치
            spd,               // 속도
            dmg,               // 데미지
            10f,               // 최대 거리
            0,                 // 관통 횟수
            5f                 // 생존 시간
        );
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌했을 때
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                //Debug.Log($"[SlashProjectile] 플레이어에게 {damage} 데미지!");
            }
            DestroyProjectile();
        }
    }
}