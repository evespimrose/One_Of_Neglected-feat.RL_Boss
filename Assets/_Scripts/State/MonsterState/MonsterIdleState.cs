//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MonsterIdleState : MonsterStateBase
//{
//    private float idleDuration = 1f;
//    private float idleTimer = 0f;

//    public MonsterIdleState(StateHandler<MonsterBase> handler) : base(handler) { }

//    public override void Enter(MonsterBase monster)
//    {
//        monster.Animator?.SetBool("IsMoving", false);
//        monster.Animator?.SetTrigger("Idle");
//        idleTimer = 0f;
//    }

//    public override void Update(MonsterBase monster)
//    {
//        idleTimer += Time.deltaTime;
//        if (idleTimer >= idleDuration)
//        {
//            handler.ChangeState(typeof(MonsterMoveState));
//        }
//    }

//    public override void Exit(MonsterBase monster)
//    {
//        monster.Animator?.ResetTrigger("Idle");
//    }
//}