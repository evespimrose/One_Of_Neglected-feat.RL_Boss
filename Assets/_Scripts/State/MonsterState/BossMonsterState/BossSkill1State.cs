using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill1State : MonsterStateBase
{
    private float duration = 1f;  // 스킬 지속시간 단축
    private float timer = 0f;
    private bool hasSpawnedEffects = false;

    public BossSkill1State(StateHandler<MonsterBase> handler) : base(handler) { }

    public override void Enter(MonsterBase entity)
    {
        timer = 0f;
        hasSpawnedEffects = false;
        entity.Animator?.SetTrigger("Skill1");
        //Debug.Log("[Boss] 스킬1 시작: 8방향 검기");
    }

    public override void Update(MonsterBase entity)
    {
        timer += Time.deltaTime;

        // 스킬 시작 직후 바로 검기 발사
        if (!hasSpawnedEffects)
        {
            SpawnEightDirectionalEffects(entity);
            hasSpawnedEffects = true;
        }

        // 검기 발사 후 일정 시간 뒤 스킬 종료
        if (timer >= duration)
        {
            BossMonster boss = entity as BossMonster;
            boss?.OnSkillEnd();
        }
    }

    private void SpawnEightDirectionalEffects(MonsterBase entity)
    {
        BossMonsterBase boss = entity as BossMonsterBase;
        if (boss == null) return;

        // 8방향 각도 (-90도부터 시작하여 45도씩 증가)
        float[] angles = { -90f, -45f, 0f, 45f, 90f, 135f, 180f, 225f };

        foreach (float angle in angles)
        {
            // 각도를 라디안으로 변환
            float rad = angle * Mathf.Deg2Rad;

            // 방향 벡터 계산
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            // 보스 위치에서 바로 생성
            GameObject effect = GameObject.Instantiate(
                boss.AttackEffectPrefab,
                boss.transform.position,
                Quaternion.Euler(0, 0, angle),  // 진행 방향으로 회전
                boss.SkillContainer
            );

            // ProjectileMovement 초기화
            ProjectileMovement projectile = effect.GetComponent<ProjectileMovement>();
            if (projectile != null)
            {
                float projectileSpeed = 10f;  // 검기 속도 증가
                float projectileDamage = 100f;
                projectile.Initialize(direction, projectileSpeed, projectileDamage);
                var collider = effect.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                }

                //Debug.Log($"[Boss] 검기 발사 - 각도: {angle}, 데미지: {projectileDamage}");
            }
        }
    }
}