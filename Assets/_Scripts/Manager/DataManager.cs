using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using DG.Tweening.Plugins;
using DG.Tweening.Core.Easing;
using JetBrains.Annotations;
using static Enums;
using UnityEngine.SceneManagement;

[Serializable]
public class DicDataTable
{
    public Dictionary<int, bool> bless_Table = new Dictionary<int, bool>();
}

[Serializable]
public class PlayerProperty
{
    public int gold;
    public int bless_Point;
    public int remnants_Point;
    public int causalityLv;
    public float causalityEXP;
    public List<bool> class_Unlocked =
    new List<bool>() { true, false, false };
    public int MaxHp_TrainingCount;
    public int HpRegen_TrainingCount;
    public int Defense_TrainingCount;
    public int Mspd_TrainingCount;
    public int ATK_TrainingCount;
    public int Aspd_TrainingCount;
    public int CriRate_TrainingCount;
    public int CriDamage_TrainingCount;
    public int ProjAmount_TrainingCount;
    public int ATKRange_TrainingCount;
    public int Duration_TrainingCount;
    public int Cooldown_TrainingCount;
    public int Revival_TrainingCount;
    public int Magnet_TrainingCount;
    public int Growth_TrainingCount;
    public int Greed_TrainingCount;
    public int Curse_TrainingCount;
    public int Reroll_TrainingCount;
    public int Banish_TrainingCount;
}

[Serializable]
public class BTS
{
    public int MaxHp;
    public float HpRegen;
    public int Defense;
    public float Mspd;
    public float ATK;
    public float Aspd;
    public float CriRate;
    public float CriDamage;
    public int ProjAmount;
    public float ATKRange;
    public float Duration;
    public float Cooldown;
    public int Revival;
    public float Magnet;
    public float Growth;
    public float Greed;
    public float Curse;
    public int Reroll;
    public int Banish;
    public float BarrierCooldown;
    public int DashCount;
    public bool Barrier;
    public bool ProjDestroy;
    public bool projParry;
    public bool Invincibility;
    public bool Adversary;
    public bool GodKill;
}

[Serializable]
public class DamageStats
{
    public float totalDamage;

    // 스킬별 데미지
    public Dictionary<Enums.SkillName, float> skillDamages = new Dictionary<Enums.SkillName, float>();
    // 증강별 데미지
    public Dictionary<Enums.AugmentName, float> augmentDamages = new Dictionary<Enums.AugmentName, float>();

    [Serializable]
    public class DamageRecord
    {
        public string name;
        public float damage;
    }

    public List<DamageRecord> skillDamageList = new List<DamageRecord>();
    public List<DamageRecord> augmentDamageList = new List<DamageRecord>();

    public void UpdateLists()
    {
        // 스킬 데미지 리스트 업데이트
        skillDamageList.Clear();
        foreach (var kvp in skillDamages)
        {
            skillDamageList.Add(new DamageRecord
            {
                name = kvp.Key.ToString(),
                damage = kvp.Value
            });
        }

        // 증강 데미지 리스트 업데이트
        augmentDamageList.Clear();
        foreach (var kvp in augmentDamages)
        {
            augmentDamageList.Add(new DamageRecord
            {
                name = kvp.Key.ToString(),
                damage = kvp.Value
            });
        }
    }
}
[Serializable]
public struct InGameValue
{
    public int killCount;
    public int gold;
    public int remnents;
    public Sprite playerIcon;
}

public class DataManager : Singleton<DataManager>
{

    public DicDataTable dicDataTable = new DicDataTable();
    public Dictionary<int, bool> bless_Dic = new Dictionary<int, bool>();
    public PlayerProperty player_Property = new PlayerProperty();
    public BTS BTS = new BTS();
#if UNITY_EDITOR
    string path = "Assets/Resources/SaveFile/";
#else
string path = Application.persistentDataPath + "/Save";  
#endif
    public ClassType classSelect_Type = Enums.ClassType.None;

    public DamageStats currentDamageStats = new DamageStats();
    public InGameValue inGameValue;
    public delegate void KillCountHandler(int killCount);
    public event KillCountHandler OnKillCountReached;

