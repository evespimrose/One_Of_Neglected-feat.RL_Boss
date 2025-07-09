using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void NodeReset();
public delegate void CellReset();

public class Upgrade_Panel : Panel
{
    public TextMeshProUGUI upgradePanel_TMP;
    public Toggle bless_Toggle;
    public Toggle training_Toggle;
    public List<GameObject> helpPopup_BlessElements;
    public List<GameObject> helpPopup_TrainingElements;
    [Header("가호 확대 범위")]
    public RectTransform bless_Rect;
    public float MinMagnitude;
    public float MaxMagnitude;
    [Header("가호/단련 포인트 표기")]
    public Image point_Image;
    public TextMeshProUGUI point_Text;

    public Bless_Panel bless_Panel;
    public Training_Panel training_Panel;

    private void Awake()
    {
        buttons[0].onClick.AddListener(BlessReset_BTN);
        buttons[1].onClick.AddListener(Return_BTN);
        bless_Toggle.onValueChanged.AddListener(ToggleEvents);
        bless_Toggle.interactable = false;
        training_Toggle.onValueChanged.AddListener(ToggleEvents);
        point_Text.text = DataManager.Instance.player_Property.bless_Point.ToString();
    }
    private void Update()
    {
        WheelAction();
    }

    private void WheelAction()
    {
        if (UI_Manager.Instance.panel_Dic["Bless_Panel"].gameObject.activeSelf)
        {
            float zoomAmount = Input.GetAxis("Mouse ScrollWheel") * 1f;

            Vector3 scale = new Vector3(bless_Rect.localScale.x + zoomAmount,
                                        bless_Rect.localScale.y + zoomAmount,
                                        bless_Rect.localScale.z + zoomAmount);

            if (scale.magnitude > MinMagnitude && scale.magnitude < MaxMagnitude)
            {
                bless_Rect.localScale = scale;
            }

        }
    }

    //토글 제어 메서드
    private void ToggleEvents(bool arg0)
    {
        if (training_Toggle.isOn)
        {
            upgradePanel_TMP.text = "단련";
            HelpElemets(helpPopup_TrainingElements, helpPopup_BlessElements);
            point_Image.sprite = Resources.Load<Sprite>("Using/UI/Coin");

            DisplayGold();

            bless_Toggle.interactable = true;
            training_Toggle.interactable = false;

            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(TrainingReset_BTN);

            UI_Manager.Instance.panel_Dic["Bless_Panel"].PanelClose(false);
            UI_Manager.Instance.panel_Dic["Training_Panel"].PanelOpen();
        }
        else
        {
            upgradePanel_TMP.text = "가호";
            HelpElemets(helpPopup_BlessElements, helpPopup_TrainingElements);
            point_Image.sprite = Resources.Load<Sprite>("Using/UI/Bless");

            DisplayBlessPoint();

            bless_Toggle.interactable = false;
            training_Toggle.interactable = true;

            buttons[0].onClick.RemoveAllListeners();
            buttons[0].onClick.AddListener(BlessReset_BTN);

            UI_Manager.Instance.panel_Dic["Training_Panel"].PanelClose(false);
            UI_Manager.Instance.panel_Dic["Bless_Panel"].PanelOpen();
        }
    }

    private void BlessReset_BTN()
    {
        bless_Panel.nodeReset?.Invoke();
    }
    private void TrainingReset_BTN()
    {
        training_Panel.cellReset?.Invoke();
    }
    //메인 패널로 돌아가는 메서드
    private void Return_BTN()
    {
        UI_Manager.Instance.panel_Dic["Main_Panel"].PanelOpen();
        bless_Toggle.isOn = true;
        PanelClose(true);
    }

    private void HelpElemets(List<GameObject> enableElements, List<GameObject> disableElements)
    {
        foreach (GameObject obj in enableElements)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in disableElements)
        {
            obj.SetActive(false);
        }
    }

    public void DisplayGold()
    {
        point_Text.text = DataManager.Instance.player_Property.gold.ToString();
    }
    public void DisplayBlessPoint()
    {
        point_Text.text = DataManager.Instance.player_Property.bless_Point.ToString();
    }
}
