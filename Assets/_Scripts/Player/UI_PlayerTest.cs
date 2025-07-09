using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class UI_PlayerTest : MonoBehaviour
{
    [SerializeField] private Player player;
    
    // 각 스탯의 증가/감소 버튼
    [Header("Level Stats")]
    public Button levelUpBtn;
    public Button levelDownBtn;
    public Button expUpBtn;
    public Button expDownBtn;

    [Header("Basic Stats")]
    public Button maxHpUpBtn;
    public Button maxHpDownBtn;
    public Button hpUpBtn;
    public Button hpDownBtn;
    public Button hpRegenUpBtn;
    public Button hpRegenDownBtn;
    public Button defenseUpBtn;
    public Button defenseDownBtn;

    [Header("Combat Stats")]
    public Button mspdUpBtn;
    public Button mspdDownBtn;
    public Button atkUpBtn;
    public Button atkDownBtn;
    public Button aspdUpBtn;
    public Button aspdDownBtn;
    public Button criRateUpBtn;
    public Button criRateDownBtn;
    public Button criDamageUpBtn;
    public Button criDamageDownBtn;
    public Button projAmountUpBtn;
    public Button projAmountDownBtn;
    public Button atkRangeUpBtn;
    public Button atkRangeDownBtn;
    public Button durationUpBtn;
    public Button durationDownBtn;
    public Button cooldownUpBtn;
    public Button cooldownDownBtn;

    [Header("Utility Stats")]
    public Button revivalUpBtn;
    public Button revivalDownBtn;
    public Button magnetUpBtn;
    public Button magnetDownBtn;
    public Button growthUpBtn;
    public Button growthDownBtn;
    public Button greedUpBtn;
    public Button greedDownBtn;
    public Button curseUpBtn;
    public Button curseDownBtn;
    public Button rerollUpBtn;
    public Button rerollDownBtn;
    public Button banishUpBtn;
    public Button banishDownBtn;

    [Header("Special Stats")]
    public Button godKillToggleBtn;
    public Button barrierToggleBtn;
    public Button barrierCooldownUpBtn;
    public Button barrierCooldownDownBtn;
    public Button invincibilityToggleBtn;
    public Button dashCountUpBtn;
    public Button dashCountDownBtn;
    public Button adversaryToggleBtn;
    public Button projDestroyToggleBtn;
    public Button projParryToggleBtn;

    public Slider expSlider;

    private void Start()
    {
        player = UnitManager.Instance.GetPlayer();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // Level Stats
        SetupStatButton(levelUpBtn, levelDownBtn, StatType.Level, 1);
        SetupStatButton(expUpBtn, expDownBtn, StatType.Exp, 10);
        
        // Basic Stats
        SetupStatButton(maxHpUpBtn, maxHpDownBtn, StatType.MaxHp, 10);
        SetupStatButton(hpUpBtn, hpDownBtn, StatType.Hp, 10);
        SetupStatButton(hpRegenUpBtn, hpRegenDownBtn, StatType.HpRegen, 1);
        SetupStatButton(defenseUpBtn, defenseDownBtn, StatType.Defense, 1);

        // Combat Stats
        SetupStatButton(mspdUpBtn, mspdDownBtn, StatType.Mspd, 5f);
        SetupStatButton(atkUpBtn, atkDownBtn, StatType.ATK, 5);
        SetupStatButton(aspdUpBtn, aspdDownBtn, StatType.Aspd, 5f);
        SetupStatButton(criRateUpBtn, criRateDownBtn, StatType.CriRate, 5);
        SetupStatButton(criDamageUpBtn, criDamageDownBtn, StatType.CriDamage, 10f);
        SetupStatButton(projAmountUpBtn, projAmountDownBtn, StatType.ProjAmount, 1);
        SetupStatButton(atkRangeUpBtn, atkRangeDownBtn, StatType.ATKRange, 10f);
        SetupStatButton(durationUpBtn, durationDownBtn, StatType.Duration, 5f);
        SetupStatButton(cooldownUpBtn, cooldownDownBtn, StatType.Cooldown, 5f);

        // Utility Stats
        SetupStatButton(revivalUpBtn, revivalDownBtn, StatType.Revival, 1);
        SetupStatButton(magnetUpBtn, magnetDownBtn, StatType.Magnet, 5f);
        SetupStatButton(growthUpBtn, growthDownBtn, StatType.Growth, 5f);
        SetupStatButton(greedUpBtn, greedDownBtn, StatType.Greed, 5f);
        SetupStatButton(curseUpBtn, curseDownBtn, StatType.Curse, 5f);
        SetupStatButton(rerollUpBtn, rerollDownBtn, StatType.Reroll, 1);
        SetupStatButton(banishUpBtn, banishDownBtn, StatType.Banish, 1);

        // Special Stats (Toggle buttons)
        SetupToggleButton(godKillToggleBtn, StatType.GodKill);
        SetupToggleButton(barrierToggleBtn, StatType.Barrier);
        SetupStatButton(barrierCooldownUpBtn, barrierCooldownDownBtn, StatType.BarrierCooldown, 1);
        SetupToggleButton(invincibilityToggleBtn, StatType.Invincibility);
        SetupStatButton(dashCountUpBtn, dashCountDownBtn, StatType.DashCount, 1);
        SetupToggleButton(adversaryToggleBtn, StatType.Adversary);
        SetupToggleButton(projDestroyToggleBtn, StatType.ProjDestroy);
        SetupToggleButton(projParryToggleBtn, StatType.ProjParry);
    }

    private void SetupStatButton(Button upBtn, Button downBtn, StatType statType, float amount)
    {
        if (upBtn != null)
            upBtn.onClick.AddListener(() => player.Stats.ModifyStatValue(statType, amount));
        if (downBtn != null)
            downBtn.onClick.AddListener(() => player.Stats.ModifyStatValue(statType, -amount));
    }

    private void SetupToggleButton(Button toggleBtn, StatType statType)
    {
        if (toggleBtn != null)
        {
            toggleBtn.onClick.AddListener(() => 
            {
                bool currentValue = false;
                switch (statType)
                {
                    case StatType.GodKill:
                        currentValue = player.Stats.CurrentGodKill;
                        break;
                    case StatType.Barrier:
                        currentValue = player.Stats.CurrentBarrier;
                        break;
                    case StatType.Invincibility:
                        currentValue = player.Stats.CurrentInvincibility;
                        break;
                    case StatType.Adversary:
                        currentValue = player.Stats.CurrentAdversary;
                        break;
                    case StatType.ProjDestroy:
                        currentValue = player.Stats.CurrentProjDestroy;
                        break;
                    case StatType.ProjParry:
                        currentValue = player.Stats.CurrentProjParry;
                        break;
                }
                player.Stats.ModifyStatValue(statType, currentValue ? 0 : 1);
            });
        }
    }

    private void Update()
    {
        if(!player)
        {
            player = UnitManager.Instance.GetPlayer();
        }
        expSlider.value = (float)player.Stats.currentExp / player.Stats.CurrentMaxExp;
    }
}
