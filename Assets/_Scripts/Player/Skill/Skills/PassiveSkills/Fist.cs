using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : PassiveSkill
{
    public Fist() : base(Enums.SkillName.Fist) { statType = Enums.StatType.ATK; statModifyValue = 20f; }
}