    protected override void Awake()
    {
        base.Awake();
        string fromJsonData;

        fromJsonData = Load_JsonUtility<PlayerProperty>("PlayerProperty", player_Property);

        player_Property = JsonUtility.FromJson<PlayerProperty>(fromJsonData);

        fromJsonData = Load_JsonUtility<BTS>("BTS", BTS);
        BTS = JsonUtility.FromJson<BTS>(fromJsonData);

        LoadBlessData();

        SceneManager.sceneLoaded += (x, y) =>
        {
            if (x.name == "Title")
            {
                player_Property.gold += inGameValue.gold;
                player_Property.remnants_Point += inGameValue.remnents;
                CausalityCalc();

                ResetInGameInfo();
            }
        };
    }
    private void CausalityTable(float causalityEXP)
    {

    }
    public void ResetInGameInfo()
    {
        inGameValue.gold = 0;
        inGameValue.remnents = 0;
        inGameValue.killCount = 0;
        inGameValue.playerIcon = null;
        currentDamageStats.skillDamages.Clear();
        currentDamageStats.augmentDamages.Clear();
    }

    public void SaveData()
    {

        dicDataTable.bless_Table = bless_Dic;
        string Data = DictionaryJsonUtility.ToJson(dicDataTable.bless_Table);
        File.WriteAllText(path + "BlessData", Data);
        Save_JsonUtility<PlayerProperty>("PlayerProperty", player_Property, true);
        Save_JsonUtility<BTS>("BTS", BTS, true);

    }
    public void Save_JsonUtility<T>(string fileName, T data, bool pretty = false)
    {
        string Data = JsonUtility.ToJson(data, pretty);
        File.WriteAllText(path + fileName, Data);
    }
    public string Load_JsonUtility<T>(string fileName, T data)
    {
        if (!File.Exists(path + fileName))
        {
            string Data = JsonUtility.ToJson(data);
            File.WriteAllText(path + fileName, Data);
        }
        string fromJsonData = File.ReadAllText(path + fileName);
        return fromJsonData;
    }
    public void LoadBlessData()
    {
        if (!File.Exists(path + "BlessData"))
        {
            dicDataTable.bless_Table = bless_Dic;
            string Data = DictionaryJsonUtility.ToJson(dicDataTable.bless_Table);
            File.WriteAllText(path + "BlessData", Data);
        }

        string fromJsonData_Bless =
        File.ReadAllText(path + "BlessData");
        dicDataTable.bless_Table =
        DictionaryJsonUtility.FromJson<int, bool>(fromJsonData_Bless);
        bless_Dic = dicDataTable.bless_Table;

    }

    public void AddDamageData(float damage, Enums.SkillName skillName)
    {
        currentDamageStats.totalDamage += damage;

        // 스킬 데미지 추적
        if (!currentDamageStats.skillDamages.ContainsKey(skillName))
        {
            currentDamageStats.skillDamages[skillName] = 0;
        }
        currentDamageStats.skillDamages[skillName] += damage;

        currentDamageStats.UpdateLists();
    }

    public void AddDamageData(float damage, Enums.AugmentName augmentName)
    {
        currentDamageStats.totalDamage += damage;

        // 증강 데미지 추적
        if (!currentDamageStats.augmentDamages.ContainsKey(augmentName))
        {
            currentDamageStats.augmentDamages[augmentName] = 0;
        }
        currentDamageStats.augmentDamages[augmentName] += damage;

        currentDamageStats.UpdateLists();
    }
    public void AddKillCount()
    {
        inGameValue.killCount++;

        if (inGameValue.killCount % 1000 == 0)
        {
            OnKillCountReached?.Invoke(inGameValue.killCount);
        }
    }
    private void CausalityCalc()
    {
        if (player_Property.causalityLv == 0) player_Property.causalityLv = 1;
        player_Property.causalityEXP += (float)inGameValue.killCount / 10 * player_Property.causalityLv;
        CausalityTable();
    }
    private void CausalityTable()
    {
        float requireEXP = 100;
        for (int i = 1; i < player_Property.causalityLv; i++)
        {
            requireEXP *= 1.1f;
        }
        while (player_Property.causalityEXP - requireEXP > 0)
        {
            player_Property.causalityEXP -= (int)requireEXP;
            player_Property.causalityLv++;
            player_Property.bless_Point++;
            requireEXP *= 1.1f;
        }
    }
}
