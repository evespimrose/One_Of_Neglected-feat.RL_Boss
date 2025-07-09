using UnityEngine;

public class MagicianAttackState : BaseState<Player>
{
    private const float BASE_ATTACK_DURATION = 0.5f;
    private float attackTimer;
    private bool hasDealtDamage = false;

    public MagicianAttackState(StateHandler<Player> handler) : base(handler) { }

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

        Magician Magician = player as Magician;
        if (Magician != null && Magician.AttackEffect != null)
        {
            Magician.AttackEffect.SetActive(true);
            Animator effectAnimator = Magician.AttackEffect.GetComponent<Animator>();
            if (effectAnimator != null)
            {
                effectAnimator.speed = animSpeedMultiplier;
                effectAnimator.Play("AttackEffect", 0, 0f);
            }

            bool isLookingRight = !player.Animator.GetComponent<SpriteRenderer>().flipX;
            UpdateEffectTransform(Magician.AttackEffect, isLookingRight);
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

        if (!hasDealtDamage && attackTimer >= currentAttackDuration * 0.5f)
        {
            bool isLookingRight = !player.Animator.GetComponent<SpriteRenderer>().flipX;
            Vector2 direction = isLookingRight ? Vector2.right : Vector2.left;
            Vector3 spawnPosition = player.transform.position + (Vector3)(direction * 0.2f);

            Vector3 targetPosition = UnitManager.Instance.GetNearestMonster()?.transform.position ??
                (player.transform.position + (Vector3)(direction * 10f));

            SoundManager.Instance.Play("Fireball 1", SoundManager.Sound.Effect, 1f, false, 0.3f);
            ProjectileManager.Instance.SpawnPlayerProjectile(
                "MagicianAttackProjectile",
                player.transform.position,
                targetPosition,
                1f,
                player.Stats.CurrentATK,
                1,
                10f,
                0,
                5
            );

            hasDealtDamage = true;
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
                    float attackStartRange = 3f;
                    float optimalRange = 2f;

                    if (distance <= attackStartRange)
                    {
                        handler.ChangeState(typeof(MagicianIdleState));
                    }
                    else
                    {
                        Vector2 directionToMonster = ((Vector2)nearestMonster.transform.position - (Vector2)player.transform.position).normalized;
                        player.targetPosition = (Vector2)nearestMonster.transform.position - (directionToMonster * optimalRange);
                        handler.ChangeState(typeof(MagicianMoveState));
                    }
                }
                else
                {
                    handler.ChangeState(typeof(MagicianIdleState));
                }
            }
            else
            {
                handler.ChangeState(typeof(MagicianIdleState));
            }
        }

        if (!player.IsAtDestination())
        {
            player.MoveTo(player.targetPosition);
        }

        if (Input.GetKeyDown(KeyCode.Space) && player.CanDash())
        {
            handler.ChangeState(typeof(MagicianDashState));
            return;
        }
    }

    public override void Exit(Player player)
    {
        if (player.Animator != null)
        {
            player.Animator.speed = 1f;
        }

        Magician Magician = player as Magician;
        if (Magician != null && Magician.AttackEffect != null)
        {
            Magician.AttackEffect.SetActive(false);
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
                effectTransform.localPosition = new Vector3(0, -0.14f, 0);
                if (effectSprite != null)
                {
                    effectSprite.flipX = false;
                    effectSprite.flipY = false;
                }
            }
            else
            {
                effectTransform.localPosition = new Vector3(0, -0.14f, 0);
                if (effectSprite != null)
                {
                    effectSprite.flipX = true;
                    effectSprite.flipY = false;
                }
            }
        }
    }

}