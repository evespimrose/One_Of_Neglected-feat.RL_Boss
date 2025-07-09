using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Skill
{
    public Gold() : base(Enums.SkillName.Gold) { }

    public override void ModifySkill()
    {
        DataManager.Instance.inGameValue.gold += 20;
    }

    public override void LevelUp()
    {
        ModifySkill();
    }

}
