using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateNormalMonster : NormalMonster
{
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 80f,
            speed: 1.5f,
            damage: 19f,
            range: 0.7f,
            cooldown: 1f,
            defense: 3f
        );
    }
}