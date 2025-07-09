using System;
using UnityEngine;
using static Enums;
using System.Linq;

[Serializable]
public class StatViewer
{
    [Tooltip("레벨")] public int Level;
    [Tooltip("최대 경험치")] public int MaxExp;
    [Tooltip("경험치")] public float Exp;
    [Tooltip("최대체력")] public int MaxHp;
    [Tooltip("체력")] public float Hp;
    [Tooltip("체력회복량")] public float HpRegen;
    [Tooltip("방어력")] public int Defense;
    [Tooltip("이동속도")] public float Mspd;
    [Tooltip("공격력")] public float ATK;
    [Tooltip("공격속도")] public float Aspd;
    [Tooltip("치명타 확률")] public float CriRate;
    [Tooltip("치명타 데미지")] public float CriDamage;
    [Tooltip("투사체 개수")] public int ProjAmount;
    [Tooltip("공격범위")] public float ATKRange;
    [Tooltip("지속시간")] public float Duration;
    [Tooltip("쿨타임")] public float Cooldown;
    [Tooltip("부활 횟수")] public int Revival;
    [Tooltip("재화 습득범위")] public float Magnet;
    [Tooltip("성장")] public float Growth;
    [Tooltip("탐욕")] public float Greed;
    [Tooltip("저주")] public float Curse;
    [Tooltip("새로고침 횟수")] public int Reroll;
    [Tooltip("스탯 지우기")] public int Banish;
    [Tooltip("신 처치")] public bool GodKill;
    [Tooltip("방어막")] public bool Barrier;
    [Tooltip("방어막 쿨타임")] public float BarrierCooldown;
    [Tooltip("피격시 무적")] public bool Invincibility;
    [Tooltip("대시 횟수")] public int DashCount;
    [Tooltip("대적자")] public bool Adversary;
    [Tooltip("투사체 파괴")] public bool ProjDestroy;
    [Tooltip("투사체 반사")] public bool ProjParry;
}

public abstract class Player : MonoBehaviour
{
    #region Field
    protected bool isDead = false;
    
    public ClassType ClassType { get; protected set; }
    public Animator Animator => animator;
    public PlayerStats Stats
    {
        get { return stats; }
        protected set { stats = value; }
    }
    public ParticleSystem DashEffect => dashEffect;
    [SerializeField]private GameObject ReviveEffect;

    protected AugmentSelector augmentSelector;

