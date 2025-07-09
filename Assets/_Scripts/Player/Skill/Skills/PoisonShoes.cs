using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;
using System;
using Unity.VisualScripting;

public class PoisonShoes : ActiveSkill
{
    public PoisonShoes() : base(Enums.SkillName.PoisonShoes) { }

    protected override void SubscribeToPlayerStats()
    {
        PlayerStats playerStats = UnitManager.Instance.GetPlayer().Stats;

        playerStats.OnATKChanged += (value) => stats.aTK = value;
        playerStats.OnATKRangeChanged += (value) => stats.aTKRange = value;
        playerStats.OnCriRateChanged += (value) => stats.critical = value;
        playerStats.OnCriDamageChanged += (value) => stats.cATK = value;
        playerStats.OnDurationChanged += (value) => stats.duration = value;
    }

    public override void ModifySkill()
    {
        // init SkillStats
        Debug.Log("InitSkill!!");
        stats = new SkillStats()
        {
            defaultCooldown = 1.1f,
            cooldown = UnitManager.Instance.GetPlayer().Stats.CurrentCooldown,

            defaultATKRange = 1f,
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
            lifetime = 1.1f,
            duration = UnitManager.Instance.GetPlayer().Stats.CurrentDuration,
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
                stats.defaultDamage += 10f;
                break;
            case 3:
                stats.lifetime += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 4:
                stats.defaultDamage += 10f;
                break;
            case 5:
                stats.lifetime += 1f;
                stats.defaultATKRange += 0.1f;
                break;
            case 6:
                stats.defaultDamage += 10f;
                stats.defaultATKRange += 0.1f;
                break;
        }
    }

}
