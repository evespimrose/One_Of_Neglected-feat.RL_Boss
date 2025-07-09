using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : PassiveSkill
{
    public Book() : base(Enums.SkillName.Book) { statType = Enums.StatType.ATKRange; statModifyValue = 10f; }
}
