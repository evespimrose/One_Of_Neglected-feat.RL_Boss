using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : PassiveSkill
{
    public Magnet() : base(Enums.SkillName.Magnet) { statType = Enums.StatType.Magnet; statModifyValue = 30f; }
}
