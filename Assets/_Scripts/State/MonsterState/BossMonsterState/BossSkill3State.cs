using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill3State : MonsterStateBase
{
    public BossSkill3State(StateHandler<MonsterBase> handler) : base(handler)
    {
    }

    private float duration = 3f;
    private float timer = 0f;

    public override void Enter(MonsterBase entity)
    {
        timer = 0f;
        entity.Animator?.SetTrigger("Skill3");
       // Debug.Log("[Boss] 스킬3 시작");
    }

    public override void Update(MonsterBase entity)
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            handler.ChangeState(typeof(BossMoveState));
        }
    }

    public override void Exit(MonsterBase entity)
    {
        //Debug.Log("[Boss] 스킬3 종료");
    }
}
