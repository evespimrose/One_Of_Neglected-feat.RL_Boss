using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRan = UnityEngine.Random;

[Serializable]
public struct Skill_Info
{
    public Enums.SkillName skill_Name;
    public string display_Name;
    [Multiline(4)]
    // public string skill_Text;
    public List<string> skill_Texts;
    public Sprite skill_Sprite;
    public float selectedTime;
}

public class LevelUp_Panel : Panel
{
    [SerializeField] InGameUI_Panel inGameUI_Panel;
    [SerializeField] private List<Selection> current_Selections;
    [SerializeField] private Augment_Info aug_Infos = new();
    [SerializeField] private List<Skill_Info> skill_Infos;
    [SerializeField] private TextMeshProUGUI reroll_Counter_TMP;
    [SerializeField] private TextMeshProUGUI banish_Counter_TMP;

    public Dictionary<Enums.SkillName, Skill_Info> skill_Info_Dic =
     new Dictionary<Enums.SkillName, Skill_Info>();

    public List<Enums.SkillName> m_MainSkills;
    public List<Enums.SkillName> m_SubSkills;

    public Dictionary<Enums.SkillName, float> m_MainSkill_Time =
    new Dictionary<Enums.SkillName, float>();

    public Dictionary<Enums.SkillName, float> m_SubSkill_Time =
    new Dictionary<Enums.SkillName, float>();

    public int augUpCount = 0;
    public bool isAugSelected = false;

    public int augUpCount_Property
    {
        get { return augUpCount; }
        private set { augUpCount_Property = augUpCount; }
    }

    public Augment_Info aug_Property
    {
        get { return aug_Infos; }
        private set { aug_Property = aug_Infos; }
    }
    private SkillDispenser skillDispenser;
    private void Awake()
    {
        skillDispenser = UnitManager.Instance.GetPlayer().gameObject.GetComponent<SkillDispenser>();
        augUpCount = 0;
        if (skill_Infos == null) { skill_Infos = new List<Skill_Info>(); }

        skill_Info_Dic = new Dictionary<Enums.SkillName, Skill_Info>();

        foreach (Skill_Info skill_Info in skill_Infos)
        {
            skill_Info_Dic.Add(skill_Info.skill_Name, skill_Info);
        }

        if (buttons != null && buttons.Count >= 2)
        {
            buttons[0].onClick.AddListener(Reroll_BTN);
            buttons[1].onClick.AddListener(Banish_BTN);
        }

        if (reroll_Counter_TMP != null && DataManager.Instance?.BTS != null)
        {
            reroll_Counter_TMP.text = DataManager.Instance.BTS.Reroll.ToString();
        }

        if (banish_Counter_TMP != null && DataManager.Instance?.BTS != null)
        {
            banish_Counter_TMP.text = DataManager.Instance.BTS.Banish.ToString();
        }
        // Debug.LogWarning("AWAKE");
        // Debug.Log(DataManager.Instance.classSelect_Type);
        switch (DataManager.Instance.classSelect_Type)
        {
            case Enums.ClassType.Warrior:
                aug_Infos.WarroirInit();
                break;
            case Enums.ClassType.Archer:
                aug_Infos.ArcherInit();
                break;
            case Enums.ClassType.Magician:
                aug_Infos.MagicianInit();
                break;
            default:
                break;
        }
    }

