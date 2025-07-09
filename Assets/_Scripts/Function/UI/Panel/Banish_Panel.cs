using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Banish_Panel : Panel
{
    [SerializeField] private LevelUp_Panel levelUp_Panel;
    [SerializeField] private List<Banish_Icon> mainIcon_List;
    [SerializeField] private List<Banish_Icon> subIcon_List;
    [SerializeField] private RectTransform mainBanish_Rect;
    [SerializeField] private RectTransform subBanish_Rect;
    [SerializeField] private Sprite defaultIcon;

    public SkillDispenser skillDispenser;
    private void Awake()
    {
        buttons[0].onClick.AddListener(Return_BTN);
    }
    private void OnEnable()
    {
        if (skillDispenser == null)
        {
            skillDispenser = UnitManager.Instance.GetPlayer().gameObject.GetComponent<SkillDispenser>();
        }
    }
    private void Return_BTN()
    {
        UI_Manager.Instance.panel_Dic["Banish_Panel"].PanelClose(true);
        levelUp_Panel.SelectionOnOff(true);
    }

    public void SetIconCell_Banish(Enums.SkillName skillName)
    {
        if (SkillFactory.IsActiveSkill(skillName) == 1)
        {
            SetIcons(mainIcon_List, skillName);
        }
        else
        {
            SetIcons(subIcon_List, skillName);
        }
    }
    public void SetIcons(List<Banish_Icon> iconContainer, Enums.SkillName skillName)
    {
        foreach (Banish_Icon activeSkill in iconContainer)
        {
            if (activeSkill.m_SkillName == skillName) return;
            if (activeSkill.m_SkillName == Enums.SkillName.None)
            {
                Skill_Info skill_Info =
                levelUp_Panel.skill_Info_Dic[skillName];

                activeSkill.m_SkillName = skill_Info.skill_Name;
                activeSkill.m_Icon.sprite = skill_Info.skill_Sprite;
                activeSkill.m_Icon.color = Color.white;
                activeSkill.m_BTN.interactable = true;
                return;
            }
        }
    }
    public void Remove_BanishIcon(Banish_Icon icon)
    {
        icon.m_Icon.sprite = defaultIcon;
        if (SkillFactory.IsActiveSkill(icon.m_SkillName) == 1)
        {
            ReplacingCell(mainIcon_List, icon, mainBanish_Rect);
        }
        else
        {
            ReplacingCell(subIcon_List, icon, subBanish_Rect);
        }

    }
    private void ReplacingCell(List<Banish_Icon> icon_Container, Banish_Icon icon, RectTransform parent_Rect)
    {
        icon_Container.Remove(icon);
        icon_Container.Add(icon);
        icon.transform.SetParent(null);
        icon.transform.SetParent(parent_Rect);
    }
}
