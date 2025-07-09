using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : MonsterStateBase
{
    public MonsterDieState(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase monster)
    {
        monster.StopAllCoroutines();

        monster.Animator?.SetTrigger("Die");

        if (monster.TryGetComponent<Collider2D>(out var collider))
            collider.enabled = false;
    }

    public override void Update(MonsterBase monster)
    {
    }

    public override void Exit(MonsterBase monster)
    {
    }
}
