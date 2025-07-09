using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : PassiveSkill
{
    public Crown() : base(Enums.SkillName.Crown) { statType = Enums.StatType.Growth; statModifyValue = 10f; }
}
