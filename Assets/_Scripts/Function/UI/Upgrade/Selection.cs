using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Selection : MonoBehaviour
{
    [SerializeField] private Banish_Panel banish_Panel;
    [SerializeField] private InGameUI_Panel inGameUI_Panel;
    [SerializeField] private LevelUp_Panel levelUp_Panel;

    public Enums.SkillName m_skillName;
    public Enums.AugmentName m_augType;
    public Button m_BTN;
    public TextMeshProUGUI display_Name;
    public TextMeshProUGUI info_TMP;
    public Image icon_IMG;
    public Image m_selectionBG_IMG;
    public void Select_BTN()
    {
        if (inGameUI_Panel.skillContainer.
        GetSkill(m_skillName) == Enums.SkillName.None)
        {
            if (SkillFactory.IsActiveSkill(m_skillName) != 2)
            {
                inGameUI_Panel.SetIconCell_Mini(m_skillName);
                banish_Panel.SetIconCell_Banish(m_skillName);
            }
        }
        inGameUI_Panel.skillSelector.ChooseSkill(m_skillName);

        if (SkillFactory.IsActiveSkill(m_skillName) == 1)
        {
            if (levelUp_Panel.m_MainSkills.Contains(m_skillName) == false)
            {
                levelUp_Panel.m_MainSkills.Add(m_skillName);
                levelUp_Panel.m_MainSkill_Time.Add(m_skillName, TimeManager.Instance.gameTime);
            }
        }
        else if (SkillFactory.IsActiveSkill(m_skillName) == 0)
        {
            if (levelUp_Panel.m_SubSkills.Contains(m_skillName) == false)
            {
                levelUp_Panel.m_SubSkills.Add(m_skillName);
                levelUp_Panel.m_SubSkill_Time.Add(m_skillName, TimeManager.Instance.gameTime);
            }
        }
        levelUp_Panel.PanelClose(true);
    }

    public void Select_BTN2()
    {

        UnitManager.Instance.GetPlayer().augment.ChooseAugment(m_augType);
        levelUp_Panel.augUpCount++;
        levelUp_Panel.SetAugTextInit(m_augType);

        levelUp_Panel.ChangeSelections();
    }
}