using UnityEngine;
using System.Linq;

public class ArcherIdleState : BaseState<Player>
{
    public ArcherIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            player.targetPosition = new Vector2(worldPosition.x, worldPosition.y);
            handler.ChangeState(typeof(ArcherMoveState));
            return;
        }
        else if (horizontalInput != 0 || verticalInput != 0)
        {
            handler.ChangeState(typeof(ArcherMoveState));
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.CanDash())
            {
                handler.ChangeState(typeof(ArcherDashState));
                return;
            }
        }

        if (player.isAuto)
        {
            MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
            if (nearestMonster != null)
            {
                float distance = Vector2.Distance(player.transform.position, nearestMonster.transform.position);
                float attackStartRange = 3f;  
                float optimalRange = 2f;      

                if (distance <= attackStartRange)
                {
                    player.LookAtTarget(nearestMonster.transform.position);
                    handler.ChangeState(typeof(ArcherAttackState));
                    return;
                }
                else
                {
                    Vector2 directionToMonster = ((Vector2)nearestMonster.transform.position - (Vector2)player.transform.position).normalized;
                    Vector2 optimalPosition = (Vector2)nearestMonster.transform.position - (directionToMonster * optimalRange);
                    player.targetPosition = optimalPosition;
                    handler.ChangeState(typeof(ArcherMoveState));
                    return;
                }
            }
        }
        else
        {
            MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();

            if (nearestMonster != null)
            {
                float dist = Vector2.Distance(player.transform.position, nearestMonster.transform.position);
                bool hasLongBow = player.augment.activeAugments.Any(aug => aug is Aug_LongBow);

                if (hasLongBow || dist <= player.Stats.CurrentATKRange * 1.25f * 3)
                {
                    player.LookAtTarget(nearestMonster.transform.position);
                    handler.ChangeState(typeof(ArcherAttackState));
                    return;
                }
            }
        }
    }

    public override void Exit(Player player)
    {
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("Attack");
        player.Animator?.ResetTrigger("Dash");
        player.Animator?.ResetTrigger("IsMoving");
        player.Animator?.Update(0);
    }
}