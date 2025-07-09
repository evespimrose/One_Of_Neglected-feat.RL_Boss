using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.AI;
using System.Linq;

public abstract class MonsterBase : MonoBehaviour
{
    protected StateHandler<MonsterBase> stateHandler;

    [Header("기본 컴포넌트")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator animator;
    [SerializeField] private float dieAnimationLength = 1f;
    [Header("자동으로 Awake때 가져옴")]
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [Header("스탯 증가량 설정")]
    [SerializeField] protected float damageIncreasePerInterval;  // 30초마다 증가할 공격력
    [SerializeField] protected float healthIncreasePerInterval;  // 30초마다 증가할 체력
    [SerializeField] protected float defenseIncreasePerInterval; // 증가할 방어력 (유니크 몬스터용)
    [SerializeField] protected float statUpdateInterval = 30f;   // 스탯 업데이트 간격

    private bool isDying = false;
    protected Transform playerTransform;

    protected MonsterStats stats;

    public event Action<MonsterBase> OnDeath;

    public MonsterStats Stats => stats;
    public Animator Animator => animator;

    protected virtual void Awake()
    {
        if (m_SpriteRenderer == null) m_SpriteRenderer = GetComponent<SpriteRenderer>();
        InitializeComponents();
        InitializeStats();
        InitializeStateHandler();
    }

    // TimeManager의 30초 이벤트 구독
    protected virtual void OnEnable()
    {
        var timeManager = TimeManager.Instance;
        if (timeManager != null)
        {
            if (this is RangedNormalMonster)
            {
                timeManager.OnOneMinThirtySecondsPassed += UpdateMonsterStats;
            }
            else if (this is DamageUniqueMonster || this is CrowdControlUniqueMonster || this is TankUniqueMonster)
            {
                timeManager.OnOneMinFiftySecondsPassed += UpdateMonsterStats;
            }
            else
            {
                timeManager.OnThirtySecondsPassed += UpdateMonsterStats;
            }
        }
    }

    // 이벤트 구독 해제
    protected virtual void OnDisable()
    {
        var timeManager = TimeManager.Instance;
        if (timeManager != null)
        {
            if (this is RangedNormalMonster)
            {
                timeManager.OnOneMinThirtySecondsPassed -= UpdateMonsterStats;
            }
            else if (this is DamageUniqueMonster || this is CrowdControlUniqueMonster || this is TankUniqueMonster)
            {
                timeManager.OnOneMinFiftySecondsPassed -= UpdateMonsterStats;
            }
            else
            {
                timeManager.OnThirtySecondsPassed -= UpdateMonsterStats;
            }
        }
    }

    protected virtual void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected abstract void InitializeStats();

    protected abstract void InitializeStateHandler();

    protected virtual void Update()
    {
        stateHandler?.Update();
        CheckStateTransitions();
    }

    // 30초마다 호출되어 몬스터 스탯을 업데이트하는 메서드
    protected virtual void UpdateMonsterStats()
    {
        float currentTime = TimeManager.Instance.GameTime;

        // Early 몬스터 (0~3분)
        if (this is EarlyNormalMonster && currentTime <= 180f)
        {
            int intervals = Mathf.FloorToInt(currentTime / statUpdateInterval);
            UpdateStats(intervals, damageIncreasePerInterval, healthIncreasePerInterval);
        }
        // Mid 몬스터 (3~7분)
        else if (this is MidNormalMonster && currentTime > 180f && currentTime <= 420f)
        {
            int intervals = Mathf.FloorToInt((currentTime - 180f) / statUpdateInterval);
            UpdateStats(intervals, damageIncreasePerInterval, healthIncreasePerInterval);
        }
        // Late 몬스터 (7~10분)
        else if (this is LateNormalMonster && currentTime > 420f && currentTime <= 600f)
        {

            int intervals = Mathf.FloorToInt((currentTime - 420f) / statUpdateInterval);
            UpdateStats(intervals, damageIncreasePerInterval, healthIncreasePerInterval);
        }
        else if (this is RangedNormalMonster)
        {
            // 1분 30초(90초) 간격으로 intervals 계산
            int intervals = Mathf.FloorToInt(currentTime / 90f);
            UpdateStats(intervals, damageIncreasePerInterval, healthIncreasePerInterval);
        }
        // Unique 몬스터들 (1분 50초 = 110초)
        else if (this is DamageUniqueMonster || this is CrowdControlUniqueMonster || this is TankUniqueMonster)
        {
            int intervals = Mathf.FloorToInt(currentTime / 110f);
            UpdateStats(intervals, damageIncreasePerInterval, healthIncreasePerInterval, defenseIncreasePerInterval);
        }
    }

    // 실제 스탯 업데이트를 처리하는 헬퍼 메서드
    private void UpdateStats(int intervals, float dmgIncrease, float hpIncrease, float defIncrease = 0f)
    {
        // Late 몬스터의 경우 공격력 20 제한
        if (this is LateNormalMonster || this is RangedNormalMonster)
        {
            stats.attackDamage = Mathf.Min(stats.attackDamage + (intervals * dmgIncrease), 20f);
        }
        else
        {
            stats.attackDamage += (intervals * dmgIncrease);
        }
        // 유니크 몬스터인 경우에만 방어력 증가
        if (this is DamageUniqueMonster || this is CrowdControlUniqueMonster || this is TankUniqueMonster)
        {
            stats.defense += (intervals * defIncrease);
        }

        float newMaxHealth = stats.maxHealth + (intervals * hpIncrease);
        float healthDiff = newMaxHealth - stats.maxHealth;
        stats.maxHealth = newMaxHealth;
        stats.currentHealth += healthDiff;

        // Debug.Log($"[{gameObject.name}] Stats Updated\n" +
        //  $"Current Attack: {stats.attackDamage}\n" +
        //  $"Current Health: {stats.maxHealth}");
    }

    public virtual void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        // 이동 방향 계산
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Transform을 사용한 이동
        transform.position += (Vector3)(direction * stats.moveSpeed * Time.deltaTime);

        // 스프라이트 방향 설정
        if (direction.x != 0)
        {
            // transform.localScale = new Vector3(
            //     Mathf.Sign(direction.x),
            //     transform.localScale.y,
            //     transform.localScale.z
            // );

            //Sign의 값이 -1 , 1 로 고정이 돼 있기 때문에 다른 방식을 채용
            //Sign을 사용하려면 생성됐을 기존 스케일을 저장하고 
            //Sign값과 기존 스케일의 차를 +- 해주어야 하는 등의 방식을 사용해야할것으로 보임.
        }
        //스프라이트 방향 설정
        //direction.x 값에 따라 스프라이트의 flipX 값을 바꿈;
        if (direction.x > 0)
        {
            m_SpriteRenderer.flipX = false;
        }
        if (direction.x < 0)
        {
            m_SpriteRenderer.flipX = true;
        }
    }