    public AugmentSelector augment => augmentSelector;

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] public StatViewer statViewer;
    [SerializeField] public SpriteRenderer modelRenderer;
    [SerializeField] private GameObject barrierEffect;
    [SerializeField] private CircleCollider2D magnetCollider;

    // 자동사냥 모드!
    public bool isAuto = false;

    public StateHandler<Player> stateHandler { get; protected set; }
    protected bool isSkillInProgress = false;
    protected bool isDashing = false;
    protected bool isBarrier = false;

    protected PlayerStats stats;

    public Vector2 targetPosition;
    private Vector2 savedTargetPosition;

    protected float moveThreshold = 0.1f;

    #endregion

    #region DashSettings
    public float dashRechargeTime { get; set; } = 5f;
    protected float dashRechargeTimer = 0f;
    protected int currentDashCount;

    public float DashRechargeTimer => dashRechargeTimer;
    public int CurrentDashCount => currentDashCount;
    public int MaxDashCount => stats.CurrentDashCount;
    #endregion

    #region BarrierSettings
    protected float barrierRechargeTimer = 0f;
    protected bool hasBarrierCharge = false;

    public float BarrierRechargeTimer => barrierRechargeTimer;
    public float BarrierRechargeTime => stats.CurrentBarrierCooldown;
    public bool HasBarrierCharge => hasBarrierCharge;
    #endregion

    #region InvincibilitySettings
    private bool isInvincible = false;
    private float invincibilityDuration = 0.1f;
    private float invincibilityTimer = 0f;
    #endregion

    #region MoveBuff
    private float speedBuffDuration = 0f;
    private float speedBuffTimer = 0f;
    private float speedBuffAmount = 0f;
    private bool hasSpeedBuff = false;
    #endregion

    #region DamgeReducton
    public float DamageReduction { get; set; } = 0f;
    #endregion

    #region DashDetected
    protected event Action DashDetect;
    public event Action dashDetect
    {
        add => DashDetect += value;
        remove => DashDetect -= value;
    }
    public void InvokeDashDetect()
    {
        DashDetect?.Invoke();
    }
    #endregion

    #region AttackDetected
    protected event Action<Vector3> AttackDetect;
    public event Action<Vector3> attackDetect
    {
        add => AttackDetect += value;
        remove => AttackDetect -= value;
    }
    public void InvokeAttackDetect(Vector3 targetPosition)
    {
        AttackDetect?.Invoke(targetPosition);
    }
    #endregion

    #region DashCompleted
    protected event Action DashCompleted;
    public event Action dashCompleted
    {
        add => DashCompleted += value;
        remove => DashCompleted -= value;
    }
    public void InvokeDashCompleted()
    {
        DashCompleted?.Invoke();
    }
    #endregion

    protected virtual void Awake()
    {
        InitializeStateHandler();
        InitializeStats();
        InitializeClassType();
        InitializeStatViewer();  
        SyncStatsFromViewer();
        UpdateStatViewer();

        SubscribeToStatEvents();

        currentDashCount = stats.CurrentDashCount;

        // 베리어 초기화
        isBarrier = stats.CurrentBarrier;
        hasBarrierCharge = isBarrier;
        if (barrierEffect != null)
        {
            barrierEffect.SetActive(hasBarrierCharge);
        }

        if (dashEffect != null)
        {
            dashEffect.Stop();
        }
        if (magnetCollider != null)
        {
            magnetCollider.radius = stats.CurrentMagnet;
            magnetCollider.isTrigger = true;
        }

        augmentSelector = gameObject.AddComponent<AugmentSelector>();
        augmentSelector.Initialize(this);
    }

    private void Update()
    {
        stateHandler.Update();
        UpdateDashRecharge();
        UpdateBarrierRecharge();
        UpdateInvincibility();
        UpdateHpRegen();
        UpdateSpeedBuff();
    }

    protected abstract void InitializeStats();
    protected abstract void InitializeStateHandler();
    protected abstract void InitializeClassType();
    protected abstract void InitializeStatViewer();

    private void SubscribeToStatEvents()
    {
        // 모든 스탯 변경 이벤트에 UpdateStatViewer 추가
        stats.OnLevelUp += (value) =>
        {
            statViewer.Level = value;
            UpdateStatViewer();
        };
        stats.OnMaxExpChanged += (value) =>
        {
            statViewer.MaxExp = value;
            UpdateStatViewer();
        };
        stats.OnExpChanged += (value) =>
        {
            statViewer.Exp = value;
            UpdateStatViewer();
        };
        stats.OnMaxHpChanged += (value) =>
        {
            statViewer.MaxHp = value;
            UpdateStatViewer();
        };
        stats.OnHpChanged += (value) =>
        {
            statViewer.Hp = value;
            UpdateStatViewer();
        };
        stats.OnHpRegenChanged += (value) =>
        {
            statViewer.HpRegen = value;
            UpdateStatViewer();
        };
        stats.OnDefenseChanged += (value) =>
        {
            statViewer.Defense = value;
            UpdateStatViewer();
        };
        stats.OnMspdChanged += (value) =>
        {
            statViewer.Mspd = value;
            UpdateStatViewer();
        };
        stats.OnATKChanged += (value) =>
        {
            statViewer.ATK = value;
            UpdateStatViewer();
        };
        stats.OnAspdChanged += (value) =>
        {
            statViewer.Aspd = value;
            UpdateStatViewer();
        };
        stats.OnCriRateChanged += (value) =>
        {
            statViewer.CriRate = value;
            UpdateStatViewer();
        };
        stats.OnCriDamageChanged += (value) =>
        {
            statViewer.CriDamage = value;
            UpdateStatViewer();
        };
        stats.OnProjAmountChanged += (value) =>
        {
            statViewer.ProjAmount = value;
            UpdateStatViewer();
        };
        stats.OnATKRangeChanged += (value) =>
        {
            statViewer.ATKRange = value;
            UpdateStatViewer();
        };
        stats.OnDurationChanged += (value) =>
        {
            statViewer.Duration = value;
            UpdateStatViewer();
        };
        stats.OnCooldownChanged += (value) =>
        {
            statViewer.Cooldown = value;
            UpdateStatViewer();
        };
        stats.OnRevivalChanged += (value) =>
        {
            statViewer.Revival = value;
            UpdateStatViewer();
        };
        stats.OnMagnetChanged += (value) =>
        {
            statViewer.Magnet = value;
            if (magnetCollider != null)
            {
                magnetCollider.radius = value;
            }
            UpdateStatViewer();
        };
        stats.OnGrowthChanged += (value) =>
        {
            statViewer.Growth = value;
            UpdateStatViewer();
        };
        stats.OnGreedChanged += (value) =>
        {
            statViewer.Greed = value;
            UpdateStatViewer();
        };
        stats.OnCurseChanged += (value) =>
        {
            statViewer.Curse = value;
            UpdateStatViewer();
        };
        stats.OnRerollChanged += (value) =>
        {
            statViewer.Reroll = value;
            UpdateStatViewer();
        };
        stats.OnBanishChanged += (value) =>
        {
            statViewer.Banish = value;
            UpdateStatViewer();
        };
        stats.OnGodKillChanged += (value) =>
        {
            statViewer.GodKill = value;
            UpdateStatViewer();
        };
        stats.OnBarrierChanged += (value) =>
        {
            statViewer.Barrier = value;
            UpdateStatViewer();
        };
        stats.OnBarrierCooldownChanged += (value) =>
        {
            statViewer.BarrierCooldown = value;
            UpdateStatViewer();
        };
        stats.OnInvincibilityChanged += (value) =>
        {
            statViewer.Invincibility = value;
            UpdateStatViewer();
        };
        stats.OnDashCountChanged += (value) =>
        {
            statViewer.DashCount = value;
            UpdateStatViewer();
        };
        stats.OnAdversaryChanged += (value) =>
        {
            statViewer.Adversary = value;
            UpdateStatViewer();
        };
        stats.OnProjDestroyChanged += (value) =>
        {
            statViewer.ProjDestroy = value;
            UpdateStatViewer();
        };
        stats.OnProjParryChanged += (value) =>
        {
            statViewer.ProjParry = value;
            UpdateStatViewer();
        };
    }

    #region Dash
    private void UpdateDashRecharge()
    {
        if (currentDashCount < stats.CurrentDashCount)
        {
            dashRechargeTimer += Time.deltaTime;
            if (dashRechargeTimer >= dashRechargeTime * stats.CurrentCooldown)
            {
                dashRechargeTimer = 0f;
                currentDashCount++;
            }
        }
    }
    public bool CanDash()
    {
        return currentDashCount > 0;
    }
    public void ConsumeDash()
    {
        if (currentDashCount > 0)
        {
            currentDashCount--;
        }
    }
    public void SetDashing(bool dashing)
    {
        isDashing = dashing;
    }
    #endregion

    #region move
    public void MoveTo(Vector2 destination)
    {
        if (isDashing) return;

        if (Vector2.Distance(transform.position, destination) > moveThreshold)
        {
            Vector2 direction = (destination - (Vector2)transform.position).normalized;
            Vector2 newPosition = Vector2.MoveTowards(transform.position
                , destination, stats.CurrentMspd * Time.deltaTime);

            transform.SetPositionAndRotation(new Vector3(newPosition.x, newPosition.y, transform.position.z), transform.rotation);

            if (direction.x != 0)
            {
                FlipModel(direction.x < 0);
            }
        }
    }
    public bool IsAtDestination()
    {
        return Vector2.Distance(transform.position, targetPosition) < moveThreshold;
    }
    public void SetCurrentPositionAsTarget()
    {
        targetPosition = transform.position;
        savedTargetPosition = targetPosition;
    }
    #endregion

    #region Utils
    public void SetSkillInProgress(bool inProgress, bool savePrevPosition = true)
    {
        isSkillInProgress = inProgress;
        if (inProgress && savePrevPosition)
        {
            savedTargetPosition = targetPosition;
        }
    }
    public void LookAtTarget(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        FlipModel(direction.x < 0);
    }
    public void FlipModel(bool isLeft)
    {
        if (modelRenderer != null)
        {
            modelRenderer.flipX = isLeft;
        }

        if (dashEffect != null)
        {
            Vector3 localScale = dashEffect.transform.localScale;
            localScale.x = isLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            dashEffect.transform.localScale = localScale;
        }
    }
    public virtual void TakeDamage(float damage)
    {
        // 무적이거나 죽은 상태면 데미지를 받지 않음
        if (isInvincible || isDead) return;  

        // 베리어 체크
        if (isBarrier && hasBarrierCharge)
        {
            hasBarrierCharge = false;
            if (barrierEffect != null)
            {
                barrierEffect.SetActive(false);
            }
            return;
        }

        // (데미지 - 방어력) * 뎀감
        float finalDamage = Mathf.Max(0, damage - stats.CurrentDefense) * (100 - DamageReduction) / 100;
        stats.currentHp -= finalDamage;
        ShowDamageFont(transform.position, finalDamage, transform);

        if (stats.CurrentInvincibility)
        {
            SetInvincible(invincibilityDuration);
        }

        if (stats.currentHp <= 0)
        {
            stats.currentHp = 0;
            isDead = true;  // 죽은 상태로 설정
            
            if (stats.CurrentRevival > 0)
            {
                SoundManager.Instance.Play("Revive", SoundManager.Sound.Effect, 1f, false, 0.6f);
              
                GameObject startEffect = GameObject.Instantiate(ReviveEffect, transform.position, Quaternion.identity);
                GameObject.Destroy(startEffect, 1f);

                stats.CurrentRevival -= 1; 
                ChangePlayerRevive();
                return; 
            }
                        
            ChangePlayerDie();
            UI_Manager.Instance.panel_Dic["Result_Panel"].PanelOpen();
        }
    }

    //죽을때 쓰시오
    public void ChangePlayerDie()
    {
        //죽을떼 timescle 0 please

        if (ClassType == ClassType.Warrior)
        {
            stateHandler.ChangeState(typeof(WarriorDieState));
        }
        else if (ClassType == ClassType.Archer)
        {
            stateHandler.ChangeState(typeof(ArcherDieState));
        }
        else if (ClassType == ClassType.Magician)
        {
            stateHandler.ChangeState(typeof(MagicianDieState));
        }
    }

    //되살릴때 쓰시오
    public void ChangePlayerRevive()
    {
        isDead = false;  
        stats.SetStatValue(StatType.Hp, stats.CurrentMaxHp);

        if (ClassType == ClassType.Warrior)
        {
            stateHandler.ChangeState(typeof(WarriorIdleState));
        }
        else if (ClassType == ClassType.Archer)
        {
            stateHandler.ChangeState(typeof(ArcherIdleState));
        }
        else if (ClassType == ClassType.Magician)
        {
            stateHandler.ChangeState(typeof(MagicianIdleState));
        }
    }

    public void ShowDamageFont(Vector2 pos, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Resources.Load<GameObject>("DamageText");
        if (go != null)
        {
            Vector2 spawnPosition = (Vector2)transform.position + Vector2.up * 0.2f;

            GameObject instance = Instantiate(go, spawnPosition, Quaternion.identity);
            ShowDamage damageText = instance.GetComponent<ShowDamage>();
            if (damageText != null)
            {
                damageText.SetInfo(spawnPosition, damage, parent, isCritical);
            }
        }
    }
    private void UpdateBarrierRecharge()
    {
        if (!isBarrier) return;

        if (!hasBarrierCharge)
        {
            barrierRechargeTimer += Time.deltaTime;
            if (barrierRechargeTimer >= stats.CurrentBarrierCooldown * stats.CurrentCooldown)
            {
                barrierRechargeTimer = 0f;
                hasBarrierCharge = true;
                if (barrierEffect != null)
                {
                    barrierEffect.SetActive(true);
                }
            }
        }
    }
    private void UpdateInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer >= invincibilityDuration)
            {
                isInvincible = false;
                invincibilityTimer = 0f;
            }
        }
    }
    private float regenTimer = 0f;
    private void UpdateHpRegen()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer >= 1)
        {
            regenTimer = 0f;
            stats.currentHp += stats.CurrentHpRegen;

            // 여기에 HP회복 이펙트 등 추가 가능
        }
    }

    private static class CollectibleValues
    {
        public const float BASIC_EXP = 10f;
        public const int BASIC_GOLD = 10;
        public const int LARGE_GOLD = 50;
        public const float HEAL_AMOUNT = 20f;
        public const float BOMB_DAMAGE = 100f;
    }

    private void CollectObj(WorldObject worldObject)
    {
        SoundManager.Instance.Play("SFX_UI_Click_Organic_Pop_Negative_1", SoundManager.Sound.Effect, 1f, false, 0.7f);
        switch (worldObject.objectType)
        {
            case WorldObjectType.ExpBlue:
                int expAmount = worldObject.GetExpAmount();
                //ProcessExperience(CollectibleValues.BASIC_EXP * stats.CurrentGrowth);
                ProcessExperience(expAmount * stats.CurrentGrowth);
                break;
            case WorldObjectType.ExpBlack:
                ProcessExperience(stats.CurrentMaxExp * stats.CurrentGrowth);
                DataManager.Instance.inGameValue.remnents += 1;
                break;

            case WorldObjectType.Gold_1:
                DataManager.Instance.inGameValue.gold += (int)(CollectibleValues.BASIC_GOLD * stats.CurrentGreed);
                break;

            case WorldObjectType.Gold_2:
                DataManager.Instance.inGameValue.gold += (int)(CollectibleValues.LARGE_GOLD * stats.CurrentGreed);
                break;

            case WorldObjectType.Chicken:
                stats.currentHp += CollectibleValues.HEAL_AMOUNT;
                break;

            case WorldObjectType.Time_Stop:
                //UnitManager.Instance.ActivateMagnet();
                break;

            case WorldObjectType.Boom:
                ProcessBoomEffect();
                break;
        }

        worldObject.selfDestroy();
    }

    private void ProcessExperience(float expAmount)
    {
        stats.currentExp += expAmount;

        while (stats.currentExp >= stats.CurrentMaxExp && Time.timeScale > 0)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        stats.currentExp -= stats.CurrentMaxExp;
        stats.CurrentLevel += 1;
        stats.CurrentMaxExp = CalculateNextLevelExp();

        if (Time.timeScale > 0)
        {
            UI_Manager.Instance.panel_Dic["LevelUp_Panel"].PanelOpen();
        }
    }

    private void ProcessBoomEffect()
    {
        var activeMonsters = UnitManager.Instance.GetMonstersInRange(0f, float.MaxValue);
        if (activeMonsters == null) return;

        foreach (var monster in activeMonsters.Where(m => m != null))
        {
            monster.TakeDamage(CollectibleValues.BOMB_DAMAGE);
        }
    }

    private int CalculateNextLevelExp()
    {
        float baseExp = 100;
        float totalMultiplier = 1f;

        if (stats.CurrentLevel <= 10)
        {
            totalMultiplier = Mathf.Pow(1.1f, stats.CurrentLevel - 1);
        }
        else if (stats.CurrentLevel <= 15)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9);
        }
        else if (stats.CurrentLevel <= 30)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, stats.CurrentLevel - 15);
        }
        else if (stats.CurrentLevel <= 35)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15);
        }
        else if (stats.CurrentLevel <= 60)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, stats.CurrentLevel - 35);
        }
        else if (stats.CurrentLevel <= 65)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25);
        }
        else if (stats.CurrentLevel <= 80)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25) * Mathf.Pow(1.025f, stats.CurrentLevel - 65);
        }
        else if (stats.CurrentLevel <= 85)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25) * Mathf.Pow(1.025f, 15);
        }
        else if (stats.CurrentLevel <= 100)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25) * Mathf.Pow(1.025f, 15) *
                             Mathf.Pow(1.015f, stats.CurrentLevel - 85);
        }
        else if (stats.CurrentLevel <= 105)
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25) * Mathf.Pow(1.025f, 15) *
                             Mathf.Pow(1.015f, 15);
        }
        else
        {
            totalMultiplier = Mathf.Pow(1.1f, 9) * Mathf.Pow(1.075f, 15) *
                             Mathf.Pow(1.05f, 25) * Mathf.Pow(1.025f, 15) *
                             Mathf.Pow(1.015f, 15) * Mathf.Pow(1.01f, stats.CurrentLevel - 105);
        }

        return Mathf.RoundToInt(baseExp * totalMultiplier);
    }
    public void SetInvincible(float duration)
    {
        isInvincible = true;
        invincibilityDuration = duration;
        invincibilityTimer = 0f;
    }
    public void ApplySpeedBuff(float duration, float percent)
    {
        hasSpeedBuff = true;
        speedBuffDuration = duration;
        speedBuffTimer = 0f;

        float baseSpeed = stats.CurrentMspd;
        speedBuffAmount = baseSpeed * percent;

        stats.ModifyStatValue(StatType.Mspd, speedBuffAmount);
    }
    private void UpdateSpeedBuff()
    {
        if (hasSpeedBuff)
        {
            speedBuffTimer += Time.deltaTime;
            if (speedBuffTimer >= speedBuffDuration)
            {
                stats.ModifyStatValue(StatType.Mspd, -speedBuffAmount);
                hasSpeedBuff = false;
                speedBuffTimer = 0f;
            }
        }
    }

    public virtual void OnProjectileHit(MonsterProjectile projectile)
    {
        // 패링 시도
        var swordShield = augmentSelector.activeAugments
            .FirstOrDefault(aug => aug is Aug_SwordShield) as Aug_SwordShield;

        if (swordShield != null && swordShield.TryParryProjectile(projectile))
        {
            // 패링 성공
            return;
        }

        //// 패링 실패시 일반 데미지 처리
        //TakeDamage(projectile.damage);
    }
    #endregion

    #region stat
    /// <summary>
    /// 기존 스탯에 value만큼 더함
    /// </summary>
    /// <param name="statType">스탯 타입</param>
    /// <param name="value">더해지는 값</param>
    public void ModifyStat(StatType statType, float value)
    {
        stats.ModifyStatValue(statType, value);
        UpdateStatViewer();  // 스탯 변경 후 즉시 StatViewer 업데이트
    }
    /// <summary>
    /// 기존 스탯을 value값으로 바꿈
    /// </summary>
    /// <param name="statType">스탯 타입</param>
    /// <param name="value">바꾸는 값</param>
    public void SetStat(StatType statType, float value)
    {
        stats.SetStatValue(statType, value);
        UpdateStatViewer();  // 스탯 변경 후 즉시 StatViewer 업데이트
    }
    private void UpdateStatViewer()
    {
        if (statViewer == null || stats == null) return;

        try
        {
            statViewer.Level = stats.CurrentLevel;
            statViewer.MaxExp = stats.CurrentMaxExp;
            statViewer.Exp = stats.currentExp;
            statViewer.MaxHp = stats.CurrentMaxHp;
            statViewer.Hp = stats.currentHp;
            statViewer.HpRegen = stats.CurrentHpRegen;
            statViewer.Defense = stats.CurrentDefense;
            statViewer.Mspd = stats.CurrentMspd;
            statViewer.ATK = stats.CurrentATK;
            statViewer.Aspd = stats.CurrentAspd;
            statViewer.CriRate = stats.CurrentCriRate;
            statViewer.CriDamage = stats.CurrentCriDamage;
            statViewer.ProjAmount = stats.CurrentProjAmount;
            statViewer.ATKRange = stats.CurrentATKRange;
            statViewer.Duration = stats.CurrentDuration;
            statViewer.Cooldown = stats.CurrentCooldown;
            statViewer.Revival = stats.CurrentRevival;
            statViewer.Magnet = stats.CurrentMagnet;
            statViewer.Growth = stats.CurrentGrowth;
            statViewer.Greed = stats.CurrentGreed;
            statViewer.Curse = stats.CurrentCurse;
            statViewer.Reroll = stats.CurrentReroll;
            statViewer.Banish = stats.CurrentBanish;
            statViewer.GodKill = stats.CurrentGodKill;
            statViewer.Barrier = stats.CurrentBarrier;
            statViewer.BarrierCooldown = stats.CurrentBarrierCooldown;
            statViewer.Invincibility = stats.CurrentInvincibility;
            statViewer.DashCount = stats.CurrentDashCount;
            statViewer.Adversary = stats.CurrentAdversary;
            statViewer.ProjDestroy = stats.CurrentProjDestroy;
            statViewer.ProjParry = stats.CurrentProjParry;

        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating StatViewer: {e.Message}");
        }
    }
    protected void SyncStatsFromViewer()
    {
        if (stats != null)
        {
            stats.CurrentLevel = statViewer.Level;
            stats.CurrentMaxExp = statViewer.MaxExp;
            stats.currentExp = statViewer.Exp;
            stats.CurrentMaxHp = statViewer.MaxHp;
            stats.currentHp = statViewer.Hp;
            stats.CurrentHpRegen = statViewer.HpRegen;
            stats.CurrentDefense = statViewer.Defense;
            stats.CurrentMspd = statViewer.Mspd;
            stats.CurrentATK = statViewer.ATK;
            stats.CurrentAspd = statViewer.Aspd;
            stats.CurrentCriRate = statViewer.CriRate;
            stats.CurrentCriDamage = statViewer.CriDamage;
            stats.CurrentProjAmount = statViewer.ProjAmount;
            stats.CurrentATKRange = statViewer.ATKRange;
            stats.CurrentDuration = statViewer.Duration;
            stats.CurrentCooldown = statViewer.Cooldown;
            stats.CurrentRevival = statViewer.Revival;
            stats.CurrentMagnet = statViewer.Magnet;
            stats.CurrentGrowth = statViewer.Growth;
            stats.CurrentGreed = statViewer.Greed;
            stats.CurrentCurse = statViewer.Curse;
            stats.CurrentReroll = statViewer.Reroll;
            stats.CurrentBanish = statViewer.Banish;
            stats.CurrentGodKill = statViewer.GodKill;
            stats.CurrentBarrier = statViewer.Barrier;
            stats.CurrentBarrierCooldown = statViewer.BarrierCooldown;
            stats.CurrentInvincibility = statViewer.Invincibility;
            stats.CurrentDashCount = statViewer.DashCount;
            stats.CurrentAdversary = statViewer.Adversary;
            stats.CurrentProjDestroy = statViewer.ProjDestroy;
            stats.CurrentProjParry = statViewer.ProjParry;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Env"))
        {
            WorldObject Object = collision.gameObject.GetComponent<WorldObject>();
            if (Object != null)
            {
                CollectObj(Object);
            }
            return;
        }
    }
    private void OnValidate()
    {
        if (stats != null)
        {
            SyncStatsFromViewer();
        }
    }
}
