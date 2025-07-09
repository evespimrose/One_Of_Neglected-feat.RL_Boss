using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Augment_Info
{
    public List<Enums.AugmentName> aug_Type = new List<Enums.AugmentName>();
    public List<string> aug_Name = new List<string>();
    [Multiline(4)]
    public List<string> aug_Text = new List<string>();
    public List<Sprite> aug_Icon = new List<Sprite>();
    public float selectedTime;
    //0 ~ MAX
    private int augsLevel = 5;
    public Sprite tempSprite;
    #region 워리어
    public void WarroirInit()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.TwoHandSword);
                    aug_Name.Add("검기[Lv.1]");
                    aug_Text.Add("전방에 검기를 날립니다");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/118"));
                    break;
                case 1:
                    aug_Type.Add(Enums.AugmentName.BigSword);
                    aug_Name.Add("지진[Lv.1]");
                    aug_Text.Add("캐릭터를 중심으로 땅을 강하게 내리쳐 피해를 입힙니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/227"));
                    break;
                case 2:
                    aug_Type.Add(Enums.AugmentName.SwordShield);
                    aug_Name.Add("패링[Lv.1]");
                    aug_Text.Add("적의 공격을 방어하며, 투사체라면 투사체를 튕겨내고 맞은 적에게 데미지를 가합니다. 사용 후 일정시간이 지나야 재사용 가능합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/125"));
                    break;
                case 3:
                    aug_Type.Add(Enums.AugmentName.Shielder);
                    aug_Name.Add("돌진[Lv.1]");
                    aug_Text.Add("캐릭터가 받는 피해가 감소하며, 대쉬 사용 시 전방으로 돌진하여 경로에 있는 적에게 피해를 가합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/153"));
                    break;
            }
        }

    }
    public void Two_Hand_Sword()
    {

        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.TwoHandSword);
                    aug_Name.Add("검기[Lv.1]");
                    aug_Text.Add("전방에 검기를 날립니다");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/118"));
                    break;
                case 1:
                    aug_Name.Add("검기[Lv.2]");
                    aug_Text.Add("검기의 크기가 증가합니다.");

                    break;
                case 2:
                    aug_Name.Add("검기[Lv.3]");
                    aug_Text.Add("검기를 두 방향으로 날립니다.");

                    break;
                case 3:
                    aug_Name.Add("검기[Lv.4]");
                    aug_Text.Add("검기를 더 빨리 사용합니다.");
                    break;
                case 4:
                    aug_Name.Add("검기[Lv.5]");
                    aug_Text.Add("검기를 세방향으로 날려보내며, 검기가 적의 투사체를 막아냅니다.");
                    break;
            }
        }

    }
    public void Big_Sword()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.BigSword);
                    aug_Name.Add("지진[Lv.1]");
                    aug_Text.Add("캐릭터를 중심으로 땅을 강하게 내리쳐 피해를 입힙니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/227"));
                    break;
                case 1:
                    aug_Name.Add("지진[Lv.2]");
                    aug_Text.Add("지진의 범위가 증가합니다.");

                    break;
                case 2:
                    aug_Name.Add("지진[Lv.3]");
                    aug_Text.Add("지진을 더 빨리 사용합니다.");

                    break;
                case 3:
                    aug_Name.Add("지진[Lv.4]");
                    aug_Text.Add("지진의 강도가 증가합니다.");
                    break;
                case 4:
                    aug_Name.Add("지진[Lv.5]");
                    aug_Text.Add("지진의 범위가 더욱 넓어지며, 여진이 추가됩니다.");
                    break;
            }
        }
    }

    public void Sword_Shield()
    {

        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.SwordShield);
                    aug_Name.Add("패링[Lv.1]");
                    aug_Text.Add("적의 공격을 방어하며, 투사체라면 투사체를 튕겨내고 맞은 적에게 데미지를 가합니다. 사용 후 일정시간이 지나야 재사용 가능합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/125"));
                    break;
                case 1:
                    aug_Name.Add("패링[Lv.2]");
                    aug_Text.Add("조금 더 빨리 방어 태세에 들어갑니다.");

                    break;
                case 2:
                    aug_Name.Add("패링[Lv.3]");
                    aug_Text.Add("방어 시 잠시간 피해를 받지 않으며, 이동 속도가 증가합니다.");

                    break;
                case 3:
                    aug_Name.Add("패링[Lv.4]");
                    aug_Text.Add("방패가 조금 더 두꺼워집니다.");
                    break;
                case 4:
                    aug_Name.Add("패링[Lv.5]");
                    aug_Text.Add("피해를 받지 않는 시간이 증가하며, 이동 속도가 큰폭으로 증가합니다.");
                    break;
            }
        }
    }
    public void Shielder()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Shielder);
                    aug_Name.Add("돌진[Lv.1]");
                    aug_Text.Add("캐릭터가 받는 피해가 감소하며, 대쉬 사용 시 전방으로 돌진하여 경로에 있는 적에게 피해를 가합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/153"));
                    break;
                case 1:
                    aug_Name.Add("돌진[Lv.2]");
                    aug_Text.Add("대쉬를 조금 더 자주 사용할 수 있습니다.");

                    break;
                case 2:
                    aug_Name.Add("돌진[Lv.3]");
                    aug_Text.Add("방패가 조금 더 단단해지며, 대쉬 회수가 증가합니다.");

                    break;
                case 3:
                    aug_Name.Add("돌진[Lv.4]");
                    aug_Text.Add("대쉬를 더 자주 사용할 수 있습니다.");
                    break;
                case 4:
                    aug_Name.Add("돌진[Lv.5]");
                    aug_Text.Add("검기가 적의 투사체를 막아냅니다.");
                    break;
            }
        }
    }
    #endregion
    #region 아처
    public void ArcherInit()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.LongBow);
                    aug_Name.Add("기본기[Lv.1]");
                    aug_Text.Add("기본 공격 시, 투사체가 1개 추가됩니다. 기본 공격에 사거리 제한이 사라집니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/LongBow_Icon"));
                    break;
                case 1:
                    aug_Type.Add(Enums.AugmentName.CrossBow);
                    aug_Name.Add("속사[Lv.1]");
                    aug_Text.Add("전방에 빠르게 화살을 쏘아냅니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/CrossBow_Icon"));
                    break;
                case 2:
                    aug_Type.Add(Enums.AugmentName.GreatBow);
                    aug_Name.Add("관통[Lv.1]");
                    aug_Text.Add("전방에 강력한 화살을 발사합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/GreateBow_Icon"));
                    break;
                case 3:
                    aug_Type.Add(Enums.AugmentName.ArcRanger);
                    aug_Name.Add("테크닉[Lv.1]");
                    aug_Text.Add("대쉬 사용 시, 전방의 부채꼴 방면으로 화살을 퍼트리듯 발사하며 이동합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/ArcRanger_Icon"));
                    break;
            }
        }

    }
    public void Long_Bow()
    {

        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.LongBow);
                    aug_Name.Add("기본기[Lv.1]");
                    aug_Text.Add("기본 공격 시, 투사체가 1개 추가됩니다. 기본 공격에 사거리 제한이 사라집니다.");

                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/LongBow_Icon"));
                    break;
                case 1:
                    aug_Name.Add("기본기[Lv.2]");
                    aug_Text.Add("화살이 한 개 더 추가되며, 피해량이 증가합니다.");

                    break;
                case 2:
                    aug_Name.Add("기본기[Lv.3]");
                    aug_Text.Add("화살의 피해량이 증가합니다.");

                    break;
                case 3:
                    aug_Name.Add("기본기[Lv.4]");
                    aug_Text.Add("화살이 한 개 더 추가되며, 피해량이 증가합니다.");
                    break;
                case 4:
                    aug_Name.Add("기본기[Lv.5]");
                    aug_Text.Add("화살의 피해량이 증가합니다.");
                    break;
            }
        }
    }
    public void Cross_Bow()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.CrossBow);
                    aug_Name.Add("속사[Lv.1]");
                    aug_Text.Add("전방에 빠르게 화살을 쏘아냅니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/CrossBow_Icon"));
                    break;
                case 1:
                    aug_Name.Add("속사[Lv.2]");
                    aug_Text.Add("속사의 주기가 빨라집니다.");
                    break;
                case 2:
                    aug_Name.Add("속사[Lv.3]");
                    aug_Text.Add("속사의 사격 횟수와 위력이 증가합니다.");
                    break;
                case 3:
                    aug_Name.Add("속사[Lv.4]");
                    aug_Text.Add("속사의 주기가 빨라집니다.");
                    break;
                case 4:
                    aug_Name.Add("속사[Lv.5]");
                    aug_Text.Add("속사의 사격 횟수와 위력이 증가합니다.");
                    break;
            }
        }
    }
    public void Great_Bow()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.GreatBow);
                    aug_Name.Add("관통[Lv.1]");
                    aug_Text.Add("전방에 강력한 화살을 발사합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/GreateBow_Icon"));
                    break;
                case 1:
                    aug_Name.Add("관통[Lv.2]");
                    aug_Text.Add("관통의 주기가 빨라집니다.");
                    break;
                case 2:
                    aug_Name.Add("관통[Lv.3]");
                    aug_Text.Add("화살의 적 타격 회수 1회 증가");
                    break;
                case 3:
                    aug_Name.Add("관통[Lv.4]");
                    aug_Text.Add("관통력이 증가합니다.");
                    break;
                case 4:
                    aug_Name.Add("관통[Lv.5]");
                    aug_Text.Add("화살의 적 타격 회수 1회 증가");
                    break;

            }
        }
    }
    public void Arc_Ranger()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.ArcRanger);
                    aug_Name.Add("테크닉[Lv.1]");
                    aug_Text.Add("대쉬 사용 시, 전방의 부채꼴 방면으로 화살을 퍼트리듯 발사하며 이동합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/ArcRanger_Icon"));
                    break;
                case 1:
                    aug_Name.Add("테크닉[Lv.2]");
                    aug_Text.Add("대쉬를 조금 더 자주 사용할 수 있습니다.");
                    break;
                case 2:
                    aug_Name.Add("테크닉[Lv.3]");
                    aug_Text.Add("방패가 조금 더 단단해지며, 대쉬 회수가 증가합니다.");
                    break;
                case 3:
                    aug_Name.Add("테크닉[Lv.4]");
                    aug_Text.Add("대쉬를 더 자주 사용할 수 있습니다.");
                    break;
                case 4:
                    aug_Name.Add("테크닉[Lv.5]");
                    aug_Text.Add("적에게 적중 시, 화살이 터집니다.");
                    break;

            }
        }
    }
    #endregion
    #region 매지션
    public void MagicianInit()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Staff);
                    aug_Name.Add("파워[Lv.1]");
                    aug_Text.Add("필드의 모든 몬스터를 대상으로 강한 공격을 가합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Power_Icon"));
                    break;
                case 1:
                    aug_Type.Add(Enums.AugmentName.Wand);
                    aug_Name.Add("캐스팅[Lv.1]");
                    aug_Text.Add("메인 특성의 쿨타임을 주기적으로 초기화합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Wand_Icon"));
                    break;
                case 2:
                    aug_Type.Add(Enums.AugmentName.Orb);
                    aug_Name.Add("쥬얼[Lv.1]");
                    aug_Text.Add("자기를 중심으로 일정 범위 내 피해를 주는 화염구를 소환합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Orb_Icon"));
                    break;
                case 3:
                    aug_Type.Add(Enums.AugmentName.Warlock);
                    aug_Name.Add("룬워드[Lv.1]");
                    aug_Text.Add("대쉬가 텔레포트로 변경됩니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/BattleMage_Icon"));
                    break;
            }
        }

    }
    public void Staff()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Staff);
                    aug_Name.Add("파워[Lv.1]");
                    aug_Text.Add("필드의 모든 몬스터를 대상으로 강한 공격을 가합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Power_Icon"));
                    break;
                case 1:
                    aug_Name.Add("파워[Lv.2]");
                    aug_Text.Add("더 강한 힘으로 적을 공격합니다.");
                    break;
                case 2:
                    aug_Name.Add("파워[Lv.3]");
                    aug_Text.Add("주기가 감소합니다.");
                    break;
                case 3:
                    aug_Name.Add("파워[Lv.4]");
                    aug_Text.Add("더 강한 힘으로 적을 공격합니다.");
                    break;
                case 4:
                    aug_Name.Add("파워[Lv.5]");
                    aug_Text.Add("주기가 감소합니다.");
                    break;

            }
        }
    }
    public void Wand()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Wand);
                    aug_Name.Add("캐스팅[Lv.1]");
                    aug_Text.Add("메인 특성의 쿨타임을 주기적으로 초기화합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Wand_Icon"));
                    break;
                case 1:
                    aug_Name.Add("캐스팅[Lv.2]");
                    aug_Text.Add("캐스팅 속도가 빨라집니다.");
                    break;
                case 2:
                    aug_Name.Add("캐스팅[Lv.3]");
                    aug_Text.Add("캐스팅 속도가 빨라집니다.");
                    break;
                case 3:
                    aug_Name.Add("캐스팅[Lv.4]");
                    aug_Text.Add("캐스팅 속도가 빨라집니다.");
                    break;
                case 4:
                    aug_Name.Add("캐스팅[Lv.5]");
                    aug_Text.Add("캐스팅 속도가 빨라집니다.");
                    break;

            }
        }
    }
    public void Orb()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Orb);
                    aug_Name.Add("쥬얼[Lv.1]");
                    aug_Text.Add("자기를 중심으로 일정 범위 내 피해를 주는 화염구를 소환합니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/Orb_Icon"));
                    break;
                case 1:
                    aug_Name.Add("쥬얼[Lv.2]");
                    aug_Text.Add("화염구의 주기가 빨라지며, 피해량이 상승합니다.");
                    break;
                case 2:
                    aug_Name.Add("쥬얼[Lv.3]");
                    aug_Text.Add("화염구의 주기가 빨라지며, 피해량이 상승합니다.");
                    break;
                case 3:
                    aug_Name.Add("쥬얼[Lv.4]");
                    aug_Text.Add("화염구의 주기가 빨라지며, 피해량이 상승합니다.");
                    break;
                case 4:
                    aug_Name.Add("쥬얼[Lv.5]");
                    aug_Text.Add("화염구가 더욱 맹렬하게 타오릅니다.");
                    break;

            }
        }
    }
    public void Warlock()
    {
        for (int i = 0; i < augsLevel; i++)
        {
            switch (i)
            {
                case 0:
                    aug_Type.Add(Enums.AugmentName.Warlock);
                    aug_Name.Add("룬워드[Lv.1]");
                    aug_Text.Add("대쉬가 텔레포트로 변경됩니다.");
                    aug_Icon.Add(Resources.Load<Sprite>("Using/UI/Icon/Custom/BattleMage_Icon"));
                    break;
                case 1:
                    aug_Name.Add("룬워드[Lv.2]");
                    aug_Text.Add("텔레포트를 조금 더 자주 사용할 수 있습니다.");
                    break;
                case 2:
                    aug_Name.Add("룬워드[Lv.3]");
                    aug_Text.Add("텔레포트 사용 회수가 1 증가하며, 피해 범위가 늘어납니다.");
                    break;
                case 3:
                    aug_Name.Add("룬워드[Lv.4]");
                    aug_Text.Add("텔레포트를 조금 더 자주 사용할 수 있습니다.");
                    break;
                case 4:
                    aug_Name.Add("룬워드[Lv.5]");
                    aug_Text.Add("텔레포트 사용 회수가 1 증가하며, 피해 범위가 늘어납니다.");
                    break;
            }
        }
    }
    #endregion
}

