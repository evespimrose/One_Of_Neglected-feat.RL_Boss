using UnityEngine;

public class Cheese : Skill
{
    public Cheese() : base(Enums.SkillName.Cheese) { }

    public override void ModifySkill()
    {
        var player = UnitManager.Instance.GetPlayer();

        player.Stats.ModifyStatValue(Enums.StatType.Hp, 20f);
    }

    public override void LevelUp()
    {
        ModifySkill();
    }
}