    protected virtual void CheckStateTransitions()
    {
        if (stateHandler == null) return;

        bool playerInRange = IsPlayerInAttackRange();

        // 현재 상태가 공격 상태일 때
        if (stateHandler.CurrentState is MonsterAttackState || stateHandler.CurrentState is RangedAttackState)
        {
            // 플레이어가 범위를 벗어나면 즉시 이동 상태로 전환
            if (!playerInRange)
            {
                stateHandler.ChangeState(typeof(MonsterMoveState));
            }
        }
        // 현재 상태가 이동 상태일 때
        else if (stateHandler.CurrentState is MonsterMoveState)
        {
            // 플레이어가 범위 안에 있으면 공격 상태로 전환
            if (playerInRange)
            {
                Type attackStateType = this is RangedMonster
                    ? typeof(RangedAttackState)
                    : typeof(MonsterAttackState);
                stateHandler.ChangeState(attackStateType);
            }
        }
    }

    public virtual bool IsPlayerInAttackRange()
    {
        if (playerTransform == null) return false;
        return Vector2.Distance(transform.position, playerTransform.position) <= stats.attackRange;
    }
    public virtual void TakeDamage(float damage)
    {
        SoundManager.Instance.Play("MonsterAttacked_Test", SoundManager.Sound.Effect, 1f, false, 1f);
        stats.currentHealth -= damage;
        ShowDamageFont(transform.position, damage, transform);
        animator?.SetTrigger("Hit");
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void ShowDamageFont(Vector2 pos, float damage, Transform parent)
    {
        GameObject go = Resources.Load<GameObject>("DamageText");
        if (go != null)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Vector2.up * 0.2f;

            GameObject instance = Instantiate(go, spawnPosition, Quaternion.identity);
            ShowDamage damageText = instance.GetComponent<ShowDamage>();
            if (damageText != null)
            {
                damageText.SetInfo(spawnPosition, damage, parent);
            }
        }
    }

    protected virtual async void Die()
    {
        if (isDying) return;
        isDying = true;
        DataManager.Instance.AddKillCount();

        UnitManager.Instance.RemoveMonster(this);
        stateHandler.ChangeState(typeof(MonsterDieState));
        try
        {
            await UniTask.Delay((int)(dieAnimationLength * 1000));
            OnMonsterDestroy();

        }
        finally
        {
            // 항상 오브젝트 제거 실행
            if (this != null && gameObject != null)
            {
                OnDeath?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnMonsterDestroy()
    {
        // 유니크 몬스터인 경우
        if (this is DamageUniqueMonster || this is CrowdControlUniqueMonster)
        {
            UnitManager.Instance.SpawnWorldObject(Enums.WorldObjectType.ExpBlack, transform.position);
            // Debug.Log($"{GetType().Name} 처치 - ExpBlack 드롭!");
            return;
        }
        // 탱크 유니크 몬스터인 경우
        else if (this is TankUniqueMonster)
        {
            // [수정] activeMonsters.Count 대신 킬 카운트 사용
            if (UnitManager.Instance.IsLastTankMonster())
            {
                UnitManager.Instance.SpawnWorldObject(Enums.WorldObjectType.ExpBlack, transform.position);
                // Debug.Log("마지막 탱크 몬스터 처치 - ExpBlack 드롭!");
                // [추가] 킬 카운트 리셋
                UnitManager.Instance.ResetTankMonsterKillCount();
                return;
            }
            // [추가] 탱크 몬스터 킬 카운트 증가
            UnitManager.Instance.IncreaseTankMonsterKillCount();
        }

        // 1% 확률로 골드 드롭
        if (UnityEngine.Random.Range(0f, 100f) <= 1f)
        {
            UnitManager.Instance.SpawnWorldObject(Enums.WorldObjectType.Gold_1, transform.position);
            // Debug.Log("골드 드롭!");
        }
        else
        {
            UnitManager.Instance.SpawnWorldObject(Enums.WorldObjectType.ExpBlue, transform.position);
        }
    }

    public virtual void ApplyKnockback(Vector2 hitPoint, float knockbackForce)
    {
        if (rb != null)
        {
            Vector2 knockbackDirection = (transform.position - (Vector3)hitPoint).normalized;

            rb.AddForce(knockbackDirection * Mathf.Clamp(knockbackForce, 1f, 5f), ForceMode2D.Impulse);
        }
    }

}