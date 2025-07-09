using UnityEngine;

public class Water : PassiveSkill
{
    public Water() : base(Enums.SkillName.Water) { statType = Enums.StatType.HpRegen; statModifyValue = 0.4f; }
}