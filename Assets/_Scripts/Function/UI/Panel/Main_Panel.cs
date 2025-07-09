using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Panel : Panel
{
    private void Awake()
    {
        buttons[0].onClick.AddListener(Start_BTN);
        buttons[1].onClick.AddListener(Upgrade_BTN);
        buttons[2].onClick.AddListener(Control_BTN);
        buttons[3].onClick.AddListener(Option_Panel);
        buttons[4].onClick.AddListener(Exit_BTN);
        buttons[5].onClick.AddListener(Crew_BTN);
        buttons[6].onClick.AddListener(Story_BTN);
    }
    private void Start()
    {
        SoundManager.Instance.Play("Title_Loopable", SoundManager.Sound.Bgm);
        SoundManager.Instance.SetVolume(SoundManager.Sound.Bgm, 0.5f);

    }
    //게임 시작
    private void Start_BTN()
    {
        UI_Manager.Instance.panel_Dic["ClassSelect_Panel"].PanelOpen();
        PanelClose(true);
        // GameSceneManager.SceneLoad("Game");
    }

    //업그레이드 패널
    private void Upgrade_BTN()
    {
        UI_Manager.Instance.panel_Dic["Upgrade_Panel"].PanelOpen();
        PanelClose(true);
    }

    //조작방법 패널
    private void Control_BTN()
    {
        UI_Manager.Instance.panel_Dic["Control_Panel"].PanelOpen();
        PanelClose(true);
    }

    //옵션 패널
    private void Option_Panel()
    {
        UI_Manager.Instance.panel_Dic["Option_Panel"].PanelOpen();
        PanelClose(true);
    }

    //게임 종료
    private void Exit_BTN()
    {
        DataManager.Instance.SaveData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
    //제작진
    private void Crew_BTN()
    {
        UI_Manager.Instance.panel_Dic["Crew_Panel"].PanelOpen();
        PanelClose(true);
    }
    //스토리
    private void Story_BTN()
    {
        UI_Manager.Instance.panel_Dic["Story_Panel"].PanelOpen();
        PanelClose(true);
    }

}
