using UnityEngine;

public class Blood : PassiveSkill
{
    public Blood() : base(Enums.SkillName.Blood) { statType = Enums.StatType.MaxHp; statModifyValue = 10f; }
}