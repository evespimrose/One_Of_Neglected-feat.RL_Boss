using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class TankUniqueMonster : NormalMonster
{
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: 100f,
            speed: 1f,
            damage: 10f,
            range: 0.7f,
            cooldown: 1f,
            defense: 6f,        
            regen: 5f,          
            regenDelay: 1f
        );
    }
    protected override void Update()
    {
        base.Update();
        if (stats.healthRegen > 0)
        {
            stats.RegenerateHealth(Time.deltaTime);
        }
    }

}
