using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossSkill2State : MonsterStateBase
{
    private float attackInterval = 0.5f;
    private float timer = 0f;
    private int currentPhantomIndex = 0;
    private List<GameObject> phantoms = new List<GameObject>();
    private Transform player;
    private Vector3 originalPosition;
    private bool isClockwise;
    private Renderer bossRenderer;

    public BossSkill2State(StateHandler<MonsterBase> handler, bool clockwise = true) : base(handler)
    {
        isClockwise = clockwise;
    }

    public override void Enter(MonsterBase entity)
    {
        timer = 0f;
        currentPhantomIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalPosition = entity.transform.position;

        bossRenderer = entity.GetComponent<Renderer>();
        if (bossRenderer != null) bossRenderer.enabled = false;

        SpawnPhantoms(entity);
        entity.Animator?.SetTrigger("Skill2");
        //Debug.Log($"[Boss] 스킬{(isClockwise ? "2" : "3")} 시작");
    }

    private void SpawnPhantoms(MonsterBase entity)
    {
        BossMonsterBase boss = entity as BossMonsterBase;
        if (boss == null || player == null) return;

        float radius = 4f;  // 플레이어로부터의 거리
        int phantomCount = 8;  // 환영 개수
        float angleStep = 360f / phantomCount;  // 각 환영 사이의 각도

        // 12시 방향부터 시작
        float startAngle = 90f;

        for (int i = 0; i < phantomCount; i++)
        {
            // 각도 계산 (시계/반시계 방향에 따라)
            float angle = isClockwise ?
                startAngle + (i * angleStep) :
                startAngle - (i * angleStep);

            // 각도를 라디안으로 변환
            float rad = angle * Mathf.Deg2Rad;

            // 위치 계산
            Vector2 spawnPos = (Vector2)player.position + new Vector2(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius
            );

            // 환영 생성
            GameObject phantom = GameObject.Instantiate(
                boss.PhantomPrefab,
                spawnPos,
                Quaternion.identity,
                boss.SkillContainer
            );

            // PhantomController 초기화
            PhantomController phantomCtrl = phantom.GetComponent<PhantomController>();
            if (phantomCtrl != null)
            {
                phantomCtrl.Initialize(boss.Stats.attackDamage);
            }

            phantoms.Add(phantom);
            //Debug.Log($"환영 생성 - 인덱스: {i}, 각도: {angle}, 위치: {spawnPos}");
        }
    }

    public override void Update(MonsterBase entity)
    {
        timer += Time.deltaTime;

        if (phantoms == null || phantoms.Count == 0)
        {
            BossMonster boss = entity as BossMonster;
            boss?.OnSkillEnd();
            return;
        }

        // 일정 간격으로 순차적으로 돌진
        if (timer >= attackInterval && currentPhantomIndex < 8)
        {
            int index = isClockwise ? currentPhantomIndex : (7 - currentPhantomIndex);
            if (index >= 0 && index < phantoms.Count)
            {
                GameObject phantom = phantoms[index];
                if (phantom != null)
                {
                    LaunchPhantomAttack(phantom);
                    //Debug.Log($"환영 돌진 - 인덱스: {index}");
                }
            }
            currentPhantomIndex++;
            timer = 0f;
        }

        // 모든 환영이 돌진을 마치면 스킬 종료
        if (currentPhantomIndex >= 8 && AreAllPhantomsFinished())
        {
            if (bossRenderer != null) bossRenderer.enabled = true;
            entity.transform.position = originalPosition;
            CleanupPhantoms();

            BossMonster boss = entity as BossMonster;
            boss?.OnSkillEnd();
        }
    }

    private void LaunchPhantomAttack(GameObject phantom)
    {
        if (phantom == null || player == null) return;

        PhantomController phantomCtrl = phantom.GetComponent<PhantomController>();
        if (phantomCtrl != null)
        {
            Vector3 direction = (player.position - phantom.transform.position).normalized;
            phantomCtrl.StartDash(direction);
        }
    }

    private bool AreAllPhantomsFinished()
    {
        if (phantoms == null || phantoms.Count == 0) return true;
        return phantoms.All(p => p == null || p.GetComponent<PhantomController>()?.HasFinishedDash == true);
    }

    private void CleanupPhantoms()
    {
        if (phantoms != null)
        {
            foreach (var phantom in phantoms)
            {
                if (phantom != null)
                    GameObject.Destroy(phantom);
            }
            phantoms.Clear();
        }
    }

    public override void Exit(MonsterBase entity)
    {
        CleanupPhantoms();
        if (bossRenderer != null) bossRenderer.enabled = true;
        //Debug.Log($"[Boss] 스킬{(isClockwise ? "2" : "3")} 종료");
    }
}