using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBasicAttackState : MonsterStateBase
{
    private float attackTimer = 0f;
    private const float ATTACK_COOLDOWN = 1f;

    public BossBasicAttackState(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase entity)
    {
        attackTimer = ATTACK_COOLDOWN;
        entity.Animator?.SetBool("IsMoving", false);
    }

    public override void Update(MonsterBase entity)
    {
        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

        // 플레이어가 없거나 죽었으면 즉시 이동 상태로 전환
        if (player == null || player.Stats.currentHp <= 0)
        {
            handler.ChangeState(typeof(BossMoveState));
            return;
        }

        // 플레이어가 공격 범위를 벗어나면 이동 상태로
        if (!entity.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(BossMoveState));
            return;
        }

        attackTimer += Time.deltaTime;

        // 쿨타임이 찼을 때만 공격
        if (attackTimer >= ATTACK_COOLDOWN)
        {
            // 공격 직전에 다시 한번 플레이어 상태 체크
            if (player != null && player.Stats.currentHp > 0)
            {
                entity.Animator?.SetTrigger("Attack");
                PerformAttack(entity);
                attackTimer = 0f;
            }
            else
            {
                handler.ChangeState(typeof(BossMoveState));
            }
        }
    }

    private void PerformAttack(MonsterBase entity)
    {
        BossMonsterBase boss = entity as BossMonsterBase;
        if (boss == null) return;

        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();
        if (player != null && player.Stats.currentHp > 0)
        {
            float damage = boss.Stats.attackDamage;
            player.TakeDamage(damage);
            //Debug.Log($"[Boss] 데미지 적용: {damage}");
        }
    }


    public override void Exit(MonsterBase entity)
    {
        entity.Animator?.SetBool("IsMoving", true);
        entity.Animator?.ResetTrigger("Attack");
    }
}
