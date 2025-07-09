using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    public Dictionary<string, Panel> panel_Dic = new Dictionary<string, Panel>();
    public List<Panel> panel_List;
    public Option_Panel option_Panel;
    public float sounds_Value;
    public float effects_Value;
    [SerializeField] private TitleCanvas m_Title_Canvas;
    [SerializeField] private InGameCanvas m_InGame_Canvas;
    [SerializeField] private LoadingCanvas m_Loading_Canvas;
    [SerializeField] private CanvasScaler m_CanvasScaler;
    protected override void Awake()
    {
        base.Awake();
        if (m_CanvasScaler == null) m_CanvasScaler = GetComponent<CanvasScaler>();
        SceneManager.sceneLoaded += (x, y) =>
        {
            if (x.name == "Title")
            {
                panel_List.Clear();
                panel_Dic.Clear();

                m_Title_Canvas = FindAnyObjectByType<TitleCanvas>();
                m_Title_Canvas.m_CanvasScaler.referenceResolution =
                 new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

                panel_List = m_Title_Canvas.titleCanvasPanels;

                panel_List.Add(option_Panel);
                foreach (Panel panel in panel_List)
                {
                    panel_Dic.Add(panel.gameObject.name, panel);
                }

            }
            if (x.name == "Game")
            {
                panel_List.Clear();
                panel_Dic.Clear();

                m_InGame_Canvas = FindAnyObjectByType<InGameCanvas>();
                m_InGame_Canvas.m_CanvasScaler.referenceResolution =
                new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);

                panel_List = m_InGame_Canvas.inGameCanvasPanels;
                panel_List.Add(option_Panel);
                foreach (Panel panel in panel_List)
                {
                    panel_Dic.Add(panel.gameObject.name, panel);
                }
            }
            if (x.name == "LoadingScene")
            {
                m_Loading_Canvas = FindAnyObjectByType<LoadingCanvas>();
                m_Loading_Canvas.m_CanvasScaler.referenceResolution =
                new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            }
        };

    }

    private void Start()
    {
        panel_Dic["Option_Panel"].GetComponent<Option_Panel>().OnStart();

    }
    public void SetCanvasScaler()
    {
        if (m_Title_Canvas != null)
        {
            m_Title_Canvas.m_CanvasScaler.referenceResolution =
            new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
        if (m_InGame_Canvas != null)
        {
            m_InGame_Canvas.m_CanvasScaler.referenceResolution =
            new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }
        m_CanvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }
}
