using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    public CanvasScaler m_CanvasScaler;
    public TextMeshProUGUI inGameTextTMP;
    public TextMeshProUGUI inGameTipTMP;

    private List<string> inGameText = new List<string>();
    private List<string> inGameTip = new List<string>();
    private void Awake()
    {
        for (int i = 0; i < 15; i++)
        {
            switch (i)
            {
                case 0:
                    inGameText.Add("#0. 아이들은  '용사가 나타나 마왕을 물리쳤어요.'\n라는 내용의 이야길 좋아한다.");
                    inGameTip.Add("DPS에 가장 큰 영향을 미치는 것은, 사실 책이랍니다.");
                    break;
                case 1:
                    inGameText.Add("#1. 마족에 마왕이 나타나니,\n왕국에 마왕군의 침공이 시작되었다.");
                    inGameTip.Add("투사체가 많으면 게임이 편해집니다.");
                    break;
                case 2:
                    inGameText.Add("#2. 왕국은 마계에 대항하기 위해\n만반의 준비를 가했다.");
                    inGameTip.Add("보유한 특성을 지운다면 그 특성은 해당 회차에\n더 이상 등장하지 않습니다.");
                    break;
                case 3:
                    inGameText.Add("#3. 마왕군의 침공이 시작되고,\n용사의 부재에도 왕국은 치열한 공방을 주고 받았다.");
                    inGameTip.Add("감당할 수 있는 저주는 사실 축복일지도…");
                    break;
                case 4:
                    inGameText.Add("#4. 긴 시간 전선을 유지하였지만,\n용사는 나타나지 않았다.");
                    inGameTip.Add("가호 중 하나인 대적자는 화신의 공격 능력을 약화시킵니다.");
                    break;
                case 5:
                    inGameText.Add("#5. 오랜 전쟁에 지친 왕국의 전선은\n후퇴를 반복하였고, 공세를 잃어가기 시작했다.");
                    inGameTip.Add("가호 중 하나인 신살은 화신의 방어 능력을 약화시킵니다.");
                    break;
                case 6:
                    inGameText.Add("#6. 왕국의 백성들은 피난길에 올랐으며, 전선에 근접해\n있는 영토들은 마왕군에 의해 쑥대밭이 되었다.");
                    inGameTip.Add("대적자와 신살을 보유할 경우, 화신은 크게 약화됩니다.");
                    break;
                case 7:
                    inGameText.Add("#7. 한 학자가 찾아낸 과거 진행된 '용사 소환 의식'");
                    inGameTip.Add("가호는 공격, 방어, 유틸 3가지 가호가 있습니다.");
                    break;
                case 8:
                    inGameText.Add("#8. 왕국은 용사가 소환되기까지\n절대 방어 태세에 들어갔다.");
                    inGameTip.Add("골드는 몬스터 처치와 상자 파괴에서\n확률적으로 얻을 수 있습니다.");
                    break;
                case 9:
                    inGameText.Add("#9. 왕국의 모든 백성들은 용사가 하루빨리 소환되길\n바라며 살아남기 위한 발버둥을 계속했다.");
                    inGameTip.Add("체력이 부족하다면 상자를 파괴해보세요.");
                    break;
                case 10:
                    inGameText.Add("#10. 용사가 나타나지 않았다면,\n누군가는 용사의 역할을 대신 해야한다.");
                    inGameTip.Add("몬스터는 시간이 지날수록 점점 강해집니다.");
                    break;
                case 11:
                    inGameText.Add("#11. 백성들은 언제 소환될지 모를 용사보다,\n당장 눈 앞의 병사에게 더욱 의지한다.");
                    inGameTip.Add("속도는 생각보다 중요한 역할입니다.");
                    break;
                case 12:
                    inGameText.Add("#12. 용사가 있어야 이야기가 시작이 된다면,\n직접 용사가 되는건 어떨까?");
                    inGameTip.Add("범위가 넓으면 그만큼 많은 범위의 적을 공격할 수 있습니다.");
                    break;
                case 13:
                    inGameText.Add("#13. 용사가 되기 위해, 왕국을 위해, 백성을 위해…\n혼자일지라도 전장에 나서는 병사");
                    inGameTip.Add("주신의 가호 아래, 왕국을 수호하세요.");
                    break;
                case 14:
                    inGameText.Add("#14. 반복되는 죽음에도 병사는 무기를 들었다. 이야기의\n시작을 위해. 방치된 세계의 시간이 흐르길 바라며");
                    inGameTip.Add("단련을 게을리하지 마세요.");
                    break;
            }
        }
    }
    private void Start()
    {
        int idx = UnityEngine.Random.Range(0, 6);
        inGameTextTMP.text = inGameText[idx];

        idx = UnityEngine.Random.Range(0, 4);
        inGameTipTMP.text = inGameTip[idx];

    }
}
