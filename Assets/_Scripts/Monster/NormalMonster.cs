using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NormalMonster : MonsterBase
{
    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<MonsterBase>(this);
        //stateHandler.RegisterState(new MonsterIdleState(stateHandler));
        stateHandler.RegisterState(new MonsterMoveState(stateHandler));
        stateHandler.RegisterState(new MonsterAttackState(stateHandler));
        stateHandler.RegisterState(new MonsterDieState(stateHandler));
        stateHandler.ChangeState(typeof(MonsterMoveState));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.gameObject == null || stateHandler == null) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            stateHandler.ChangeState(typeof(MonsterAttackState));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision == null || collision.gameObject == null || stateHandler == null) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            stateHandler.ChangeState(typeof(MonsterMoveState));
        }
    }
}