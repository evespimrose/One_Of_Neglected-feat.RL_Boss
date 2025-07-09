using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training : MonoBehaviour
{
    public void MaxHp_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.MaxHp += 10;
        else DataManager.Instance.BTS.MaxHp -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.MaxHp_TrainingCount = trainingCount;
    }
    public void HpRegen_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.HpRegen += 0.5f;
        else DataManager.Instance.BTS.HpRegen -= 0.5f;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.HpRegen_TrainingCount = trainingCount;
    }
    public void Defense_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Defense += 1;
        else DataManager.Instance.BTS.Defense -= 1;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Defense_TrainingCount = trainingCount;
    }
    public void Mspd_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Mspd += 5;
        else DataManager.Instance.BTS.Mspd -= 5;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Mspd_TrainingCount = trainingCount;
    }
    public void ATK_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.ATK += 10;
        else DataManager.Instance.BTS.ATK -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.ATK_TrainingCount = trainingCount;
    }
    public void Aspd_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Aspd += 3;
        else DataManager.Instance.BTS.Aspd -= 3;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Aspd_TrainingCount = trainingCount;
    }
    public void CriRate_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.CriRate += 10;
        else DataManager.Instance.BTS.CriRate -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.CriRate_TrainingCount = trainingCount;
    }
    public void CriDamage_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.CriDamage += 10;
        else DataManager.Instance.BTS.CriDamage -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.CriDamage_TrainingCount = trainingCount;
    }
    public void ProjAmount_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.ProjAmount += 1;
        else DataManager.Instance.BTS.ProjAmount -= 1;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.ProjAmount_TrainingCount = trainingCount;
    }
    public void ATKRange_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.ATKRange += 5;
        else DataManager.Instance.BTS.ATKRange -= 5;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.ATKRange_TrainingCount = trainingCount;
    }
    public void Duration_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Duration += 5;
        else DataManager.Instance.BTS.Duration -= 5;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Duration_TrainingCount = trainingCount;
    }
    public void Cooldown_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Cooldown += 4;
        else DataManager.Instance.BTS.Cooldown -= 4;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Cooldown_TrainingCount = trainingCount;
    }
    public void Revival_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Revival += 1;
        else DataManager.Instance.BTS.Revival -= 1;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Revival_TrainingCount = trainingCount;
    }
    public void Magnet_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Magnet += 5;
        else DataManager.Instance.BTS.Magnet -= 5;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Magnet_TrainingCount = trainingCount;
    }
    public void Growth_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Growth += 10;
        else DataManager.Instance.BTS.Growth -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Growth_TrainingCount = trainingCount;
    }
    public void Greed_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Greed += 10;
        else DataManager.Instance.BTS.Greed -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Greed_TrainingCount = trainingCount;
    }
    public void Curse_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Curse += 10;
        else DataManager.Instance.BTS.Curse -= 10;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Curse_TrainingCount = trainingCount;
    }
    public void Reroll_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Reroll += 1;
        else DataManager.Instance.BTS.Reroll -= 1;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Reroll_TrainingCount = trainingCount;
    }
    public void Banish_Modify(bool On, int requireGold, int trainingCount)
    {
        if (On) DataManager.Instance.BTS.Banish += 1;
        else DataManager.Instance.BTS.Banish -= 1;
        DataManager.Instance.player_Property.gold += requireGold;
        DataManager.Instance.player_Property.Banish_TrainingCount = trainingCount;
    }

    public void OnResetBTNClick()
    {
        DataManager.Instance.player_Property.MaxHp_TrainingCount = 0;
        DataManager.Instance.player_Property.HpRegen_TrainingCount = 0;
        DataManager.Instance.player_Property.Defense_TrainingCount = 0;
        DataManager.Instance.player_Property.Mspd_TrainingCount = 0;
        DataManager.Instance.player_Property.ATK_TrainingCount = 0;
        DataManager.Instance.player_Property.Aspd_TrainingCount = 0;
        DataManager.Instance.player_Property.CriRate_TrainingCount = 0;
        DataManager.Instance.player_Property.CriDamage_TrainingCount = 0;
        DataManager.Instance.player_Property.ProjAmount_TrainingCount = 0;
        DataManager.Instance.player_Property.ATKRange_TrainingCount = 0;
        DataManager.Instance.player_Property.Duration_TrainingCount = 0;
        DataManager.Instance.player_Property.Cooldown_TrainingCount = 0;
        DataManager.Instance.player_Property.Revival_TrainingCount = 0;
        DataManager.Instance.player_Property.Magnet_TrainingCount = 0;
        DataManager.Instance.player_Property.Growth_TrainingCount = 0;
        DataManager.Instance.player_Property.Greed_TrainingCount = 0;
        DataManager.Instance.player_Property.Curse_TrainingCount = 0;
        DataManager.Instance.player_Property.Reroll_TrainingCount = 0;
        DataManager.Instance.player_Property.Banish_TrainingCount = 0;
    }
}
