using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoes : PassiveSkill
{
    public Shoes() : base(Enums.SkillName.Shoes) { statType = Enums.StatType.Mspd; statModifyValue = 10f; }
}
