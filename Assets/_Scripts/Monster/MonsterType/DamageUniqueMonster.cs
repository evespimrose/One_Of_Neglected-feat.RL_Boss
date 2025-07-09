using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUniqueMonster : NormalMonster
{
    [Header("스킬 설정")]
    [SerializeField] private float skillCooldown = 8f;     // 스킬 재사용 대기시간
    [SerializeField] private GameObject slashPrefab;       // 발사할 검기 프리팹
    [SerializeField] private float slashSpeed = 12f;       // 검기 이동 속도
    [SerializeField] private float slashDamage = 20f;      // 검기 공격력
    [SerializeField] private float skillRange = 5f;        // 스킬 사용 가능 범위
    private float skillTimer = 0f;
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 300f,
            speed: 1.2f,
            damage: 15f,
            range: 0.7f,
            cooldown: 1f,
            defense: 1f,
            regen: 1f,
            regenDelay: 1f
        );
    }
    protected override void InitializeStateHandler()
    {
        base.InitializeStateHandler();
        stateHandler.RegisterState(new DamageUniqueSkillState(stateHandler));
    }
    protected bool CanUseSkill()
    {
        if (skillTimer <= 0 && playerTransform != null)
        {
            // 플레이어가 스킬 범위 안에 있는지 확인
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            return distanceToPlayer <= skillRange;
        }
        return false;
    }
    protected override void Update()
    {
        // 스킬 쿨타임 감소
        if (skillTimer > 0)
        {
            skillTimer -= Time.deltaTime;
        }
        // 스킬 사용 조건 체크 및 스킬 상태로 전환
        else if (CanUseSkill())
        {
            stateHandler.ChangeState(typeof(DamageUniqueSkillState));
            skillTimer = skillCooldown;
            //Debug.Log($"[{gameObject.name}] 검기 스킬 사용!");
        }
        base.Update();
        if (stats.healthRegen > 0)
        {
            stats.RegenerateHealth(Time.deltaTime);
        }
    }
    public void UseSkill()
    {
        if (playerTransform == null) return;

        // 플레이어 방향으로의 단위 벡터 계산
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // 검기 프리팹 생성
        GameObject slash = Instantiate(slashPrefab, transform.position, Quaternion.identity);

        // 검기 회전 (플레이어 방향으로)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        slash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 검기 초기화 및 발사
        SlashProjectile projectile = slash.GetComponent<SlashProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction, slashSpeed, slashDamage);
        }
    }
}