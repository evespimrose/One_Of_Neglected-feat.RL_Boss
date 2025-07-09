using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassSelect_Panel : Panel
{
    public Queue<ClassInfo> selection_Queue = new();
    public TextMeshProUGUI remnents_TMP;
    public List<ClassInfo> classInfos;
    public Button return_BTN;

    private void Awake()
    {
        return_BTN.onClick.AddListener(Return_BTN);
    }
    private void Start()
    {

        if (DataManager.Instance.player_Property.class_Unlocked.Count > 0)
        {
            for (int i = 0; i < classInfos.Count; i++)
            {
                classInfos[i].isUnlocked =
                DataManager.Instance.player_Property.class_Unlocked[i];
            }
        }

    }
    private void OnEnable()
    {
        remnents_TMP.text = DataManager.Instance.player_Property.remnants_Point.ToString();
    }
    private void Return_BTN()
    {
        UI_Manager.Instance.panel_Dic["Main_Panel"].PanelOpen();
        PanelClose(true);
    }

}
