using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackState : MonsterStateBase
{
    private float attackTimer = 0f;
    private float attackDelay = 0.5f;  // 애니메이션과 실제 공격 사이의 딜레이

    public RangedAttackState(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase monster)
    {
        attackTimer = 0f;
        // 이동 애니메이션 중지
        monster.Animator?.SetBool("IsMoving", false);
    }

    public override void Update(MonsterBase monster)
    {
        // 플레이어가 공격 범위를 벗어나면 즉시 이동 상태로 전환
        if (!monster.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(MonsterMoveState));
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= monster.Stats.attackCooldown)
        {
            PerformRangedAttack(monster);
            attackTimer = 0f;
        }
    }

    public override void Exit(MonsterBase monster)
    {
        monster.Animator?.SetBool("IsMoving", true);
        monster.Animator?.ResetTrigger("RangedAttack");
    }

    private void PerformRangedAttack(MonsterBase monster)
    {
        // 공격 시도 전에 다시 한번 거리 체크
        if (!monster.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(MonsterMoveState));
            return;
        }

        monster.Animator?.SetTrigger("RangedAttack");

        // RangedMonster인 경우에만 원거리 공격 실행
        if (monster is RangedMonster rangedMonster)
        {
            // attackDelay 후에 실제 공격 실행
            monster.StartCoroutine(DelayedRangedAttack(rangedMonster));
        }
    }

    private IEnumerator DelayedRangedAttack(RangedMonster rangedMonster)
    {
        yield return new WaitForSeconds(attackDelay);
        if (rangedMonster != null && rangedMonster.IsPlayerInAttackRange())
        {
            rangedMonster.RangedAttack();
        }
    }
}