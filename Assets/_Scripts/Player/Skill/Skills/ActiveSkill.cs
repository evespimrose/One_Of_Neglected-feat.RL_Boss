using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    public SkillStats stats;

    public float FinalDamage => stats.defaultDamage + stats.aTK;       // ex) 3 + (3 per level) + Player's ATK Stat

    public float AdvancedCooldown => stats.defaultCooldown * stats.cooldown;
    public float FinalCooldown => MathF.Max(((FinalProjAmount * stats.projectileDelay) + stats.shotDelay) * stats.amount, AdvancedCooldown);       // ex) (1 per 3 seconds) * (1 - 0.3);

    public float FinalATKRange => stats.defaultATKRange * stats.aTKRange;

    public int FinalProjAmount => stats.defaultProjectileCount + UnitManager.Instance.GetPlayer().Stats.CurrentProjAmount - 1;

    public float FinalDuration => stats.lifetime * stats.duration;

    public ActiveSkill(Enums.SkillName skillName) : base(skillName)
    {
        // init Stats
        ModifySkill();

        SubscribeToPlayerStats();
    }

    public override async void StartMainTask()
    {
        await StartSkill();
    }

    public override void StopMainTask()
    {
        isSkillActive = false;
    }

    protected virtual async UniTask StartSkill()
    {
        isSkillActive = true;

        while (isSkillActive)
        {
            if (!GameManager.Instance.isPaused)
            {
                Fire();
                await UniTask.Delay(TimeSpan.FromSeconds(FinalCooldown));
            }
            else
            {
                await UniTask.Yield();
            }
        }
    }

    public override void Fire()
    {
        base.Fire();
        //ProjectileManager.Instance.SpawnProjectile(skillName, stats.defaultDamage, level, stats.shotCount, stats.projectileCount, stats.projectileDelay, stats.shotDelay, stats.pierceCount);
        ProjectileManager.Instance.SpawnProjectile(skillName,
            new ProjectileStats()
            {
                skillName = skillName,
                level = level,
                finalCooldown = FinalCooldown,
                finalATKRange = FinalATKRange,
                finalDamage = FinalDamage,
                pierceCount = stats.pierceCount,
                shotCount = stats.shotCount,
                finalProjectileCount = FinalProjAmount,
                projectileDelay = stats.projectileDelay,
                shotDelay = stats.shotDelay,
                critical = stats.critical,
                cATK = stats.cATK,
                amount = stats.amount,
                finalDuration = FinalDuration,
                projectileSpeed = stats.projectileSpeed,
                canParry = stats.canParry,
            }
            );

        if(skillName != Enums.SkillName.Mine || skillName != Enums.SkillName.Aura) SoundManager.Instance.Play(skillName.ToString(), SoundManager.Sound.Effect, 1f, false, 1f);
    }

}