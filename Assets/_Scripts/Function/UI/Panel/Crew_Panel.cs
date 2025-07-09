using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crew_Panel : Panel
{
    private void Awake()
    {
        buttons[0].onClick.AddListener(Return_BTN);
    }

    private void Return_BTN()
    {
        UI_Manager.Instance.panel_Dic["Main_Panel"].PanelOpen();
        PanelClose(true);
    }
}
