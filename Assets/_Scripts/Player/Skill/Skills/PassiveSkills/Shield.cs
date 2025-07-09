using UnityEngine;

public class Shield : PassiveSkill
{
    public Shield() : base(Enums.SkillName.Shield) { statType = Enums.StatType.Defense; statModifyValue = 1f; }
}