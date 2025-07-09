using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// 0~3분 초반부 일반 몬스터
public class EarlyNormalMonster : NormalMonster
{
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 10f,
            speed: 1f,
            damage: 5f,
            range: 0.7f,
            cooldown: 1f,
            defense: 1
        );
    }
}
