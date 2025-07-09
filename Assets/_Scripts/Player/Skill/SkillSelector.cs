using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class SkillSelector : MonoBehaviour
{
    private SkillContainer skillContainer;
    private SkillDispenser skillDispenser;

    private void Start()
    {
        skillContainer = GetComponent<SkillContainer>();
        if (UnitManager.Instance.GetPlayer().TryGetComponent(out skillDispenser))
            print("Success");
    }

    public void Initialize(SkillContainer container, SkillDispenser dispenser)
    {
        skillContainer = container;
        skillDispenser = dispenser;
    }

    public List<SkillName> SelectSkills()
    {
        List<SkillName> availableSkills = skillContainer.GetAvailableSkills();
        HashSet<SkillName> selectedSkills = new HashSet<SkillName>();

        bool activeMax = !skillContainer.CanAddActiveSkill();
        bool passiveMax = !skillContainer.CanAddPassiveSkill();

        if (activeMax && passiveMax)
        {
            AddOwnedSkills(selectedSkills);
        }
        else
        {
            foreach (var skill in availableSkills)
            {
                if (IsMaxLevel(skill)) continue;

                SkillName skillName = (!activeMax && !passiveMax) ||
                    (activeMax && (IsPassiveSkill(skill) || skillContainer.OwnedSkills.Contains(skill))) ||
                    (passiveMax && (IsActiveSkill(skill) || skillContainer.OwnedSkills.Contains(skill)))
                    ? skill : SkillName.None;
                selectedSkills.Add(skillName);
            }

            selectedSkills.RemoveWhere(skill => skill == SkillName.None);
        }

        var skillList = selectedSkills.ToList();

        AddEtcSkills(skillList);

        // Fisher - Yates Shuffle
        Shuffle(skillList);

        return skillList.Take(3).ToList();
    }

    private void AddOwnedSkills(HashSet<SkillName> selectedSkills)
    {
        foreach (var skill in skillContainer.OwnedSkills)
        {
            if (!IsMaxLevel(skill))
            {
                selectedSkills.Add(skill);
            }
        }
    }

    private void Shuffle(List<SkillName> skillList)
    {
        int n = skillList.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (skillList[k], skillList[n]) = (skillList[n], skillList[k]);
        }
    }

    private void AddEtcSkills(List<SkillName> skillList)
    {
        if (skillList.Count <= 1)
        {
            if (!skillList.Contains(SkillName.Cheese)) skillList.Add(SkillName.Cheese);
            if (!skillList.Contains(SkillName.Gold)) skillList.Add(SkillName.Gold);
        }
        else if (skillList.Count == 2)
        {
            SkillName defaultSkill = UnityEngine.Random.value > 0.5f ? SkillName.Cheese : SkillName.Gold;
            if (!skillList.Contains(defaultSkill)) skillList.Add(defaultSkill);
        }
    }

    public void ChooseSkill(SkillName chosenAbility)
    {
        SkillName skillName = skillContainer.GetSkill(chosenAbility);

        if (skillName == SkillName.None)
        {
            skillDispenser.RegisterSkill(chosenAbility);
            skillContainer.AddSkill(chosenAbility);
            return;
        }
        else
        {
            skillDispenser.RegisterSkill(skillName);
        }

        if (IsMaxLevel(skillName))
        {
            skillContainer.SelectableSkills.Remove(skillName);
        }
    }

    public void DeductSkill(SkillName deDuctSkillName)
    {
        if (skillDispenser.skills.ContainsKey(deDuctSkillName))
        {
            skillContainer.removedSkills.Add(deDuctSkillName, skillDispenser.skills[deDuctSkillName].level);

            skillDispenser.UnRegisterSkill(deDuctSkillName);

            skillContainer.RemoveSkill(deDuctSkillName);
        }
    }

    public int SkillLevel(SkillName skillName)
    {
        if (skillDispenser.skills.ContainsKey(skillName))
        {
            return skillDispenser.skills[skillName].level;
        }
        else if (skillContainer.removedSkills.ContainsKey(skillName))
        {
            return skillContainer.removedSkills[skillName];
        }
        else
        {
            return -1;
        }
    }

    private bool IsActiveSkill(SkillName skillName)
    {
        return SkillFactory.IsActiveSkill(skillName) == 1;
    }

    private bool IsPassiveSkill(SkillName skillName)
    {
        return SkillFactory.IsActiveSkill(skillName) == 0;
    }

    private bool IsMaxLevel(SkillName skillName)
    {
        if (skillName == SkillName.Ring) return SkillLevel(skillName) >= 2;
        return IsActiveSkill(skillName) ? SkillLevel(skillName) >= 6 : SkillLevel(skillName) >= 5;
    }
}
