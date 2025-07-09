using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Enums;
[Serializable]
public class ClassInfo : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TextMeshProUGUI className_TMP;
    [SerializeField]
    private Image protrait_IMG;
    [SerializeField]
    private Image weaponIcon_IMG;
    [SerializeField]
    private ClassSelect_Panel classSelect_Panel;

    public string m_className;
    public Sprite m_Portrait;
    public Sprite m_WeaponIcon;
    public Button m_BTN;
    public Outline m_Outline;
    public bool isUnlocked;
    public int requireRemnents;
    public Action<int> selectAction;
    private void Awake()
    {
        if (classSelect_Panel == null) classSelect_Panel = GetComponentInParent<ClassSelect_Panel>();
        if (m_BTN == null) m_BTN = GetComponent<Button>();
        if (m_Outline == null) m_Outline = GetComponent<Outline>();
        m_BTN.onClick.AddListener(ClassSelect);
        //TODO : 저장된 m_level변수로 레벨 불러오기
    }
    private void Start()
    {
        className_TMP.text = m_className;
        protrait_IMG.sprite = m_Portrait;
        weaponIcon_IMG.sprite = m_WeaponIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UnlockClass();
            if (isUnlocked == false) return;
            if (m_BTN.interactable) return;
            m_BTN.interactable = true;
            if (classSelect_Panel.selection_Queue.Count > 0)
            {
                ClassInfo classInfo = classSelect_Panel.selection_Queue.Dequeue();
                classInfo.m_BTN.interactable = false;
            }
            classSelect_Panel.selection_Queue.Enqueue(this);
        }
    }

    private void UnlockClass()
    {
        if (isUnlocked) return;
        if (DataManager.Instance.player_Property.remnants_Point - requireRemnents >= 0)
        {
            DataManager.Instance.player_Property.remnants_Point -= requireRemnents;

            classSelect_Panel.remnents_TMP.text =
            DataManager.Instance.player_Property.remnants_Point.ToString();

            isUnlocked = true;

            for (int i = 0; i < classSelect_Panel.classInfos.Count; i++)
            {
                if (classSelect_Panel.classInfos[i] == this)
                {
                    DataManager.Instance.player_Property.class_Unlocked[i] = true;
                }
            }
        }
    }

    private void ClassSelect()
    {
        DataManager.Instance.SaveData();
        for (int i = 0; i < classSelect_Panel.classInfos.Count; i++)
        {
            if (classSelect_Panel.classInfos[i] == this)
            {
                DataManager.Instance.classSelect_Type = (ClassType)i;
                break;
            }
        }
        DataManager.Instance.inGameValue.playerIcon = m_Portrait;
        UI_Manager.Instance.panel_Dic["ClassSelect_Panel"].PanelClose(true);
        GameSceneManager.SceneLoad("Game");
    }
}
