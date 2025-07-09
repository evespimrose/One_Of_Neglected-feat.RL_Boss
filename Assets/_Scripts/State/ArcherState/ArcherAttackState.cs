using UnityEngine;
using Transform = UnityEngine.Transform;
using System.Linq;

public class ArcherAttackState : BaseState<Player>
{
    private const float BASE_ATTACK_DURATION = 0.5f;
    private float attackTimer;
    private bool hasDealtDamage = false;

    public ArcherAttackState(StateHandler<Player> handler) : base(handler) { }

    private float GetCurrentAttackDuration(Player player)
    {
        // 공격 속도가 1보다 클 때는 더 빠르게, 1보다 작을 때는 더 느리게
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

        Archer archer = player as Archer;
        if (archer != null && archer.AttackEffect != null)
        {
            archer.AttackEffect.SetActive(true);
            Animator effectAnimator = archer.AttackEffect.GetComponent<Animator>();
            if (effectAnimator != null)
            {
                effectAnimator.speed = animSpeedMultiplier;
                effectAnimator.Play("AttackEffect", 0, 0f);
            }

            bool isLookingRight = !player.Animator.GetComponent<SpriteRenderer>().flipX;
            UpdateEffectTransform(archer.AttackEffect, isLookingRight);
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
            if (nearestMonster != null)
            {
                player.LookAtTarget(nearestMonster.transform.position);
            }
        }

        // 공격 실행
        if (!hasDealtDamage && attackTimer >= currentAttackDuration * 0.5f)
        {
            bool isLookingRight = !player.Animator.GetComponent<SpriteRenderer>().flipX;
            Vector2 direction = isLookingRight ? Vector2.right : Vector2.left;
            Vector3 spawnPosition = player.transform.position + (Vector3)(direction * 0.2f);
           
            Vector3 targetPosition = UnitManager.Instance.GetNearestMonster()?.transform.position ?? 
                (player.transform.position + (Vector3)(direction * 10f));

            SoundManager.Instance.Play("Bow Attack 1", SoundManager.Sound.Effect);
            ProjectileManager.Instance.SpawnPlayerProjectile(
                "ArcherAttackProjectile",
                player.transform.position,
                targetPosition,
                1f,
                player.Stats.CurrentATK,
                10f,
                10f,
                0,
                5
            );

            player.InvokeAttackDetect(targetPosition);
            hasDealtDamage = true;
        }

        if (!player.IsAtDestination())
        {
            player.MoveTo(player.targetPosition);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.CanDash())
        {
            handler.ChangeState(typeof(ArcherDashState));
            return;
        }

        // 공격 완료 후 상태 전환
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
                    float attackStartRange = 3f;  
                    float optimalRange = 2f;      

                    if (distance <= attackStartRange)
                    {
                        handler.ChangeState(typeof(ArcherIdleState));
                    }
                    else
                    {
                        Vector2 directionToMonster = ((Vector2)nearestMonster.transform.position - (Vector2)player.transform.position).normalized;
                        player.targetPosition = (Vector2)nearestMonster.transform.position - (directionToMonster * optimalRange);
                        handler.ChangeState(typeof(ArcherMoveState));
                    }
                }
                else
                {
                    handler.ChangeState(typeof(ArcherIdleState));
                }
            }
            else
            {
                handler.ChangeState(typeof(ArcherIdleState));
            }
        }
    }

    public override void Exit(Player player)
    {
        if (player.Animator != null)
        {
            player.Animator.speed = 1f;
        }
        
        Archer archer = player as Archer;
        if (archer != null && archer.AttackEffect != null)
        {
            archer.AttackEffect.SetActive(false);
        }

        player.Animator?.ResetTrigger("Attack");
        player.Animator?.ResetTrigger("Idle");
        player.Animator?.ResetTrigger("IsMoving");

        player.Animator?.Update(0);
    }

    private void UpdateEffectTransform(GameObject effectObject, bool isLookingRight)
    {
        if (effectObject != null)
        {
            SpriteRenderer effectSprite = effectObject.GetComponent<SpriteRenderer>();
            Transform effectTransform = effectObject.transform;

            if (isLookingRight)
            {
                effectTransform.localPosition = new Vector3(-0.15f, -0.02f, 0);
                if (effectSprite != null)
                {
                    effectSprite.flipX = false;
                    effectSprite.flipY = false;
                }
            }
            else
            {
                effectTransform.localPosition = new Vector3(0.15f, -0.02f, 0);
                if (effectSprite != null)
                {
                    effectSprite.flipX = true;
                    effectSprite.flipY = false;
                }
            }
        }
    }

}