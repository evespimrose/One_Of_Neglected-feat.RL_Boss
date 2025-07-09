using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banish_Icon : MonoBehaviour
{
    private InGameUI_Panel inGameUI_Panel;
    private LevelUp_Panel levelUp_Panel;
    private Banish_Panel banish_Panel;
    public Enums.SkillName m_SkillName;
    public Button m_BTN;
    public Image m_Icon;

    private void Awake()
    {
        if (inGameUI_Panel == null) inGameUI_Panel =
        UI_Manager.Instance.panel_Dic["InGameUI_Panel"].GetComponent<InGameUI_Panel>();
        if (levelUp_Panel == null) levelUp_Panel =
        UI_Manager.Instance.panel_Dic["LevelUp_Panel"].GetComponent<LevelUp_Panel>();

        if (banish_Panel == null) banish_Panel =
        UI_Manager.Instance.panel_Dic["Banish_Panel"].GetComponent<Banish_Panel>();

        if (m_BTN == null) m_BTN = GetComponent<Button>();
        m_BTN.onClick.AddListener(RemoveSkill);
    }

    private void RemoveSkill()
    {
        if (UnitManager.Instance.GetPlayer().Stats.CurrentBanish == 0) return;
        UnitManager.Instance.GetPlayer().ModifyStat(Enums.StatType.Banish, -1);
        levelUp_Panel.UpdateBanishCnt();

        inGameUI_Panel.skillSelector.DeductSkill(m_SkillName);
        //기본 비활성화
        m_Icon.color = Color.clear;
        m_BTN.interactable = false;
        //셀 재배치 메서드
        inGameUI_Panel.Remove_MiniIcon(m_SkillName, m_Icon.sprite);
        banish_Panel.Remove_BanishIcon(this);
        //셀 재배치 완료 후 Enums 초기화
        m_SkillName = Enums.SkillName.None;

    }
}