    private void OnEnable()
    {
        SoundManager.Instance.Play("LevelUp", SoundManager.Sound.Effect);
        if (SceneManager.GetActiveScene().name != "Game") return;
        Time.timeScale = 0;
        inGameUI_Panel.display_Level_TMP.text =
        "Lv." + UnitManager.Instance.GetPlayer().Stats.CurrentLevel.ToString();

        if (augUpCount <= 4 && UnitManager.Instance.GetPlayer().Stats.CurrentLevel % 10 == 0)
        {   //TODO : 최대 레벨설정
            //증강과 특성 선택하는 메서드
            AugSelections();
        }
        else
        {
            //특성만 선택
            ChangeSelections();
        }

    }
    private void OnDisable()
    {
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.ResumeGame();
            Time.timeScale = 1;
        }
    }
    private void Banish_BTN()
    {
        // Debug.Log("배니쉬");
        SelectionOnOff(false);
        UI_Manager.Instance.panel_Dic["Banish_Panel"].PanelOpen();
    }
    private void Reroll_BTN()
    {
        // Debug.Log("리롤");
        if (DataManager.Instance.BTS.Reroll > 0)
        {
            ChangeSelections();
            DataManager.Instance.BTS.Reroll--;
            reroll_Counter_TMP.text = DataManager.Instance.BTS.Reroll.ToString();
        }
    }
    public void ChangeSelections()
    {
        if (current_Selections[3].gameObject.activeSelf)
            current_Selections[3].gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            current_Selections[i].m_BTN.onClick.RemoveAllListeners();
            current_Selections[i].m_BTN.onClick.AddListener(current_Selections[i].Select_BTN);
            current_Selections[i].gameObject.SetActive(true);
            current_Selections[i].m_selectionBG_IMG.color = Color.white;

        }
        List<Enums.SkillName> popSkill_List = inGameUI_Panel.skillSelector.SelectSkills();
        if (popSkill_List.Count == 2)
        {
            current_Selections[2].gameObject.SetActive(false);

        }
        Skill_Info skill_Info;
        for (int i = 0; i < popSkill_List.Count; i++)
        {
            skill_Info = skill_Info_Dic[popSkill_List[i]];
            current_Selections[i].m_skillName = skill_Info.skill_Name;
            current_Selections[i].display_Name.text = skill_Info.display_Name;
            if (inGameUI_Panel.skillContainer.GetSkill(skill_Info.skill_Name) != Enums.SkillName.None)
            {
                current_Selections[i].info_TMP.text =
                skill_Info.skill_Texts[inGameUI_Panel.skillSelector.SkillLevel(skill_Info.skill_Name)];
            }
            else
            {
                current_Selections[i].info_TMP.text = skill_Info.skill_Texts[0];
            }
            current_Selections[i].icon_IMG.sprite = skill_Info.skill_Sprite;
        }
    }
    public void AugSelections()
    {
        if (isAugSelected == false)
        {
            current_Selections[3].gameObject.SetActive(true);

            if (DataManager.Instance.BTS.DashCount == 0)
                current_Selections[3].m_BTN.interactable = false;
            for (int i = 0; i < 4; i++)
            {
                current_Selections[i].m_BTN.onClick.RemoveAllListeners();
                current_Selections[i].m_BTN.onClick.AddListener(current_Selections[i].Select_BTN2);
                current_Selections[i].m_augType = aug_Infos.aug_Type[i];
                current_Selections[i].display_Name.text = aug_Infos.aug_Name[i];
                current_Selections[i].info_TMP.text = aug_Infos.aug_Text[i];
                current_Selections[i].icon_IMG.sprite = aug_Infos.aug_Icon[i];
                current_Selections[i].m_selectionBG_IMG.color = Color.yellow;
            }
        }
        else
        {
            for (int i = 1; i < 4; i++)
            {
                current_Selections[i].gameObject.SetActive(false);
            }
            current_Selections[0].m_BTN.onClick.RemoveAllListeners();
            current_Selections[0].m_BTN.onClick.AddListener(current_Selections[0].Select_BTN2);
            current_Selections[0].m_augType = aug_Infos.aug_Type[0];
            current_Selections[0].display_Name.text = aug_Infos.aug_Name[augUpCount];
            current_Selections[0].info_TMP.text = aug_Infos.aug_Text[augUpCount];
            current_Selections[0].icon_IMG.sprite = aug_Infos.aug_Icon[0];
            current_Selections[0].m_selectionBG_IMG.color = Color.yellow;
        }

    }
    public void SelectionOnOff(bool On)
    {
        foreach (Selection selection in current_Selections)
        {
            selection.m_BTN.interactable = On;
        }
        buttons[0].interactable = On;
        buttons[0].interactable = On;
    }
    public void UpdateBanishCnt()
    {
        banish_Counter_TMP.text = UnitManager.Instance.GetPlayer().Stats.CurrentBanish.ToString();
    }

    public void SetAugTextInit(Enums.AugmentName augmentName)
    {
        if (isAugSelected) return;
        aug_Infos.aug_Type.Clear();
        aug_Infos.aug_Name.Clear();
        aug_Infos.aug_Text.Clear();
        aug_Infos.aug_Icon.Clear();
        switch (augmentName)
        {
            case Enums.AugmentName.TwoHandSword:
                aug_Infos.Two_Hand_Sword();
                break;
            case Enums.AugmentName.BigSword:
                aug_Infos.Big_Sword();
                break;
            case Enums.AugmentName.SwordShield:
                aug_Infos.Sword_Shield();
                break;
            case Enums.AugmentName.Shielder:
                aug_Infos.Shielder();
                break;
            case Enums.AugmentName.LongBow:
                aug_Infos.Long_Bow();
                break;
            case Enums.AugmentName.CrossBow:
                aug_Infos.Cross_Bow();
                break;
            case Enums.AugmentName.GreatBow:
                aug_Infos.Great_Bow();
                break;
            case Enums.AugmentName.ArcRanger:
                aug_Infos.Arc_Ranger();
                break;
            case Enums.AugmentName.Staff:
                aug_Infos.Staff();
                break;
            case Enums.AugmentName.Wand:
                aug_Infos.Wand();
                break;
            case Enums.AugmentName.Orb:
                aug_Infos.Orb();
                break;
            case Enums.AugmentName.Warlock:
                aug_Infos.Warlock();
                break;
        }
        isAugSelected = true;
        aug_Infos.selectedTime = TimeManager.Instance.gameTime;
    }

    public string FindSkillName(Enums.SkillName skillName)
    {
        return skill_Info_Dic[skillName].display_Name;
    }
    public Sprite FindSkillIcon(Enums.SkillName skillName)
    {
        return skill_Info_Dic[skillName].skill_Sprite;
    }
}
