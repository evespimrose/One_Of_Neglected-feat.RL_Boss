using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public struct MonsterStats
{
    public float maxHealth;     // 최대 체력
    public float currentHealth; // 현재 체력
    public float moveSpeed;     // 이동 속도
    public float attackDamage;  // 공격력
    public float attackRange;   // 공격 범위
    public float attackCooldown;// 공격 쿨다운
    public float defense;

    [Header("체력 회복")]
    public float healthRegen;    // 초당 체력 회복량
    public float regenDelay;     // 피격 후 회복 시작까지의 대기 시간

    public MonsterStats(float health, float speed, float damage,
        float range, float cooldown, float defense = 0f, float regen = 0f, float regenDelay = 1f)
    {
        maxHealth = currentHealth = health;
        moveSpeed = speed;
        attackDamage = damage;
        attackRange = range;
        attackCooldown = cooldown;
        this.defense = defense;
        healthRegen = regen;
        this.regenDelay = regenDelay;
    }
    // 체력 회복 메서드
    public void RegenerateHealth(float deltaTime)
    {
        if (healthRegen <= 0 || currentHealth >= maxHealth) return;

        float newHealth = currentHealth + (healthRegen * deltaTime);
        currentHealth = Mathf.Min(newHealth, maxHealth);
    }

    // 데미지 계산 메서드
    public float CalculateDamage(float incomingDamage)
    {
        return Mathf.Max(0, incomingDamage - defense);
    }
}