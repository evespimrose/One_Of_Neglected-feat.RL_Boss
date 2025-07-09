using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : MonsterStateBase
{
    public MonsterMoveState(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase monster)
    {
        monster.Animator?.SetBool("IsMoving", true);
    }

    public override void Update(MonsterBase monster)
    {
        monster.MoveTowardsPlayer();
    }

    public override void Exit(MonsterBase monster)
    {
        monster.Animator?.SetBool("IsMoving", false);
    }
}