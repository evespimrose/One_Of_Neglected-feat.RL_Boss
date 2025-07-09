using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeDefine
{
    ATK,
    DEF,
    UTI
}
public enum ATK_Bless
{
    None,
    ATK_INCREASE,
    PROJECTILE_INCREASE,
    ATK_SPEED_INCREASE,
    CRITICAL_DAMAGE_INCREASE,
    CRITICAL_PERCENT_INCREASE,
    PROJECTILE_DESTROY,
    PROJECTILE_PARRY,
    GOD_KILL
}
public enum DEF_Bless
{
    None,
    MAX_HP_INCREASE,
    DEFENSE_INCREASE,
    HP_REGEN_INCREASE,
    BARRIER_ACTIVATE,
    BARRIER_COOLDOWN,
    INVINCIBILITY,
    ADVERSARY
}
public enum UTI_Bless
{
    None,
    ATK_RANGE,
    DURATION,
    COOLDOWN,
    Revival,
    MAGNET,
    GROWTH,
    Greed,
    DASH
}

public class Bless_Panel : Panel
{
    [SerializeField] private Upgrade_Panel upgrade_Panel;
    public Bless bless;
    //공통된걸 등록해줄 리스트
    public List<Node> ATK_Node_List;
    public List<Node> DEF_Node_List;
    public List<Node> UTI_Node_List;

    public List<Image> ATK_Node_Line;
    public List<Image> DEF_Node_Line;
    public List<Image> UTI_Node_Line;

    public NodeReset nodeReset;
    public BlessTooltip tooltip;
    private bool isInit = false;
    private void OnEnable()
    {
        if (isInit)
        {
            AfterInit(ref ATK_Node_List);
            AfterInit(ref DEF_Node_List);
            AfterInit(ref UTI_Node_List);
        }
    }

    private void Start()
    {
        Node_Initialize(ref ATK_Node_List, ref ATK_Node_Line);
        Node_Initialize(ref DEF_Node_List, ref DEF_Node_Line);
        Node_Initialize(ref UTI_Node_List, ref UTI_Node_Line);

    }

    //딕셔너리 초기화 및 불러온 데이터에 따라 노드활성화
    // 어째서인지 모르지만 게임씬에서 넘어왔을 때 딕셔너리의 키를 못찾아옴
    private void Node_Initialize(ref List<Node> nodes, ref List<Image> nodeLines)
    {
        int i = 0;
        foreach (Node node in nodes)
        {   //딕셔너리에 없으면 추가
            if (DataManager.Instance.bless_Dic.ContainsKey(node.m_ID) == false)
            {
                DataManager.Instance.bless_Dic.Add(node.m_ID, false);
            }
            if (DataManager.Instance.bless_Dic[node.m_ID] == true)
            {
                DataManager.Instance.player_Property.bless_Point++;
                node.m_BTN.onClick?.Invoke();
                DataManager.Instance.player_Property.bless_Point--;
            }
            node.baseNodeAction += upgrade_Panel.DisplayBlessPoint;
            ByNodeDefine(node);
            isInit = true;
            if (node.next_Nodes.Count != 0)
            {
                node.m_Line = nodeLines[i];
                if (DataManager.Instance.bless_Dic.ContainsKey(node.m_ID))
                    if (DataManager.Instance.bless_Dic[node.m_ID])
                    {
                        node.m_Line.color = Color.white;
                    }
            }

            i++;
        }
    }

