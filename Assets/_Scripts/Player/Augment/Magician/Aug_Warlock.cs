using UnityEngine;

public class Aug_Warlock : ConditionalAugment
{
    private float teleportDistance = 1f; 
    private bool teleported = false;
    private GameObject teleportEffectPrefab;

    private float damageMultiplier = 1f;
    private float baseProjectileSize = 1f;
    private int penetration = 100;
    private float maxDistance = 10f;

    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    private PlayerProjectile currentPathProjectile;

    public Aug_Warlock(Player owner) : base(owner)
    {
        aguName = Enums.AugmentName.Warlock;
        teleportEffectPrefab = Resources.Load<GameObject>("Using/Effect/TeleportEffect");
    }

    public override void Activate()
    {
        base.Activate();
        owner.dashDetect += OnDashDetected;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        owner.dashDetect -= OnDashDetected;

        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
            currentPathProjectile = null;
        }
    }

    public override bool CheckCondition()
    {
        return true; 
    }

    public override void OnConditionDetect()
    {
    }

    private void OnDashDetected()
    {
        if (CheckCondition())
        {
            OnConditionDetect();

            if (owner is Player player)
            {
                teleported = true;

                if (teleportEffectPrefab != null)
                {
                    GameObject startEffect = GameObject.Instantiate(teleportEffectPrefab, player.transform.position, Quaternion.identity);
                    GameObject.Destroy(startEffect, 1f);
                }

                float horizontalInput = Input.GetAxisRaw("Horizontal");
                float verticalInput = Input.GetAxisRaw("Vertical");
                Vector2 direction;

                if (horizontalInput != 0 || verticalInput != 0)
                {
                    direction = new Vector2(horizontalInput, verticalInput).normalized;
                }
                else
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    direction = (mousePosition - (Vector2)player.transform.position).normalized;
                }

                Vector2 targetPosition = (Vector2)player.transform.position + (direction * teleportDistance);
                player.transform.position = targetPosition;
                player.SetCurrentPositionAsTarget();

                if (teleportEffectPrefab != null)
                {
                    GameObject endEffect = GameObject.Instantiate(teleportEffectPrefab, targetPosition, Quaternion.identity);
                    GameObject.Destroy(endEffect, 0.5f);
                }

                SpawnshockwaveProjectile();

                player.SetDashing(false);
                player.SetSkillInProgress(false, false);
                player.Animator?.SetBool("IsMoving", false);
                player.Animator?.SetTrigger("Idle");

                player.stateHandler.ChangeState(typeof(MagicianIdleState));
            }
        }
    }
    private void SpawnshockwaveProjectile()
    {
        SoundManager.Instance.Play("ShockWave", SoundManager.Sound.Effect, 1.0f, false, 0.3f);
        Vector2 dashEnd = owner.targetPosition;
        ProjectileManager.Instance.SpawnPlayerProjectile(
            "WarlockShockProjectile",
            dashEnd,
            dashEnd,
            0f,
            CurrentDamage * 0.2f,
            CurrentProjectileSize * 2,
            maxDistance,
            penetration,
            0.5f);
    }

    public bool WasTeleported()
    {
        if (teleported)
        {
            teleported = false;
            return true;
        }
        return false;
    }

    protected override void OnLevelUp()
    {
        base.OnLevelUp();
        switch (level)
        {
            case 1:
                break;
            case 2:
                owner.dashRechargeTime *= 0.8f;
                break;
            case 3:
                baseProjectileSize += 0.1f;
                owner.Stats.CurrentDashCount++;
                break;
            case 4:
                owner.dashRechargeTime *= 0.8f;
                break;
            case 5:

                baseProjectileSize += 0.2f;
                owner.Stats.CurrentDashCount++;
                break;
        }
    }

}
