using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : MonsterStateBase
{
    private float attackTimer = 0f;

    public MonsterAttackState(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase entity)
    {
        attackTimer = 0f;
        // 이동 애니메이션 중지
        entity.Animator?.SetBool("IsMoving", false);
    }

    public override void Update(MonsterBase entity)
    {
        if (!entity.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(MonsterMoveState));
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= entity.Stats.attackCooldown)
        {
            PerformAttack(entity);
            attackTimer = 0f;
        }
    }

    public override void Exit(MonsterBase entity)
    {
        entity.Animator?.SetBool("IsMoving", true);
        entity.Animator?.ResetTrigger("Attack");
    }

    private void PerformAttack(MonsterBase entity)
    {
        // 공격 시도 전에 다시 한번 거리 체크
        if (!entity.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(MonsterMoveState));
            return;
        }

        entity.Animator?.SetTrigger("Attack");
        Player player = UnitManager.Instance.GetPlayer();
        if (player != null)
        {
            // 몬스터의 공격력으로 플레이어에게 데미지
            player.TakeDamage(entity.Stats.attackDamage);
            //Debug.Log($"Monster deals {entity.Stats.attackDamage} damage to player!");
        }
    }
}
