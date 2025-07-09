using UnityEngine;
using Cysharp.Threading.Tasks;

public class WarriorAttackProjectile : PlayerProjectile
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float animationDuration;

    //공격프레임과 이펙트 프레임 싱크 맞추기 용도 -> 직접 입력하시오
    private const float ATTACK_ANIMATION_FRAMES = 5f;
    private const float EFFECT_ANIMATION_FRAMES = 11f;

    private const float BASE_SCALE = 1f;

    // 이펙트 크기와 콜라이더 크기 보정용 -> 직접 입력하시오
    private const float BASE_SYNC = 0.6f;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isMoving = true;
        transform.position = startPosition;
        
        transform.rotation = Quaternion.identity;
        
        if (animator != null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                animationDuration = clips[0].length;
                float aspdMultiplier = GetPlayerAspd();
                float baseSpeedMultiplier = EFFECT_ANIMATION_FRAMES / ATTACK_ANIMATION_FRAMES;
                animator.speed = baseSpeedMultiplier * aspdMultiplier;
            }
        }

        Vector2 direction = (targetPosition - startPosition).normalized;
        spriteRenderer.flipX = direction.x > 0;

        //float rangeMultiplier = GetPlayerRange() / 0.4f;
        transform.localScale = Vector3.one * GetPlayerRange();
        
        //if (TryGetComponent<Collider2D>(out var collider))
        //{
        //    if (collider is CircleCollider2D circleCollider)
        //    {
        //        circleCollider.radius *= rangeMultiplier * BASE_SYNC / 3;
        //    }
            
        //}

        damage = GetPlayerDamage();
        speed = 0f;
        Destroy(gameObject, animationDuration / animator.speed);
    }

    private float GetPlayerAspd()
    {
        Player player = UnitManager.Instance.GetPlayer();
        return player != null ? player.Stats.CurrentAspd : 0f;
    }

    private float GetPlayerRange()
    {
        Player player = UnitManager.Instance.GetPlayer();
        return player != null ? player.Stats.CurrentATKRange : 0.5f;
    }

    private float GetPlayerDamage()
    {
        Player player = UnitManager.Instance.GetPlayer();
        return player != null ? player.Stats.CurrentATK : 0f;
    }

    protected override async UniTaskVoid MoveProjectileAsync(System.Threading.CancellationToken token)
    {
        transform.position = startPosition;
        
        while (isMoving && !token.IsCancellationRequested)
        {
            if (gameObject == null || !isMoving)
            {
                break;
            }

            await UniTask.Yield(token);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            if (collision.TryGetComponent(out MonsterBase monster))
            {
                float finalDamage = UnityEngine.Random.value < stats.critical ? damage * stats.cATK : damage;
                monster.TakeDamage(finalDamage);
                DataManager.Instance.AddDamageData(finalDamage, Enums.SkillName.None);
            }
        }
    }

    public override void InitProjectile(Vector3 startPos, Vector3 targetPos, ProjectileStats projectileStats)
    {
        base.InitProjectile(startPos, targetPos, projectileStats);
        stats = projectileStats;
        startPosition = startPos;
        targetPosition = targetPos;
    }
} 