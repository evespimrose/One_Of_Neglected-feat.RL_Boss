using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : ActiveSkill
{
    private bool hasSpawned = false;

    public Aura() : base(Enums.SkillName.Aura) { }

    protected override async UniTask StartSkill()
    {
        isSkillActive = true;

        while (true)
        {
            if (!GameManager.Instance.isPaused && isSkillActive)
            {
                if (!hasSpawned)
                {
                    Fire();
                    hasSpawned = true;
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            else
            {
                hasSpawned = false;
                await UniTask.Yield();
            }
        }
    }

    protected override void SubscribeToPlayerStats()
    {
        PlayerStats playerStats = UnitManager.Instance.GetPlayer().Stats;

        playerStats.OnATKChanged += (value) => { stats.aTK = value; Fire(); };
        playerStats.OnATKRangeChanged += (value) => {stats.aTKRange = value; Fire(); };
        playerStats.OnCriRateChanged += (value) => {stats.critical = value; Fire(); };
        playerStats.OnCriDamageChanged += (value) => {stats.cATK = value; Fire(); };
        playerStats.OnATKRangeChanged += (value) => {stats.aTKRange = value; Fire(); };
    }

    public override void ModifySkill()
    {
        // init SkillStats
        stats = new SkillStats()
        {
            defaultCooldown = 2f,
            cooldown = UnitManager.Instance.GetPlayer().Stats.CurrentCooldown,
            defaultATKRange = 0.2f,
            aTKRange = UnitManager.Instance.GetPlayer().Stats.CurrentATKRange,
            defaultDamage = 1f,
            aTK = UnitManager.Instance.GetPlayer().Stats.CurrentATK,
            pierceCount = 0,
            shotCount = 1,
            defaultProjectileCount = 1,
            projectileDelay = 0.1f,
            shotDelay = 0.5f,
            critical = 0.1f,
            cATK = UnitManager.Instance.GetPlayer().Stats.CurrentCriDamage,
            amount = 1f,
            lifetime = 5f,
            projectileSpeed = 1f,
        };
    }

    public override void LevelUp()
    {
        base.LevelUp();

        if (level >= 7)
        {
            level = 6;
            return;
        }

        switch (level)
        {
            case 2:
                stats.defaultDamage += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 3:
                stats.defaultDamage += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 4:
                stats.defaultDamage += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 5:
                stats.defaultDamage += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 6:
                stats.defaultDamage += 1f;
                stats.defaultATKRange += 0.1f;
                break;
        }
        Fire();
    }
}