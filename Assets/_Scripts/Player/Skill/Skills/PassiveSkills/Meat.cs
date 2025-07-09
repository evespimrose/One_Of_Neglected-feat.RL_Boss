using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : PassiveSkill
{
    public Meat() : base(Enums.SkillName.Meat) { statType = Enums.StatType.Greed; statModifyValue = 10f; }
}
