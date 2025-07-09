using UnityEngine;

public class WarriorMoveState : BaseState<Player>
{
    private bool isMouseMoving = false;

    public WarriorMoveState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("Dash");
        player.Animator?.ResetTrigger("IsMoving");
        
        player.Animator?.SetBool("IsMoving", true);
    }

    public override void Update(Player player)
    {
        if (player.isAuto)
        {
            MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
            if (nearestMonster != null)
            {
                float distance = Vector2.Distance(player.transform.position, nearestMonster.transform.position);
                float attackStartRange = 0.6f;  
                float optimalRange = 0.4f;      

                if (distance <= attackStartRange)
                {
                    handler.ChangeState(typeof(WarriorAttackState));
                    return;
                }
                else
                {
                    Vector2 directionToMonster = ((Vector2)nearestMonster.transform.position - (Vector2)player.transform.position).normalized;
                    player.targetPosition = (Vector2)nearestMonster.transform.position - (directionToMonster * optimalRange);
                    player.MoveTo(player.targetPosition);
                    player.LookAtTarget(nearestMonster.transform.position);
                }
            }
            else
            {
                handler.ChangeState(typeof(WarriorIdleState));
                return;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (player.CanDash())
                {
                    handler.ChangeState(typeof(WarriorDashState));
                    return;
                }
            }

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            bool hasKeyboardInput = horizontalInput != 0 || verticalInput != 0;

            if (hasKeyboardInput)
            {
                isMouseMoving = false;
                Vector2 moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
                player.transform.Translate(moveDirection * player.Stats.CurrentMspd * Time.deltaTime);
                
                if (horizontalInput != 0)
                {
                    player.FlipModel(horizontalInput < 0);
                }
            }
            else
            {
                if (Input.GetMouseButton(1))
                {
                    isMouseMoving = true;
                    Vector3 mousePosition = Input.mousePosition;
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    player.targetPosition = new Vector2(worldPosition.x, worldPosition.y);
                    
                    Vector2 direction = (player.targetPosition - (Vector2)player.transform.position).normalized;
                    if (direction.x != 0)
                    {
                        player.FlipModel(direction.x < 0);
                    }

                    player.MoveTo(player.targetPosition);
                }
                else if (isMouseMoving)
                {
                    if (!player.IsAtDestination())
                    {
                        player.MoveTo(player.targetPosition);
                    }
                    else
                    {
                        isMouseMoving = false;
                        handler.ChangeState(typeof(WarriorIdleState));
                        return;
                    }
                }
                else
                {
                    handler.ChangeState(typeof(WarriorIdleState));
                    return;
                }
            }
        }
    }

    public override void Exit(Player player)
    {
        isMouseMoving = false;
        if (!Input.GetMouseButton(1))
        {
            player.SetCurrentPositionAsTarget();
        }
        player.Animator?.SetBool("IsMoving", false);
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("Dash");
        player.Animator?.ResetTrigger("IsMoving");
        
        player.Animator?.Update(0);
    }
} 