using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class InGameUI_Panel : Panel
{
    private Player player;
    private int min;
    private int sec;
    [SerializeField] private LevelUp_Panel levelUp_Panel;
    [SerializeField] private TextMeshProUGUI display_Time_TMP;
    [SerializeField] private RectTransform main_Icon_Rect;
    [SerializeField] private RectTransform sub_Icon_Rect;
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Slider expSlider;
    public List<Image> mainSkill_Icon_Container;
    public List<Image> subSkill_Icon_Container;
    public SkillSelector skillSelector;
    public SkillContainer skillContainer;
    public TextMeshProUGUI display_Level_TMP;
    public TextMeshProUGUI goldDisplay;
    public TextMeshProUGUI remnentsDisplay;
    public TextMeshProUGUI killCountDisplay;
    private void Awake()
    {
        buttons[0].onClick.AddListener(Auto_BTN);
    }

    private void Start()
    {
        player = UnitManager.Instance.GetPlayer();

        if (skillSelector == null) skillSelector = GetComponent<SkillSelector>();
        skillSelector.Initialize(skillContainer, UnitManager.Instance.GetPlayer().GetComponent<SkillDispenser>());
        display_Level_TMP.text = "Lv." + UnitManager.Instance.GetPlayer().statViewer.Level;
    }
    private void Update()
    {

        expSlider.value = (float)player.Stats.currentExp / player.Stats.CurrentMaxExp;
        if (UnitManager.Instance.GetPlayer().Stats.currentHp > 0)
            display_Time_TMP.text = "Time : " + TimeCalc(TimeManager.Instance.gameTime);
        goldDisplay.text = "골드 : " + DataManager.Instance.inGameValue.gold.ToString();
        remnentsDisplay.text = "사후잔념 : " + DataManager.Instance.inGameValue.remnents.ToString();
        killCountDisplay.text = "킬 : " + DataManager.Instance.inGameValue.killCount.ToString();
    }
    public void SetIconCell_Mini(Enums.SkillName skillName)
    {

        if (SkillFactory.IsActiveSkill(skillName) == 1)
        {
            SetIcons(mainSkill_Icon_Container, skillName);
        }
        else
        {
            SetIcons(subSkill_Icon_Container, skillName);
        }

    }
    public void SetIcons(List<Image> skill_List, Enums.SkillName skillName)
    {
        foreach (Image image in skill_List)
        {
            if (image.sprite == null)
            {
                image.sprite = levelUp_Panel.skill_Info_Dic[skillName].skill_Sprite;
                image.color = Color.white;
                break;
            }
        }
    }
    public void Remove_MiniIcon(Enums.SkillName skillName, Sprite sprite)
    {
        if (SkillFactory.IsActiveSkill(skillName) == 1)
        {
            Icon_Replace(mainSkill_Icon_Container, sprite, main_Icon_Rect);
            levelUp_Panel.m_MainSkills.Remove(skillName);
            levelUp_Panel.m_MainSkill_Time.Remove(skillName);
        }
        else
        {
            Icon_Replace(subSkill_Icon_Container, sprite, sub_Icon_Rect);
            levelUp_Panel.m_SubSkills.Remove(skillName);
            levelUp_Panel.m_SubSkill_Time.Remove(skillName);
        }
    }

    public void Icon_Replace(List<Image> icon_Container, Sprite sprite, RectTransform parent_Rect)
    {
        foreach (Image image in icon_Container)
        {
            if (image.sprite == sprite)
            {
                image.sprite = null;
                image.color = Color.clear;
                image.transform.parent.SetParent(null);
                image.transform.parent.SetParent(parent_Rect);

            }
        }
    }

    public string TimeCalc(float time)
    {
        min = (int)time / 60;
        sec = (int)time % 60;

        return min.ToString("00") + " : " + sec.ToString("00");
    }

    private void Auto_BTN()
    {
        UnitManager.Instance.GetPlayer().isAuto = !UnitManager.Instance.GetPlayer().isAuto;
    }
}
