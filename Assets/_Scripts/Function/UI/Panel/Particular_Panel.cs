using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particular_Panel : MonoBehaviour
{
    [SerializeField] GameObject particularMember;
    [SerializeField] RectTransform content_Rect;

    private void Awake()
    {
        PlayerStats stats = UnitManager.Instance.GetPlayer().Stats;
        for (int i = 0; i < 19; i++)
        {
            GameObject obj = Instantiate(particularMember, content_Rect);
            ParticularMember element = obj.GetComponent<ParticularMember>();
            // m_Members.Add(element);
            switch (i)
            {
                case 0:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/126");
                    element.m_NameTMP.text = "최대체력";
                    element.m_ValueTMP.text = stats.CurrentMaxHp.ToString();
                    break;
                case 1:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/84");
                    element.m_NameTMP.text = "체력회복";
                    element.m_ValueTMP.text = stats.CurrentHpRegen.ToString();
                    break;
                case 2:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/28");
                    element.m_NameTMP.text = "방어력";
                    element.m_ValueTMP.text = stats.CurrentDefense.ToString();
                    break;
                case 3:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/22");
                    element.m_NameTMP.text = "이동속도";
                    element.m_ValueTMP.text = stats.CurrentMspd.ToString();
                    break;
                case 4:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/84");
                    element.m_NameTMP.text = "공격력";
                    element.m_ValueTMP.text = stats.CurrentATK.ToString();
                    break;
                case 5:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/95");
                    element.m_NameTMP.text = "공격속도";
                    element.m_ValueTMP.text = stats.CurrentAspd.ToString();
                    break;
                case 6:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/101");
                    element.m_NameTMP.text = "치명타 확률";
                    element.m_ValueTMP.text = stats.CurrentCriRate.ToString();
                    break;
                case 7:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/97");
                    element.m_NameTMP.text = "치명타 피해";
                    element.m_ValueTMP.text = stats.CurrentCriDamage.ToString();
                    break;
                case 8:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/52");
                    element.m_NameTMP.text = "투사체 개수";
                    element.m_ValueTMP.text = stats.CurrentProjAmount.ToString();
                    break;
                case 9:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/250 WARRIOR ICONS (pack by batareya)/31");
                    element.m_NameTMP.text = "공격범위";
                    element.m_ValueTMP.text = stats.CurrentATKRange.ToString();
                    break;
                case 10:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/112");
                    element.m_NameTMP.text = "지속시간";
                    element.m_ValueTMP.text = stats.CurrentDuration.ToString();
                    break;
                case 11:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/114");
                    element.m_NameTMP.text = "쿨타입";
                    element.m_ValueTMP.text = stats.CurrentCooldown.ToString();
                    break;
                case 12:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/244");
                    element.m_NameTMP.text = "부활";
                    element.m_ValueTMP.text = stats.CurrentRevival.ToString();
                    break;
                case 13:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/186");
                    element.m_NameTMP.text = "자석";
                    element.m_ValueTMP.text = stats.CurrentMagnet.ToString();
                    break;
                case 14:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/36");
                    element.m_NameTMP.text = "성장";
                    element.m_ValueTMP.text = stats.CurrentGrowth.ToString();
                    break;
                case 15:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/108");
                    element.m_NameTMP.text = "탐욕";
                    element.m_ValueTMP.text = stats.CurrentGreed.ToString();
                    break;
                case 16:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/51");
                    element.m_NameTMP.text = "저주";
                    element.m_ValueTMP.text = stats.CurrentCurse.ToString();
                    break;
                case 17:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/Custom/dice_Icon4");
                    element.m_NameTMP.text = "새로고침";
                    element.m_ValueTMP.text = stats.CurrentReroll.ToString();
                    break;
                case 18:
                    element.m_IconIMG.sprite = Resources.Load<Sprite>("Using/UI/Icon/MAGE ICONS BIG PACk (by Batareya)/28");
                    element.m_NameTMP.text = "지우기";
                    element.m_ValueTMP.text = stats.CurrentBanish.ToString();
                    break;

            }
        }
    }

}
