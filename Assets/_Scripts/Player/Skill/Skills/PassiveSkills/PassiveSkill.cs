using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill
{
    protected Enums.StatType statType;
    protected float statModifyValue;
    protected int maxLevel = 5;

    public PassiveSkill(Enums.SkillName skillName) : base(skillName) { }

    public override void LevelUp()
    {
        if (level >= maxLevel) return;
        
        base.LevelUp();
        ModifySkill();
    }

    public override void ModifySkill()
    {
        var player = UnitManager.Instance.GetPlayer();

        player.Stats.ModifyStatValue(statType, statModifyValue);
    }

    public override void UnRegister()
    {
        var player = UnitManager.Instance.GetPlayer();
        UnRegisterRecursive(level, player);
    }

    private void UnRegisterRecursive(int currentLevel, Player player)
    {
        if (currentLevel <= 0) return;

        player.Stats.ModifyStatValue(statType, -statModifyValue);
        
        UnRegisterRecursive(currentLevel - 1, player);
    }
}