    private void ByNodeDefine(Node node)
    {
        switch (node.nodeDefine)
        {
            case NodeDefine.ATK:
                Add_ATK_Bless(node);
                break;
            case NodeDefine.DEF:
                Add_DEF_Bless(node);
                break;
            case NodeDefine.UTI:
                Add_UTI_Bless(node);
                break;
        }
    }
    private void AfterInit(ref List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.baseNodeAction += upgrade_Panel.DisplayBlessPoint;
            ByNodeDefine(node);
        }
    }
    private void Add_ATK_Bless(Node node)
    {
        switch (node.ATK_Bless)
        {
            case ATK_Bless.ATK_INCREASE:
                node.methodAction += bless.ATK_Modify;
                break;
            case ATK_Bless.PROJECTILE_INCREASE:
                node.methodAction += bless.ProjAmount_Modify;
                break;
            case ATK_Bless.ATK_SPEED_INCREASE:
                node.methodAction += bless.ASPD_Modify;
                break;
            case ATK_Bless.CRITICAL_DAMAGE_INCREASE:
                node.methodAction += bless.CriDamage_Modify;
                break;
            case ATK_Bless.CRITICAL_PERCENT_INCREASE:
                node.methodAction += bless.CriRate_Modify;
                break;
            case ATK_Bless.PROJECTILE_DESTROY:
                node.methodAction += bless.ProjDestroy_Modify;
                break;
            case ATK_Bless.PROJECTILE_PARRY:
                node.methodAction += bless.ProjParry_Modify;
                break;
            case ATK_Bless.GOD_KILL:
                node.methodAction += bless.GodKill_Modify;
                break;
        }
    }

    private void Add_DEF_Bless(Node node)
    {
        switch (node.DEF_Bless)
        {
            case DEF_Bless.MAX_HP_INCREASE:
                node.methodAction += bless.MaxHP_Modify;
                break;
            case DEF_Bless.DEFENSE_INCREASE:
                node.methodAction += bless.Defense_Modify;
                break;
            case DEF_Bless.HP_REGEN_INCREASE:
                node.methodAction += bless.HPRegen_Modify;
                break;
            case DEF_Bless.BARRIER_ACTIVATE:
                node.methodAction += bless.Barrier_Modify;
                break;
            case DEF_Bless.BARRIER_COOLDOWN:
                node.methodAction += bless.BarrierCooldown_Modify;
                break;
            case DEF_Bless.INVINCIBILITY:
                node.methodAction += bless.Invincibility_Modify;
                break;
            case DEF_Bless.ADVERSARY:
                node.methodAction += bless.Adversary_Modify;
                break;
        }
    }

    private void Add_UTI_Bless(Node node)
    {
        switch (node.UTI_Bless)
        {
            case UTI_Bless.ATK_RANGE:
                node.methodAction += bless.ATKRange_Modify;
                break;
            case UTI_Bless.DURATION:
                node.methodAction += bless.Duration_Modify;
                break;
            case UTI_Bless.COOLDOWN:
                node.methodAction += bless.Cooldown_Modify;
                break;
            case UTI_Bless.Revival:
                node.methodAction += bless.Revival_Modify;
                break;
            case UTI_Bless.MAGNET:
                node.methodAction += bless.Magnet_Modify;
                break;
            case UTI_Bless.GROWTH:
                node.methodAction += bless.Growth_Modify;
                break;
            case UTI_Bless.Greed:
                node.methodAction += bless.Greed_Modify;
                break;
            case UTI_Bless.DASH:
                node.methodAction += bless.DashCount_Modify;
                break;
        }
    }

    private void OnDisable()
    {
        foreach (Node node in ATK_Node_List)
        {
            node.baseNodeAction -= upgrade_Panel.DisplayBlessPoint;
            switch (node.ATK_Bless)
            {
                case ATK_Bless.ATK_INCREASE:
                    node.methodAction -= bless.ATK_Modify;
                    break;
                case ATK_Bless.PROJECTILE_INCREASE:
                    node.methodAction -= bless.ProjAmount_Modify;
                    break;
                case ATK_Bless.ATK_SPEED_INCREASE:
                    node.methodAction -= bless.ASPD_Modify;
                    break;
                case ATK_Bless.CRITICAL_DAMAGE_INCREASE:
                    node.methodAction -= bless.CriDamage_Modify;
                    break;
                case ATK_Bless.CRITICAL_PERCENT_INCREASE:
                    node.methodAction -= bless.CriRate_Modify;
                    break;
                case ATK_Bless.PROJECTILE_DESTROY:
                    node.methodAction -= bless.ProjDestroy_Modify;
                    break;
                case ATK_Bless.PROJECTILE_PARRY:
                    node.methodAction -= bless.ProjParry_Modify;
                    break;
                case ATK_Bless.GOD_KILL:
                    node.methodAction -= bless.GodKill_Modify;
                    break;
            }
        }
        foreach (Node node in DEF_Node_List)
        {
            node.baseNodeAction -= upgrade_Panel.DisplayBlessPoint;
            switch (node.DEF_Bless)
            {
                case DEF_Bless.MAX_HP_INCREASE:
                    node.methodAction -= bless.MaxHP_Modify;
                    break;
                case DEF_Bless.DEFENSE_INCREASE:
                    node.methodAction -= bless.Defense_Modify;
                    break;
                case DEF_Bless.HP_REGEN_INCREASE:
                    node.methodAction -= bless.HPRegen_Modify;
                    break;
                case DEF_Bless.BARRIER_ACTIVATE:
                    node.methodAction -= bless.Barrier_Modify;
                    break;
                case DEF_Bless.BARRIER_COOLDOWN:
                    node.methodAction -= bless.BarrierCooldown_Modify;
                    break;
                case DEF_Bless.INVINCIBILITY:
                    node.methodAction -= bless.Invincibility_Modify;
                    break;
                case DEF_Bless.ADVERSARY:
                    node.methodAction -= bless.Adversary_Modify;
                    break;
            }
        }
        foreach (Node node in UTI_Node_List)
        {
            node.baseNodeAction -= upgrade_Panel.DisplayBlessPoint;
            switch (node.UTI_Bless)
            {
                case UTI_Bless.ATK_RANGE:
                    node.methodAction -= bless.ATKRange_Modify;
                    break;
                case UTI_Bless.DURATION:
                    node.methodAction -= bless.Duration_Modify;
                    break;
                case UTI_Bless.COOLDOWN:
                    node.methodAction -= bless.Cooldown_Modify;
                    break;
                case UTI_Bless.Revival:
                    node.methodAction -= bless.Revival_Modify;
                    break;
                case UTI_Bless.MAGNET:
                    node.methodAction -= bless.Magnet_Modify;
                    break;
                case UTI_Bless.GROWTH:
                    node.methodAction -= bless.Growth_Modify;
                    break;
                case UTI_Bless.Greed:
                    node.methodAction -= bless.Greed_Modify;
                    break;
                case UTI_Bless.DASH:
                    node.methodAction -= bless.DashCount_Modify;
                    break;
            }
        }
    }

}
