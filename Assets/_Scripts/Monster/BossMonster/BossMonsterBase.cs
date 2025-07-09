using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMonsterBase : MonsterBase
{
    [Header("보스 설정")]
    [SerializeField] protected BossType[] possibleStats;       
    [SerializeField] protected GameObject attackEffectPrefab;
    [SerializeField] protected GameObject phantomPrefab;


    protected Transform skillContainer;    // 스킬 이펙트 컨테이너
    protected bool isInvulnerable;        // 무적 상태

    public GameObject AttackEffectPrefab => attackEffectPrefab;
    public GameObject PhantomPrefab => phantomPrefab;
    public Transform SkillContainer => skillContainer;

    protected override void Awake()
    {
        base.Awake();
        SetupBoss();

        skillContainer = new GameObject("SkillContainer").transform;
        skillContainer.SetParent(transform);
        skillContainer.localPosition = Vector3.zero;
    }

    // 보스 초기 설정
    private void SetupBoss()
    {
        // 플레이어 스킬 상태에 따른 스탯 설정
        BTS playerSkills = DataManager.Instance.BTS;
        int statIndex;

        if (!playerSkills.Adversary && !playerSkills.GodKill)
        {
            statIndex = 0;           // 스킬 없음 - 무적
            isInvulnerable = true;
        }
        else if (playerSkills.Adversary && !playerSkills.GodKill)
        {
            statIndex = 1;           // Adversary만
            isInvulnerable = false;
        }
        else if (!playerSkills.Adversary && playerSkills.GodKill)
        {
            statIndex = 2;           // GodKill만
            isInvulnerable = false;
        }
        else
        {
            statIndex = 3;           // 둘 다 있음
            isInvulnerable = false;
        }
        BossType selectedStats = possibleStats[statIndex];
        float attackCooldown = selectedStats.attackSpeed;
        stats = new MonsterStats(
            selectedStats.health,
            selectedStats.moveSpeed,
            selectedStats.damage,
            attackCooldown,
            selectedStats.defense,
            selectedStats.healthRegen,
            selectedStats.attackRange
        );
    }
    public override bool IsPlayerInAttackRange()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        float attackRange = Stats.attackRange;  // 공격 범위가 너무 작을 수 있음

        
        return distance <= attackRange;
    }

    public override void TakeDamage(float damage)
    {
        // 무적 상태면 데미지 무시
        if (isInvulnerable)
        {
            //Debug.Log("[Boss] 무적 상태 - 데미지 무시됨");
            return;
        }

        base.TakeDamage(damage);
    }

}

[Serializable]
public struct BossType
{
    public string typeName;      // 보스 타입 이름 (God Incarnate 등)
    public float health;         // 체력
    public float damage;         // 공격력
    public float defense;        // 방어력
    public float moveSpeed;      // 이동속도
    public float attackSpeed;    // 공격속도
    public float healthRegen;    // 초당 체력 회복량
    public float attackRange;
}
