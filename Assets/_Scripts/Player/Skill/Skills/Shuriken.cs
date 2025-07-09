using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : ActiveSkill
{
    public Shuriken() : base(Enums.SkillName.Shuriken) { }

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
        stats = new SkillStats()
        {
            defaultCooldown = 1f,
            cooldown = UnitManager.Instance.GetPlayer().Stats.CurrentCooldown,
            defaultATKRange = 1f,
            aTKRange = UnitManager.Instance.GetPlayer().Stats.CurrentATKRange,
            defaultDamage = 5f,
            aTK = UnitManager.Instance.GetPlayer().Stats.CurrentATK,
            pierceCount = 0,
            shotCount = 1,
            defaultProjectileCount = 1,
            projectileDelay = 0.1f,
            shotDelay = 0.5f,
            critical = 0.1f,
            cATK = UnitManager.Instance.GetPlayer().Stats.CurrentCriDamage,
            duration = UnitManager.Instance.GetPlayer().Stats.CurrentDuration,
            amount = 1f,
            lifetime = 3f,
            projectileSpeed = 3f,
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
                stats.defaultProjectileCount++;
                stats.pierceCount++;
                break;
            case 3:
                stats.defaultProjectileCount++;
                stats.pierceCount++;
                break;
            case 4:
                stats.defaultProjectileCount++;
                stats.pierceCount++;
                break;
            case 5:
                stats.defaultProjectileCount++;
                stats.pierceCount++;
                break;
            case 6:
                stats.defaultProjectileCount++;
                stats.pierceCount++;
                break;
        }
    }
    
}
