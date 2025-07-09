using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Option_Panel : Panel
{
    [SerializeField] private CanvasScaler m_CanvasScaler;
    public TMP_Dropdown resolutionDropdown;
    public Slider sounds_Slider;
    public Slider effects_Slider;
    private Resolution[] tempRes;
    private List<Resolution> resolutions = new List<Resolution>();

    private void Awake()
    {
        buttons[0].onClick.AddListener(ReturnMainPanel);
        buttons[1].onClick.AddListener(ReturnTitle_BTN);

        sounds_Slider.onValueChanged.AddListener(OnSoundSliderValueChanged);
    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            buttons[1].gameObject.SetActive(true);
        }
        else
        {
            buttons[1].gameObject.SetActive(false);
        }

        sounds_Slider.value = SoundManager.Instance.getMasterVolume();
    }
    public void OnStart()
    {
        tempRes = Screen.resolutions;

        foreach (Resolution resolution in tempRes)
        {
            var MAX = GCD(resolution.width, resolution.height);
            if ((resolution.width / MAX == 16) && (resolution.height / MAX == 9))
            {
                if (resolution.refreshRateRatio.value < 60) continue;
                resolutions.Add(new Resolution { width = resolution.width, height = resolution.height, refreshRateRatio = resolution.refreshRateRatio });
            }
        }
        if (resolutions.Count == 0)
        {
            Resolution currentRes = Screen.currentResolution;
            resolutions.Add(new Resolution { width = currentRes.width, height = currentRes.height, refreshRateRatio = currentRes.refreshRateRatio });
        }
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " / " + Mathf.Round((float)resolutions[i].refreshRateRatio.value) + "Hz";
            options.Add(option);
        }
        options.Add("전체화면(창) *");
        options.Add("전체화면 *");
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = options.Count - 1;
        resolutionDropdown.RefreshShownValue();

        // 게임이 가장 적합한 해상도로 시작되도록 설정합니다.
        SetResolution(options.Count - 1);
    }

    public void SetResolution(int resolutionIdx)
    {
        if (resolutionIdx > resolutions.Count - 1)
        {
            if (resolutionIdx == resolutions.Count)
            {
                Screen.SetResolution(resolutions[resolutions.Count - 1].width, resolutions[resolutions.Count - 1].height, FullScreenMode.FullScreenWindow);
            }
            else
            {
                Screen.SetResolution(resolutions[resolutions.Count - 1].width, resolutions[resolutions.Count - 1].height, FullScreenMode.ExclusiveFullScreen);
            }
            UI_Manager.Instance.SetCanvasScaler();
            m_CanvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
            return;
        }
        Resolution resolution = resolutions[resolutionIdx];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);
        UI_Manager.Instance.SetCanvasScaler();
        m_CanvasScaler.referenceResolution = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
    }

    private void ReturnMainPanel()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            PanelClose(true);
            UI_Manager.Instance.panel_Dic["Main_Panel"].PanelOpen();
        }
        else
        {
            PanelClose(true);
            Time.timeScale = 1;
            GameManager.Instance.isPaused = false;
        }
    }

    private void ReturnTitle_BTN()
    {
        Time.timeScale = 1;
        UnitManager.Instance.GetPlayer().GetComponent<AugmentSelector>().RemoveAllAugments();
        Destroy(GameManager.Instance.gameObject);
        Destroy(UnitManager.Instance.gameObject);
        Destroy(ProjectileManager.Instance.gameObject);
        Destroy(TimeManager.Instance.gameObject);
        Destroy(SoundManager.Instance.gameObject);
        GameSceneManager.SceneLoad("Title");
        PanelClose(true);
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private void OnSoundSliderValueChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value * 100);
    }
}