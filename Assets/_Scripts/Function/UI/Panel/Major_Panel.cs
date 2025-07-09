using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Major_Panel : MonoBehaviour
{
    #region 구조체 영역
    [Serializable]
    public struct Base_Info
    {
        public TextMeshProUGUI time;
        public TextMeshProUGUI killCount;
        public TextMeshProUGUI gold;
        public TextMeshProUGUI remnents;
        public Image player_Portrait;
    }
    [Serializable]
    public struct Augment_TMP
    {
        public TextMeshProUGUI augName;
        public TextMeshProUGUI augDamage;
        public TextMeshProUGUI augTime;
    }
    #endregion

    [Header("Left_Display")]
    public Base_Info base_Info;
    [SerializeField] private List<Image> mainSkill_Icons;
    [SerializeField] private List<Image> subSkill_Icons;
    [Header("Right_Display")]
    [SerializeField] private RectTransform skillMemberParent;
    [SerializeField] private GameObject skillMemeber_Prefab;
    public Augment_TMP augment_TMP;
    public List<SkillMember> skillMembers;
    public InGameUI_Panel inGameUI_Panel;
    public LevelUp_Panel levelUp_Panel;

    private void Start()
    {
        if (inGameUI_Panel == null)
            inGameUI_Panel =
            UI_Manager.Instance.panel_Dic["InGameUI_Panel"].
            GetComponent<InGameUI_Panel>();
        if (levelUp_Panel == null)
            levelUp_Panel =
            UI_Manager.Instance.panel_Dic["LevelUp_Panel"].
            GetComponent<LevelUp_Panel>();

        SkillIconSetting();
        BaseInfoSetting();
        AugInfoSetting();
        SkillInfoSetting(ref levelUp_Panel.m_MainSkills, true);
        SkillInfoSetting(ref levelUp_Panel.m_SubSkills, false, levelUp_Panel.m_MainSkills.Count);
    }

    //스킬 정보
    private void SkillInfoSetting(ref List<Enums.SkillName> skillNames, bool isActivesSkill, int startIdx = 0)
    {
        if (skillNames.Count == 0) return;

        for (int i = 0; i < skillNames.Count; i++)
        {
            GameObject member = Instantiate(skillMemeber_Prefab, skillMemberParent, false);

            skillMembers.Add(member.GetComponent<SkillMember>());
            // Debug.Log($"i : {i}");
            // Debug.Log($"startIdx : {startIdx}");
            skillMembers[startIdx].icon.sprite =
            levelUp_Panel.FindSkillIcon(skillNames[i]);

            skillMembers[startIdx].skillName.text =
            levelUp_Panel.FindSkillName(skillNames[i]);

            skillMembers[startIdx].level.text =
            "Lv." + inGameUI_Panel.skillSelector.
            SkillLevel(skillNames[i]).ToString();
            if (isActivesSkill)
            {
                if (false == DataManager.Instance.currentDamageStats.skillDamages.ContainsKey(skillNames[i])) continue;
                skillMembers[startIdx].damage.text =
                DataManager.Instance.currentDamageStats.skillDamages[skillNames[i]].ToString();

                float time = TimeManager.Instance.gameTime -
                levelUp_Panel.m_MainSkill_Time[skillNames[i]];

                skillMembers[startIdx].time.text = inGameUI_Panel.TimeCalc(time);
            }
            else
            {
                float time = TimeManager.Instance.gameTime -
                levelUp_Panel.m_SubSkill_Time[skillNames[i]];

                skillMembers[startIdx].time.text = inGameUI_Panel.TimeCalc(time);
            }
            startIdx++;
        }

    }

    //증강 정보
    public void AugInfoSetting()
    {
        if (levelUp_Panel.augUpCount - 1 < 0) return;

        augment_TMP.augName.text =
        levelUp_Panel.aug_Property.aug_Name[levelUp_Panel.augUpCount - 1];
        try
        {
            augment_TMP.augDamage.text =
            DataManager.Instance.currentDamageStats.augmentDamages[levelUp_Panel.aug_Property.aug_Type[0]].ToString();
        }
        catch
        {
            augment_TMP.augDamage.text = "0";
        }
        float time = TimeManager.Instance.gameTime -
                     levelUp_Panel.aug_Property.selectedTime;

        augment_TMP.augTime.text = inGameUI_Panel.TimeCalc(time);
    }

    //스킬 아이콘
    private void SkillIconSetting()
    {

        for (int i = 0; i < mainSkill_Icons.Count; i++)
        {
            mainSkill_Icons[i].sprite = inGameUI_Panel.mainSkill_Icon_Container[i].sprite;
            if (mainSkill_Icons[i].sprite == null)
                mainSkill_Icons[i].sprite = Resources.Load<Sprite>("Using/UI/Icon/Custom/Icon_Border_BG");
        }
        for (int i = 0; i < subSkill_Icons.Count; i++)
        {
            subSkill_Icons[i].sprite = inGameUI_Panel.subSkill_Icon_Container[i].sprite;
            if (subSkill_Icons[i].sprite == null)
                subSkill_Icons[i].sprite = Resources.Load<Sprite>("Using/UI/Icon/Custom/Icon_Border_BG");
        }
    }

    //기본 정보
    private void BaseInfoSetting()
    {

        base_Info.time.text =
        inGameUI_Panel.TimeCalc(TimeManager.Instance.gameTime);

        base_Info.killCount.text =
        DataManager.Instance.inGameValue.killCount.ToString();

        base_Info.gold.text =
        DataManager.Instance.inGameValue.gold.ToString();

        base_Info.remnents.text =
        DataManager.Instance.inGameValue.remnents.ToString();

        base_Info.player_Portrait.sprite =
        DataManager.Instance.inGameValue.playerIcon;
    }
}
