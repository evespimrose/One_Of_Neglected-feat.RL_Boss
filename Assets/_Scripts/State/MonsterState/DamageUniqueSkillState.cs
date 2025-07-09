using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUniqueSkillState : MonsterStateBase
{
    private float skillDuration = 1f;  // 스킬 상태 지속시간
    private float timer = 0f;          // 현재 타이머

    public DamageUniqueSkillState(StateHandler<MonsterBase> handler) : base(handler) { }
    public override void Enter(MonsterBase monster)
    {
        timer = 0f;
        monster.Animator?.SetTrigger("Skill");

        if (monster is DamageUniqueMonster damageMonster)
        {
            damageMonster.UseSkill();
        }
    }

    public override void Update(MonsterBase monster)
    {
        timer += Time.deltaTime;
        if (timer >= skillDuration)
        {
            handler.ChangeState(typeof(MonsterMoveState));
        }
    }

    public override void Exit(MonsterBase monster)
    {
        monster.Animator?.ResetTrigger("Skill");
    }
}
