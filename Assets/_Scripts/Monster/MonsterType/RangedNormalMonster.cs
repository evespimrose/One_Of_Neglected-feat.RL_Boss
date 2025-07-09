using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class RangedNormalMonster : RangedMonster
{
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 10f,
            speed: 0.8f,
            damage: 3f,
            range: 3f,
            cooldown: 1.5f        
        );
    }
}
