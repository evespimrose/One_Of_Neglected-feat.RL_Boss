using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class SkillContainer : MonoBehaviour
{
    private List<SkillName> ownedSkills = new List<SkillName>();

    private List<SkillName> selectableSkills = new List<SkillName>();

    public Dictionary<SkillName, int> removedSkills = new Dictionary<SkillName, int>();

    private int maxActiveSkills = 5;
    private int maxPassiveSkills = 5;

    public IReadOnlyList<SkillName> OwnedSkills => ownedSkills.AsReadOnly();
    public List<SkillName> SelectableSkills => selectableSkills;


    public void AddSkill(SkillName skillName)
    {
        if (!ownedSkills.Contains(skillName))
        {
            ownedSkills.Add(skillName);
        }
    }

    private void Awake()
    {
        foreach (SkillName skillName in Enum.GetValues(typeof(SkillName)))
        {
            if (skillName == SkillName.None || skillName == SkillName.Cheese || skillName == SkillName.Gold) continue;
            AddSelectableSkill(skillName);
        }
    }

    public void AddSelectableSkill(SkillName skillName)
    {
        if (!selectableSkills.Contains(skillName))
        {
            selectableSkills.Add(skillName);
        }
    }

    public List<SkillName> GetAvailableSkills()
    {
        return new List<SkillName>(selectableSkills);
    }

    public SkillName GetSkill(SkillName skillName)
    {
        if (ownedSkills.Contains(skillName))
        {
            return skillName;
        }
        return SkillName.None;
    }

    public bool CanAddActiveSkill()
    {
        int activeSkillCount = CountActiveSkills();
        return activeSkillCount < maxActiveSkills;
    }

    public bool CanAddPassiveSkill()
    {
        int passiveSkillCount = CountPassiveSkills();
        return passiveSkillCount < maxPassiveSkills;
    }

    private int CountActiveSkills()
    {
        int count = 0;
        foreach (var skillName in ownedSkills)
        {
            if (SkillFactory.IsActiveSkill(skillName) == 1) count++;
        }
        return count;
    }

    private int CountPassiveSkills()
    {
        int count = 0;
        foreach (var skillName in ownedSkills)
        {
            if (SkillFactory.IsActiveSkill(skillName) == 0) count++;
        }
        return count;
    }

    public void RemoveSkill(SkillName deDuctSkillName)
    {
        if (ownedSkills.Contains(deDuctSkillName))
        {
            ownedSkills.Remove(deDuctSkillName);
            selectableSkills.Remove(deDuctSkillName);
        }
    }
}
