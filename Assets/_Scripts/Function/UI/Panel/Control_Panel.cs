using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Control_Panel : Panel
{
    private void Awake()
    {
        buttons[0].onClick.AddListener(CtrlBTNClick);
    }

    private void CtrlBTNClick()
    {
        PanelClose(true);
        UI_Manager.Instance.panel_Dic["Main_Panel"].PanelOpen();
    }
}
