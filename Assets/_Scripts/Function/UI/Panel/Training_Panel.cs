using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Training_Category
{
    MaxHp,
    HpRegen,
    Defense,
    Mspd,
    ATK,
    Aspd,
    CriRate,
    CriDamage,
    ProjAmount,
    ATKRange,
    Duration,
    Cooldown,
    Revival,
    Magnet,
    Growth,
    Greed,
    Curse,
    Reroll,
    Banish
}

public class Training_Panel : Panel, IPointerExitHandler
{
    [SerializeField] private Upgrade_Panel upgrade_Panel;
    public Image info_IMG;
    public TextMeshProUGUI info_Text;
    public RectTransform trainingInfo;
    public Queue<TrainingCell> ClickedCell_Queue = new();
    public Sprite tinyCellOn_Sprite;
    public Sprite tinyCellOff_Sprite;
    [SerializeField]
    Training training;
    public CellReset cellReset;
    public List<TrainingCell> trainingCells_List;
    public TextMeshProUGUI requireGold_TMP;
    private void Start()
    {
        ByCellCategory();

    }

    private void OnEnable()
    {
        Cell_Initialize(ref trainingCells_List);
        foreach (TrainingCell cell in trainingCells_List)
        {
            cell.baseCellAction += upgrade_Panel.DisplayGold;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        trainingInfo.gameObject.SetActive(false);
    }

    private void Cell_Initialize(ref List<TrainingCell> cells)
    {
        foreach (TrainingCell cell in cells)
        {
            switch (cell.training_Category)
            {
                case Training_Category.MaxHp:
                    cell.method_Action += training.MaxHp_Modify;
                    break;
                case Training_Category.HpRegen:
                    cell.method_Action += training.HpRegen_Modify;
                    break;
                case Training_Category.Defense:
                    cell.method_Action += training.Defense_Modify;
                    break;
                case Training_Category.Mspd:
                    cell.method_Action += training.Mspd_Modify;
                    break;
                case Training_Category.ATK:
                    cell.method_Action += training.ATK_Modify;
                    break;
                case Training_Category.Aspd:
                    cell.method_Action += training.Aspd_Modify;
                    break;
                case Training_Category.CriRate:
                    cell.method_Action += training.CriRate_Modify;
                    break;
                case Training_Category.CriDamage:
                    cell.method_Action += training.CriDamage_Modify;
                    break;
                case Training_Category.ProjAmount:
                    cell.method_Action += training.ProjAmount_Modify;
                    break;
                case Training_Category.ATKRange:
                    cell.method_Action += training.ATKRange_Modify;
                    break;
                case Training_Category.Duration:
                    cell.method_Action += training.Duration_Modify;
                    break;
                case Training_Category.Cooldown:
                    cell.method_Action += training.Cooldown_Modify;
                    break;
                case Training_Category.Revival:
                    cell.method_Action += training.Revival_Modify;
                    break;
                case Training_Category.Magnet:
                    cell.method_Action += training.Magnet_Modify;
                    break;
                case Training_Category.Growth:
                    cell.method_Action += training.Growth_Modify;
                    break;
                case Training_Category.Greed:
                    cell.method_Action += training.Greed_Modify;
                    break;
                case Training_Category.Curse:
                    cell.method_Action += training.Curse_Modify;
                    break;
                case Training_Category.Reroll:
                    cell.method_Action += training.Reroll_Modify;
                    break;
                case Training_Category.Banish:
                    cell.method_Action += training.Banish_Modify;
                    break;
                default:
                    // Debug.Log("액션구독문제");
                    break;

            }

        }
    }
    private void OnDisable()
    {
        foreach (TrainingCell cell in trainingCells_List)
        {
            cell.baseCellAction -= upgrade_Panel.DisplayGold;
            switch (cell.training_Category)
            {
                case Training_Category.MaxHp:
                    cell.method_Action -= training.MaxHp_Modify;
                    break;
                case Training_Category.HpRegen:
                    cell.method_Action -= training.HpRegen_Modify;
                    break;
                case Training_Category.Defense:
                    cell.method_Action -= training.Defense_Modify;
                    break;
                case Training_Category.Mspd:
                    cell.method_Action -= training.Mspd_Modify;
                    break;
                case Training_Category.ATK:
                    cell.method_Action -= training.ATK_Modify;
                    break;
                case Training_Category.Aspd:
                    cell.method_Action -= training.Aspd_Modify;
                    break;
                case Training_Category.CriRate:
                    cell.method_Action -= training.CriRate_Modify;
                    break;
                case Training_Category.CriDamage:
                    cell.method_Action -= training.CriDamage_Modify;
                    break;
                case Training_Category.ProjAmount:
                    cell.method_Action -= training.ProjAmount_Modify;
                    break;
                case Training_Category.ATKRange:
                    cell.method_Action -= training.ATKRange_Modify;
                    break;
                case Training_Category.Duration:
                    cell.method_Action -= training.Duration_Modify;
                    break;
                case Training_Category.Cooldown:
                    cell.method_Action -= training.Cooldown_Modify;
                    break;
                case Training_Category.Revival:
                    cell.method_Action -= training.Revival_Modify;
                    break;
                case Training_Category.Magnet:
                    cell.method_Action -= training.Magnet_Modify;
                    break;
                case Training_Category.Growth:
                    cell.method_Action -= training.Growth_Modify;
                    break;
                case Training_Category.Greed:
                    cell.method_Action -= training.Greed_Modify;
                    break;
                case Training_Category.Curse:
                    cell.method_Action -= training.Curse_Modify;
                    break;
                case Training_Category.Reroll:
                    cell.method_Action -= training.Reroll_Modify;
                    break;
                case Training_Category.Banish:
                    cell.method_Action -= training.Banish_Modify;
                    break;
                default:
                    // Debug.Log("액션구독해제문제");
                    break;
            }
        }
    }

    private void ByCellCategory()
    {
        foreach (TrainingCell cell in trainingCells_List)
        {
            switch (cell.training_Category)
            {
                case Training_Category.MaxHp:
                    cell.trainingCount = DataManager.Instance.player_Property.MaxHp_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.HpRegen:
                    cell.trainingCount = DataManager.Instance.player_Property.HpRegen_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Defense:
                    cell.trainingCount = DataManager.Instance.player_Property.Defense_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Mspd:
                    cell.trainingCount = DataManager.Instance.player_Property.Mspd_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.ATK:
                    cell.trainingCount = DataManager.Instance.player_Property.ATK_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Aspd:
                    cell.trainingCount = DataManager.Instance.player_Property.Aspd_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.CriRate:
                    cell.trainingCount = DataManager.Instance.player_Property.CriRate_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.CriDamage:
                    cell.trainingCount = DataManager.Instance.player_Property.CriDamage_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.ProjAmount:
                    cell.trainingCount = DataManager.Instance.player_Property.ProjAmount_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.ATKRange:
                    cell.trainingCount = DataManager.Instance.player_Property.ATKRange_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Duration:
                    cell.trainingCount = DataManager.Instance.player_Property.Duration_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Cooldown:
                    cell.trainingCount = DataManager.Instance.player_Property.Cooldown_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Revival:
                    cell.trainingCount = DataManager.Instance.player_Property.Revival_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Magnet:
                    cell.trainingCount = DataManager.Instance.player_Property.Magnet_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Growth:
                    cell.trainingCount = DataManager.Instance.player_Property.Growth_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Greed:
                    cell.trainingCount = DataManager.Instance.player_Property.Greed_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Curse:
                    cell.trainingCount = DataManager.Instance.player_Property.Curse_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Reroll:
                    cell.trainingCount = DataManager.Instance.player_Property.Reroll_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                case Training_Category.Banish:
                    cell.trainingCount = DataManager.Instance.player_Property.Banish_TrainingCount;
                    cell.ByTrainingCount();
                    break;
                default:
                    // Debug.Log("셀 카테고리 문제");
                    break;
            }
        }
    }

}
