using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdControlUniqueMonster : NormalMonster
{
    [Header("스킬 설정")]
    [SerializeField] private float skillCooldown = 8f;     // 스킬 재사용 대기시간
    [SerializeField] private float dashSpeed = 15f;        // 돌진 이동 속도
    [SerializeField] private float dashDuration = 0.5f;    // 돌진 지속시간
    [SerializeField] private float dashDamage = 25f;       // 돌진 충돌 데미지
    [SerializeField] private float skillRange = 6f;

    private float skillTimer = 0f;                         // 현재 스킬 쿨타임 타이머
    private Vector2 dashDirection;                         // 돌진 방향
    private bool isDashing = false;                        // 현재 돌진 중인지 여부
    private bool hasDamaged = false;

    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 500f,
            speed: 1.2f,
            damage: 15f,
            range: 0.7f,
            cooldown: 1f,
            defense: 1f,
            regen: 3f,
            regenDelay: 1f
        );
    }
    protected override void InitializeStateHandler()
    {
        base.InitializeStateHandler();
        stateHandler.RegisterState(new CCUniqueSkillState(stateHandler));
    }
    protected bool CanUseSkill()
    {
        if (skillTimer <= 0 && playerTransform != null && !isDashing)
        {
            // 플레이어가 스킬 범위 안에 있는지 확인
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            return distanceToPlayer <= skillRange;
        }
        return false;
    }
    public override void TakeDamage(float damage)
    {
        stats.currentHealth -= damage;
        ShowDamageFont(transform.position, damage, transform);
        SoundManager.Instance.Play("MonsterAttacked2", SoundManager.Sound.Effect, 1f, false, 0.3f);
        // 돌진 중이 아닐 때만 히트 애니메이션 재생
        if (!isDashing)
        {
            animator?.SetTrigger("Hit");
        }

        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    protected override void Update()
    {
        base.Update();
        if (stats.healthRegen > 0)
        {
            stats.RegenerateHealth(Time.deltaTime);
        }
        // 스킬 쿨타임 감소
        if (skillTimer > 0)
        {
            skillTimer -= Time.deltaTime;
        }
        // 스킬 사용 조건 체크 및 스킬 상태로 전환
        else if (CanUseSkill())
        {
            stateHandler.ChangeState(typeof(CCUniqueSkillState));
            skillTimer = skillCooldown;
            //Debug.Log($"[{gameObject.name}] 돌진 스킬 사용!");
        }
    }
    public void UseSkill()
    {
        if (playerTransform == null) return;
        StartCoroutine(DashCoroutine());
    }
    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        hasDamaged = false;
        dashDirection = (playerTransform.position - transform.position).normalized;

        float elapsedTime = 0f;
        Vector2 startPos = transform.position;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector2.Lerp(startPos,
                startPos + (dashDirection * dashSpeed),
                elapsedTime / dashDuration);
            if (!hasDamaged)  
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
                foreach (var hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        Player player = hit.GetComponent<Player>();
                        if (player != null)
                        {
                            player.TakeDamage(dashDamage);
                            hasDamaged = true;  
                            break;  
                        }
                    }
                }
            }

            yield return null;
        }

        isDashing = false;
    }
}