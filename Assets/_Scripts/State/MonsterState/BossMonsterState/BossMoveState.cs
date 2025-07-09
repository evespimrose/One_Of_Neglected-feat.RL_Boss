using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMoveState : MonsterStateBase
{
    public BossMoveState(StateHandler<MonsterBase> handler) : base(handler)
    {
    }

    public override void Enter(MonsterBase entity)
    {
        entity.Animator?.SetBool("IsMoving", true);
    }

    public override void Update(MonsterBase entity)
    {
        // 플레이어가 공격 범위 안에 있으면 기본 공격으로 전환
        if (entity.IsPlayerInAttackRange())
        {
            handler.ChangeState(typeof(BossBasicAttackState));
            return;
        }

        // 플레이어 추적
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = (player.transform.position - entity.transform.position).normalized;
            entity.transform.position += direction * entity.Stats.moveSpeed * Time.deltaTime;

            // 이동 애니메이션
            entity.Animator?.SetBool("IsMoving", true);
        }
    }

    public override void Exit(MonsterBase entity)
    {
        entity.Animator?.SetBool("IsMoving", false);
    }
}
