using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TrainingCell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Training_Panel training_Panel;
    private Outline outline;
    [SerializeField]
    private int maxTrainingLevel;
    private Button m_BTN;
    public Image training_IMG;

    public Training_Category training_Category;
    [TextArea]
    public string training_Text;
    public int trainingCount = 0;
    public List<Image> tinyCells;
    public bool isDisplayLv;
    public TextMeshProUGUI lvText;
    public event Action<bool, int, int> method_Action;
    public event Action baseCellAction;
    public List<int> requireGold_List;
    private int currentRequireGold;
    private void Awake()
    {
        training_Panel = GetComponentInParent<Training_Panel>();
        m_BTN = GetComponent<Button>();
        m_BTN.interactable = false;
        outline = GetComponent<Outline>();
        m_BTN.onClick.AddListener(Training);
        RequireGoldCalc(Convert.ToDouble(requireGold_List[0]));
    }
    private void Start()
    {
        if (isDisplayLv)
        {
            if (trainingCount > 10) currentRequireGold = requireGold_List[10];
            else currentRequireGold = requireGold_List[trainingCount];
        }
        training_Panel.requireGold_TMP.text = "필요골드\n" + currentRequireGold.ToString();

    }
    private void OnEnable()
    {
        training_Panel.cellReset += CellReset;
    }
    private void OnDisable()
    {
        training_Panel.cellReset -= CellReset;
    }
    public void CellReset()
    {
        foreach (Image image in tinyCells)
        {
            image.sprite = training_Panel.tinyCellOff_Sprite;
            image.color = Color.white;
        }
        if (isDisplayLv == false)
        {
            for (int i = trainingCount; i > 0; i--)
            {
                method_Action?.Invoke(false, requireGold_List[i - 1], 0);
            }
        }

        if (isDisplayLv)
        {
            if (trainingCount > 9)
            {
                for (int i = trainingCount; i > 10; i--)
                {
                    method_Action?.Invoke(false, requireGold_List[10], 0);
                    // Debug.Log(requireGold_List[10]);
                }
                for (int j = 9; j >= 0; j--)
                {
                    method_Action?.Invoke(false, requireGold_List[j], 0);
                    // Debug.Log(requireGold_List[j]);
                }
            }
            else if (trainingCount <= 9)
            {
                for (int i = trainingCount - 1; i >= 0; i--)
                {
                    method_Action?.Invoke(false, requireGold_List[i], 0);
                    // Debug.Log(requireGold_List[i]);
                }
            }
        }

        trainingCount = 0;
        currentRequireGold = requireGold_List[trainingCount];
        if (isDisplayLv) lvText.text = "Lv." + trainingCount.ToString();
        baseCellAction?.Invoke();
    }

    private void Training()
    {
        if (currentRequireGold == -1) return;
        if (DataManager.Instance.player_Property.gold < currentRequireGold) return;
        if (maxTrainingLevel > trainingCount || isDisplayLv)
        {
            if (isDisplayLv == false && maxTrainingLevel < 10)
                tinyCells[trainingCount].sprite = training_Panel.tinyCellOn_Sprite;
            else if (isDisplayLv == false)
            {
                if (trainingCount < 10)
                {
                    tinyCells[trainingCount].sprite = training_Panel.tinyCellOn_Sprite;
                }
                else
                {
                    tinyCells[trainingCount - 10].color = Color.yellow;
                }
            }
            trainingCount++;
            if (isDisplayLv == false)
            {
                method_Action?.Invoke(true, -requireGold_List[trainingCount - 1], trainingCount);
            }
            else
            {
                if (trainingCount <= 10)
                    method_Action?.Invoke(true, -requireGold_List[trainingCount - 1], trainingCount);
                if (trainingCount > 10)
                    method_Action?.Invoke(true, -requireGold_List[10], trainingCount);
            }
            baseCellAction?.Invoke();
            if (isDisplayLv == false)
            {
                if (trainingCount != maxTrainingLevel)
                    currentRequireGold = requireGold_List[trainingCount];
            }
            else
            {
                if (trainingCount < 10)
                {
                    currentRequireGold = requireGold_List[trainingCount];
                }
                else
                {
                    currentRequireGold = requireGold_List[10];
                }
                lvText.text = "Lv." + trainingCount.ToString();
            }
            training_Panel.requireGold_TMP.text = "필요골드\n" + currentRequireGold.ToString();
            if (maxTrainingLevel == trainingCount)
                training_Panel.requireGold_TMP.text = "Master";
        }
    }
    public void ByTrainingCount()
    {
        if (isDisplayLv)
        {
            lvText.text = "Lv." + trainingCount.ToString();
            if (trainingCount > 10) currentRequireGold = requireGold_List[10];
            else currentRequireGold = requireGold_List[trainingCount];
        }
        else
        {
            for (int i = 0; i < trainingCount; i++)
            {
                if (i < 10)
                    tinyCells[i].sprite = training_Panel.tinyCellOn_Sprite;
                else tinyCells[i - 10].color = Color.yellow;
            }

            if (maxTrainingLevel != trainingCount) currentRequireGold = requireGold_List[trainingCount];
            else currentRequireGold = -1;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (outline.enabled) return;

            outline.enabled = true;
            m_BTN.interactable = true;

            if (training_Panel.ClickedCell_Queue.Count > 0)
            {
                TrainingCell queueCell = training_Panel.ClickedCell_Queue.Dequeue();
                queueCell.outline.enabled = false;
                queueCell.m_BTN.interactable = false;
            }

            training_Panel.ClickedCell_Queue.Enqueue(this);

        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (outline.enabled == false) return;

            if (isDisplayLv == false && trainingCount != 0 && maxTrainingLevel <= 10)
                tinyCells[trainingCount - 1].sprite = training_Panel.tinyCellOff_Sprite;
            else if (isDisplayLv == false)
            {
                if (trainingCount > 10)
                {
                    tinyCells[trainingCount - 11].color = Color.white;
                }
                else
                {
                    if (trainingCount != 0)
                        tinyCells[trainingCount - 1].sprite = training_Panel.tinyCellOff_Sprite;
                }
            }
            if (trainingCount == 0) return;

            trainingCount--;
            if (isDisplayLv == false)
            {
                method_Action?.Invoke(false, requireGold_List[trainingCount], trainingCount);
            }
            else
            {
                if (trainingCount > 9)
                {
                    method_Action?.Invoke(false, requireGold_List[10], trainingCount);
                    // Debug.Log(requireGold_List[10]);
                }
                else
                {
                    method_Action?.Invoke(false, requireGold_List[trainingCount], trainingCount);
                    // Debug.Log(requireGold_List[trainingCount]);
                }
            }
            baseCellAction?.Invoke();
            if (isDisplayLv == false) currentRequireGold = requireGold_List[trainingCount];
            else
            {
                if (trainingCount > 9) currentRequireGold = requireGold_List[10];
                else currentRequireGold = requireGold_List[trainingCount];
            }
            if (isDisplayLv)
            {
                lvText.text = "Lv." + trainingCount.ToString();
                training_Panel.requireGold_TMP.text = "필요골드\n" + currentRequireGold.ToString();
                return;
            }
            training_Panel.requireGold_TMP.text = "필요골드\n" + currentRequireGold.ToString();
        }

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter)
        {
            training_Panel.trainingInfo.gameObject.SetActive(true);
            training_Panel.info_Text.text = training_Text;
            training_Panel.info_IMG.sprite = training_IMG.sprite;
            if (maxTrainingLevel == trainingCount)
            {
                training_Panel.requireGold_TMP.text = "Master";
            }
            else
            {
                training_Panel.requireGold_TMP.text = "필요골드\n" + currentRequireGold.ToString();
            }
        }
    }

    private void RequireGoldCalc(double startGold)
    {
        for (int i = 0; i < requireGold_List.Count; i++)
        {
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    requireGold_List[i] = GetNextRequireGold(ref startGold, 225);
                    break;
                case 2:
                    requireGold_List[i] = GetNextRequireGold(ref startGold, 150);
                    break;
                case 3:
                    requireGold_List[i] = GetNextRequireGold(ref startGold, 125);
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    requireGold_List[i] = GetNextRequireGold(ref startGold, 115);
                    break;
                default:
                    if (isDisplayLv == false) requireGold_List[i] = GetNextRequireGold(ref startGold, 105);
                    else requireGold_List[i] = 2000;
                    break;
            }
        }
    }
    private int GetNextRequireGold(ref double val, double percent)
    {
        val = Math.Round(val * percent / 100, MidpointRounding.AwayFromZero);
        return Convert.ToInt32(val);
    }
}
