using UnityEngine;

public class WarriorAttackState : BaseState<Player>
{
    private const float BASE_ATTACK_DURATION = 0.5f; 
    private float attackTimer;
    private bool hasDealtDamage = false; 

    public WarriorAttackState(StateHandler<Player> handler) : base(handler) { }

    private float GetCurrentAttackDuration(Player player)
    {
        return BASE_ATTACK_DURATION / player.Stats.CurrentAspd;
    }

    public override void Enter(Player player)
    {
        attackTimer = 0f;
        hasDealtDamage = false;  
        
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("Attack");
        player.Animator?.ResetTrigger("IsMoving");
        player.Animator?.Update(0);

        float animSpeedMultiplier = player.Stats.CurrentAspd;
        if (player.Animator != null)
        {
            player.Animator.speed = animSpeedMultiplier;
            player.Animator.SetTrigger("Attack");
        }

        MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
        if (nearestMonster != null)
        {
            player.LookAtTarget(nearestMonster.transform.position);
        }
    }

    public override void Update(Player player)
    {
        float currentAttackDuration = GetCurrentAttackDuration(player);
        attackTimer += Time.deltaTime;

        if (player.isAuto)
        {
            MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
            if (Vector2.Distance(player.transform.position, nearestMonster.transform.position) < 0.3f)
                if (nearestMonster != null)
                {
                    player.LookAtTarget(nearestMonster.transform.position);
                }
        }

        if (!hasDealtDamage && attackTimer >= currentAttackDuration * 0.5f)
        {
            bool isLookingRight = !player.Animator.GetComponent<SpriteRenderer>().flipX;
            Vector2 direction = isLookingRight ? Vector2.right : Vector2.left;
            Vector3 spawnPosition = player.transform.position + (Vector3)(direction * 0.2f);
            Vector3 targetPosition = spawnPosition + (Vector3)(direction * 1f);

            SoundManager.Instance?.Play("Sword Attack 2", SoundManager.Sound.Effect);
            ProjectileManager.Instance.SpawnPlayerProjectile(
                "WarriorAttackProjectile",
                spawnPosition,
                targetPosition,
                0f,
                player.Stats.CurrentATK,
                1
            );
            
            hasDealtDamage = true;
        }

        if (!player.IsAtDestination())
        {
            player.MoveTo(player.targetPosition);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.CanDash())
        {
            handler.ChangeState(typeof(WarriorDashState));
            return;
        }

        if (attackTimer >= currentAttackDuration)
        {
            attackTimer = 0;
            hasDealtDamage = false;

            if (player.isAuto)
            {
                MonsterBase nearestMonster = UnitManager.Instance.GetNearestMonster();
                if (nearestMonster != null)
                {
                    float distance = Vector2.Distance(player.transform.position, nearestMonster.transform.position);

                    if (distance <= player.Stats.CurrentATKRange * 0.7f)
                    {
                        handler.ChangeState(typeof(WarriorIdleState));
                        return;
                    }
                    else
                    {
                        player.targetPosition = nearestMonster.transform.position;
                        handler.ChangeState(typeof(WarriorMoveState));
                        return;
                    }
                }
            }

            if (!player.IsAtDestination())
            {
                handler.ChangeState(typeof(WarriorMoveState));
            }
            else
            {
                handler.ChangeState(typeof(WarriorIdleState));
            }
        }
    }

    public override void Exit(Player player)
    {
        if (player.Animator != null)
        {
            player.Animator.speed = 1f;
        }
        
        player.Animator?.ResetTrigger("Attack");
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("IsMoving");
        
        player.Animator?.Update(0);
    }
}
