using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bracelet : PassiveSkill
{
    public Bracelet() : base(Enums.SkillName.Bracelet) { statType = Enums.StatType.Duration; statModifyValue = 10f; }
}
