using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedMonster : MonsterBase
{
    [Header("?먭굅由?怨듦꺽 ?ㅼ젙")]
    [SerializeField] protected string projectileType = "RangedNormal";
    [SerializeField] protected float projectileSpeed = 5f;

    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<MonsterBase>(this);
        //stateHandler.RegisterState(new MonsterIdleState(stateHandler));
        stateHandler.RegisterState(new MonsterMoveState(stateHandler));
        stateHandler.RegisterState(new RangedAttackState(stateHandler));
        stateHandler.RegisterState(new MonsterDieState(stateHandler));
        stateHandler.ChangeState(typeof(MonsterMoveState));
    }
    public virtual void RangedAttack()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            ProjectileManager.Instance.SpawnMonsterProjectile(
                projectileType,
                transform.position,
                direction,
                projectileSpeed,
                stats.attackDamage
                );
        }
    }
}