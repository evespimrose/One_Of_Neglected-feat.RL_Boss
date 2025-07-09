using UnityEngine;
using static Enums;

public class Magician : Player
{
    public GameObject AttackEffect;
    protected override void Awake()
    {
        base.Awake();
        if (GetComponent<PlayerPositionLogger>() == null)
            gameObject.AddComponent<PlayerPositionLogger>();
    }

    #region Initialize
    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);
        ClassType = ClassType.Magician;
        stateHandler.RegisterState(new MagicianIdleState(stateHandler));
        stateHandler.RegisterState(new MagicianMoveState(stateHandler));
        stateHandler.RegisterState(new MagicianDashState(stateHandler));
        stateHandler.RegisterState(new MagicianAttackState(stateHandler));
        stateHandler.RegisterState(new MagicianDieState(stateHandler));


        stateHandler.ChangeState(typeof(MagicianIdleState));
    }

    protected override void InitializeStats()
    {
        stats = new PlayerStats();
    }

    protected override void InitializeClassType()
    {
        ClassType = ClassType.Magician;
    }

    protected override void InitializeStatViewer()
    {
        // 기본스텟 여기서 초기화하시오
        // 1. 가산 스탯 - 더하기
        statViewer.MaxHp = 100 + DataManager.Instance.BTS.MaxHp;

        statViewer.Defense = DataManager.Instance.BTS.Defense;
        statViewer.DashCount = DataManager.Instance.BTS.DashCount;
        statViewer.Level = 1;
        statViewer.MaxExp = 100;
        statViewer.Exp = 0;
        statViewer.Hp = 100 + DataManager.Instance.BTS.MaxHp;
        statViewer.HpRegen = DataManager.Instance.BTS.HpRegen;     
        statViewer.Revival = DataManager.Instance.BTS.Revival;
        statViewer.Reroll = DataManager.Instance.BTS.Reroll;
        statViewer.Banish = DataManager.Instance.BTS.Banish;
        statViewer.ProjAmount = 1 + DataManager.Instance.BTS.ProjAmount;
        statViewer.CriRate = DataManager.Instance.BTS.CriRate;

        // 2. 승산 스탯 - (100 + 증가율) / 100
        statViewer.ATK = 10 * ((DataManager.Instance.BTS.ATK + 100) / 100);
        statViewer.Mspd = 1.3f * ((100 + DataManager.Instance.BTS.Mspd) / 100f);
        statViewer.Aspd = 1 * ((100 + DataManager.Instance.BTS.Aspd) / 100f);
        statViewer.ATKRange = 1 * ((100 + DataManager.Instance.BTS.ATKRange) / 100f);
        statViewer.Duration = 1 * ((100 + DataManager.Instance.BTS.Duration) / 100f);
        statViewer.Magnet = 0.5f * ((100 + DataManager.Instance.BTS.Magnet) / 100f);
        statViewer.Growth = (100 + DataManager.Instance.BTS.Growth) / 100f;
        statViewer.Greed = 1 * ((100 + DataManager.Instance.BTS.Greed) / 100f);
        statViewer.Curse = 1 * ((100 + DataManager.Instance.BTS.Curse) / 100f);
        statViewer.CriDamage = 1 * ((50 + DataManager.Instance.BTS.CriDamage + 100) / 100f);

        // 3. 감산 스탯 - (100 - 감소율) / 100
        statViewer.Cooldown = 1f * ((100 - DataManager.Instance.BTS.Cooldown) / 100f);

        // 4. 기타 스탯
        statViewer.GodKill = DataManager.Instance.BTS.GodKill;
        statViewer.Barrier = DataManager.Instance.BTS.Barrier;
        statViewer.BarrierCooldown = 5 + DataManager.Instance.BTS.BarrierCooldown;
        statViewer.Invincibility = DataManager.Instance.BTS.Invincibility;
        statViewer.ProjDestroy = DataManager.Instance.BTS.ProjDestroy;
        statViewer.ProjParry = DataManager.Instance.BTS.projParry;


    }
    #endregion
}