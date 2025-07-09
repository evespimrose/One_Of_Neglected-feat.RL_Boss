using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : PassiveSkill
{
    public Ring() : base(Enums.SkillName.Ring) 
    { 
        statType = Enums.StatType.ProjAmount; 
        statModifyValue = 1f;
        maxLevel = 2;
    }
}
