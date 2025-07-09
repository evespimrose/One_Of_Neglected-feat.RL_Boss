using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : BossMonsterBase
{
    [Header("스킬 설정")]
    [SerializeField] private float skillCooldown = 5f;
    [SerializeField] private bool isSkillReady = true;
    private float skillTimer = 0f;

    [Header("스킬 확률")]
    [SerializeField] private float skill1Probability = 0.4f;
    [SerializeField] private float skill2Probability = 0.3f;
    [SerializeField] private float skill3Probability = 0.3f;

    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<MonsterBase>(this);

        stateHandler.RegisterState(new BossMoveState(stateHandler));
        stateHandler.RegisterState(new BossBasicAttackState(stateHandler));
        stateHandler.RegisterState(new BossSkill1State(stateHandler));
        stateHandler.RegisterState(new BossSkill2State(stateHandler, true));
        stateHandler.RegisterState(new BossSkill2State(stateHandler, false));

        stateHandler.ChangeState(typeof(BossMoveState));
        //Debug.Log("[Boss] 상태 초기화 완료");
    }

    protected override void Update()
    {
        base.Update();

        if (!IsInSkillState())
        {
            if (!isSkillReady)
            {
                skillTimer += Time.deltaTime;
                if (skillTimer >= skillCooldown)
                {
                    isSkillReady = true;
                    skillTimer = 0f;
                    SelectAndUseSkill();
                }
            }
            else
            {
                SelectAndUseSkill();
            }
        }
    }

    private bool IsInSkillState()
    {
        var currentState = stateHandler.CurrentState;
        return currentState is BossSkill1State || currentState is BossSkill2State;
    }

    private void SelectAndUseSkill()
    {
        float randomValue = Random.value;

        if (randomValue < skill1Probability)
        {
            stateHandler.ChangeState(typeof(BossSkill1State));
            //Debug.Log("[Boss] 스킬1 사용: 8방향 검기");
        }
        else if (randomValue < skill1Probability + skill2Probability)
        {
            stateHandler.ChangeState(typeof(BossSkill2State));
            //Debug.Log("[Boss] 스킬2 사용: 시계방향 환영");
        }
        else
        {
            stateHandler.ChangeState(typeof(BossSkill2State));
            //Debug.Log("[Boss] 스킬3 사용: 반시계방향 환영");
        }

        isSkillReady = false;
        skillTimer = 0f;
    }
    protected void Hit()
    {
        if (isInvulnerable) return;

        // Hit 애니메이션만 재생
        Animator?.SetTrigger("Hit");
        //Debug.Log("[Boss] 피격!");
    }

    protected override void Die()
    {
        // 죽음 애니메이션 재생
        Animator?.SetTrigger("Die");

        // 모든 스킬 효과 제거
        if (SkillContainer != null)
        {
            foreach (Transform child in SkillContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // 콜라이더 비활성화
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // 리지드바디 비활성화
        if (rb != null)
        {
            rb.simulated = false;
        }

        //Debug.Log("[Boss] 사망!");

        if (UI_Manager.Instance != null && UI_Manager.Instance.panel_Dic.ContainsKey("Result_Panel"))
        {
            UI_Manager.Instance.panel_Dic["Result_Panel"].PanelOpen();
           // Debug.Log("[Boss] 결과 패널 오픈!");
        }

        // 일정 시간 후 오브젝트 제거
        Destroy(gameObject, 2f);
    }

    public void OnSkillEnd()
    {
        stateHandler.ChangeState(typeof(BossMoveState));
    }

    protected override void InitializeStats()
    {
    }
}