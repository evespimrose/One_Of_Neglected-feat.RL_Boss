using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : PassiveSkill
{
    public Clock() : base(Enums.SkillName.Clock) { statType = Enums.StatType.Cooldown; statModifyValue = 10f; }
}
