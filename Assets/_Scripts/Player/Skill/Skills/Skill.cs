using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using static Enums;

[Serializable]
public class Skill
{
    public SkillName skillName;

    public int level;

    protected bool isSkillActive = false;

    protected Skill(SkillName skillName)
    {
        this.skillName = skillName;

        SubscribeToPlayerStats();
        LevelUp();
    }

    protected virtual void SubscribeToPlayerStats()
    {
    }

    public virtual void StartMainTask()
    {
    }

    public virtual void StopMainTask()
    {
    }

    public virtual void ModifySkill()
    {

    }

    public virtual void Fire()
    {

    }

    public virtual void UnRegister()
    {

    }

    public virtual void LevelUp()
    {
        level++;
    }
}
