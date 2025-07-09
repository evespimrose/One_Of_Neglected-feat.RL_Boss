using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidNormalMonster : NormalMonster
{
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 40f,
            speed: 1.2f,
            damage: 11f,
            range: 0.7f,
            cooldown: 1f,
            defense:2f
        );
    }
}