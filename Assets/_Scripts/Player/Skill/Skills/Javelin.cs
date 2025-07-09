using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Javelin : ActiveSkill
{
    public Javelin() : base(Enums.SkillName.Javelin) { }

    protected override void SubscribeToPlayerStats()
    {
        PlayerStats playerStats = UnitManager.Instance.GetPlayer().Stats;

        playerStats.OnATKChanged += (value) => stats.aTK = value;
        playerStats.OnATKRangeChanged += (value) => stats.aTKRange = value;
        playerStats.OnCriRateChanged += (value) => stats.critical = value;
        playerStats.OnCriDamageChanged += (value) => stats.cATK = value;
        playerStats.OnDurationChanged += (value) => stats.duration = value;
        playerStats.OnProjParryChanged += (value) => stats.canParry = value;
    }

    public override void ModifySkill()
    {
        stats = new SkillStats()
        {
            defaultCooldown = 4f,
            cooldown = UnitManager.Instance.GetPlayer().Stats.CurrentCooldown,
            defaultATKRange = 1f,
            aTKRange = UnitManager.Instance.GetPlayer().Stats.CurrentATKRange,
            defaultDamage = 40f,
            aTK = UnitManager.Instance.GetPlayer().Stats.CurrentATK,
            pierceCount = 1,
            shotCount = 1,
            defaultProjectileCount = 1,
            projectileDelay = 0.1f,
            shotDelay = 0.5f,
            critical = 0.1f,
            cATK = UnitManager.Instance.GetPlayer().Stats.CurrentCriDamage,
            amount = 1f,
            lifetime = 3f,
            duration = UnitManager.Instance.GetPlayer().Stats.CurrentDuration,
            projectileSpeed = 5f,
            canParry = UnitManager.Instance.GetPlayer().Stats.CurrentProjParry,
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
                stats.defaultDamage += 20f;
                stats.pierceCount += 1;
                stats.aTKRange += 0.2f;
                stats.defaultProjectileCount++;
                break;
            case 3:
                stats.defaultDamage += 20f;
                stats.pierceCount += 1;
                stats.aTKRange += 0.2f;
                break;
            case 4:
                stats.defaultDamage += 20f;
                stats.pierceCount += 1;
                stats.aTKRange += 0.2f;
                break;
            case 5:
                stats.defaultDamage += 20f;
                stats.pierceCount += 1;
                stats.aTKRange += 0.2f;
                break;
            case 6:
                stats.defaultDamage += 30f;
                stats.pierceCount += 1;
                stats.aTKRange += 0.2f;
                break;
        }
    }
}
