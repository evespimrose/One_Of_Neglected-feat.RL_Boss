using UnityEngine;

public class Aug_ArcRanger : ConditionalAugment
{
    private float damageMultiplier = 1f;
    private float projectileSpeed = 3f;
    private float baseProjectileSize = 1f;
    private int penetration = 0;
    private float duration = 5f;
    private float maxDistance = 10f;
    int baseProjectiles = 6;
    private float CurrentDamage => owner.Stats.CurrentATK * damageMultiplier;
    private float CurrentProjectileSize => baseProjectileSize * owner.Stats.CurrentATKRange;

    private PlayerProjectile currentPathProjectile;
    private bool isExplosive = false;

    public Aug_ArcRanger(Player owner) : base(owner)
    {
        aguName = Enums.AugmentName.ArcRanger;
    }

    public override void Activate()
    {
        base.Activate();

        owner.dashDetect += OnDashDetect;
        owner.dashCompleted += OnDashCompleted;
        owner.DamageReduction += 10f;
    }

    private void OnDashDetect()
    {
        if (CheckCondition())
        {
            OnConditionDetect();
            SpawnRushProjectile();
        }
    }

    public override bool CheckCondition()
    {
        return true;
    }

    public override void OnConditionDetect()
    {
    }

    private void SpawnRushProjectile()
    {
        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
        }

        Vector2 dashStart = owner.transform.position;
        
        // 플레이어에서 대쉬 방향 가져오니까 이상함 ㅜㅜ
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool hasKeyboardInput = horizontalInput != 0 || verticalInput != 0;

        Vector2 direction;
        if (hasKeyboardInput)
        {
            direction = new Vector2(horizontalInput, verticalInput).normalized;
        }
        else
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mousePosition - dashStart).normalized;
        }

        float angleStep = 5f;
        
        int totalProjectiles = baseProjectiles + owner.Stats.CurrentProjAmount -1;
        float totalAngleSpread = (totalProjectiles - 1) * angleStep;
        float startAngle = -totalAngleSpread / 2f;
        SoundManager.Instance.Play("CrossBow", SoundManager.Sound.Effect);
        for (int i = 0; i < totalProjectiles; i++)
        {
            float currentAngle = startAngle + (i * angleStep);
            Vector2 rotatedDirection = RotateVector(direction, currentAngle);
            Vector2 targetPosition = dashStart + rotatedDirection * maxDistance;

            PlayerProjectile proj = ProjectileManager.Instance.SpawnPlayerProjectile(
                "ArcRangerProjectile",
                dashStart,
                targetPosition,
                projectileSpeed,
                CurrentDamage,
                CurrentProjectileSize,
                maxDistance,
                penetration,
                duration
            );

            if (isExplosive && proj is ArcRangerProjectile arcProj)
            {
                arcProj.SetExplosive(true);
            }
        }

        currentPathProjectile = ProjectileManager.Instance.SpawnPlayerProjectile(
            "ArcRangerProjectile",
            dashStart,
            dashStart,
            0f,
            CurrentDamage,
            CurrentProjectileSize,
            maxDistance,
            penetration,
            duration
        );

        if (currentPathProjectile != null)
        {
            currentPathProjectile.transform.SetParent(owner.transform);
            currentPathProjectile.transform.rotation = Quaternion.identity;
            currentPathProjectile.transform.localPosition = Vector3.zero;
        }
    }

    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        
        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;
        
        return new Vector2(x, y);
    }

    private void OnDashCompleted()
    {
        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
            currentPathProjectile = null;
        }
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
                baseProjectiles += 2;
                owner.Stats.CurrentDashCount++;
                break;
            case 4:
                owner.dashRechargeTime *= 0.8f;
                break;
            case 5:
                isExplosive = true;
                break;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        owner.dashDetect -= OnDashDetect;
        owner.dashCompleted -= OnDashCompleted;
        owner.DamageReduction = 0f;

        if (currentPathProjectile != null)
        {
            ProjectileManager.Instance.RemoveProjectile(currentPathProjectile);
            currentPathProjectile = null;
        }
    }
}
