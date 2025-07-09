using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bless : MonoBehaviour
{
    #region 공격가호
    public void ATK_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.ATK += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.ATK -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void ProjAmount_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.ProjAmount += 1;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.ProjAmount -= 1;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void ASPD_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Aspd += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Aspd -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void CriDamage_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.CriDamage += 10;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.CriDamage -= 10;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void CriRate_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.CriRate += 10;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.CriRate -= 10;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void ProjDestroy_Modify(bool On)
    {
        DataManager.Instance.BTS.ProjDestroy = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void ProjParry_Modify(bool On)
    {
        DataManager.Instance.BTS.projParry = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void GodKill_Modify(bool On)
    {
        DataManager.Instance.BTS.GodKill = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    #endregion

    #region 방어가호

    public void MaxHP_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.MaxHp += 10;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.MaxHp -= 10;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Defense_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Defense += 2;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Defense -= 2;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void HPRegen_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.HpRegen += 1;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.HpRegen -= 1;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Barrier_Modify(bool On)
    {
        DataManager.Instance.BTS.Barrier = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void BarrierCooldown_Modify(bool On)
    {
        if (On)
        {
            if (DataManager.Instance.BTS.BarrierCooldown == 7)
                DataManager.Instance.BTS.BarrierCooldown = 5;

            if (DataManager.Instance.BTS.BarrierCooldown == 0)
                DataManager.Instance.BTS.BarrierCooldown = 7;
        }
        else
        {
            if (DataManager.Instance.BTS.BarrierCooldown == 7)
                DataManager.Instance.BTS.BarrierCooldown = 0;

            if (DataManager.Instance.BTS.BarrierCooldown == 5)
                DataManager.Instance.BTS.BarrierCooldown = 7;
        }
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Invincibility_Modify(bool On)
    {
        DataManager.Instance.BTS.Invincibility = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Adversary_Modify(bool On)
    {
        DataManager.Instance.BTS.Adversary = On;
        if (On)
        {
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    #endregion

    #region 유틸가호

    public void ATKRange_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.ATKRange += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.ATKRange -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Duration_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Duration += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Duration -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Cooldown_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Cooldown += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Cooldown -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Revival_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Revival += 1;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Revival -= 1;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Magnet_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Magnet += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Magnet -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Growth_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Growth += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Growth -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void Greed_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.Greed += 5;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.Greed -= 5;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    public void DashCount_Modify(bool On)
    {
        if (On)
        {
            DataManager.Instance.BTS.DashCount += 1;
            DataManager.Instance.player_Property.bless_Point--;
        }
        else
        {
            DataManager.Instance.BTS.DashCount -= 1;
            DataManager.Instance.player_Property.bless_Point++;
        }
    }

    #endregion
}