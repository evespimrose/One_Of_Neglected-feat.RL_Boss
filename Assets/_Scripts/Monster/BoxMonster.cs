using UnityEngine;
using static Enums;

public class BoxMonster : MonsterBase
{
    [SerializeField] private float health = 1f;
    
    protected override void InitializeStats()
    {
        stats = new MonsterStats(
            health: health,   
            speed: 0f,        
            damage: 0f,       
            range: 0f,        
            cooldown: 0f,     
            defense: 0f,      
            regen: 0f,        
            regenDelay: 0f    
        );
    }

    protected override void InitializeStateHandler()
    {
        stateHandler = null;
    }

    public override void MoveTowardsPlayer()
    {
    }

    public override void TakeDamage(float damage)
    {
        SoundManager.Instance.Play("MonsterAttacked", SoundManager.Sound.Effect, 1f, false, 0.3f);
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    protected override void Die()
    {
        if (gameObject == null) return;

        DropRandomItem();
        UnitManager.Instance.RemoveMonster(this);
        Destroy(gameObject);
    }

    private void DropRandomItem()
    {
        int randomValue = Random.Range(1, 101);
        WorldObjectType dropType;

        if (randomValue <= 35) { return; }
        else if (randomValue <= 45) {dropType = WorldObjectType.Gold_1;}
        else if (randomValue <= 50) {dropType = WorldObjectType.Gold_2;}
        else if (randomValue <= 80) {dropType = WorldObjectType.Chicken;}
        else if (randomValue <= 90){dropType = WorldObjectType.Time_Stop;}
        else {dropType = WorldObjectType.Boom;}

        UnitManager.Instance.SpawnWorldObject(dropType, transform.position);
    }

    protected override void UpdateMonsterStats()
    {
    }

    public override void ApplyKnockback(Vector2 hitPoint, float knockbackForce)
    {
    }

    protected override void OnMonsterDestroy()
    {
    }
} 