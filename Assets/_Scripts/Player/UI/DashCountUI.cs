using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class DashCountUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image[] dashRechargeImages;

    private void Start()
    {
        UpdateDashImages();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
        UpdateDashImages();
    }

    private void UpdateDashImages()
    {
        for (int i = 0; i < dashRechargeImages.Length; i++)
        {
            dashRechargeImages[i].transform.parent.gameObject.SetActive(i < player.MaxDashCount);

            if (i < player.MaxDashCount)
            {
                dashRechargeImages[i].gameObject.SetActive(true);

                if (i < player.CurrentDashCount)
                {
                    dashRechargeImages[i].fillAmount = 1;
                }
                else if (i == player.CurrentDashCount)
                {
                    dashRechargeImages[i].fillAmount = player.DashRechargeTimer / (player.dashRechargeTime * player.Stats.CurrentCooldown);
                }
                else
                {
                    dashRechargeImages[i].fillAmount = 0;
                }
            }
            else
            {
                dashRechargeImages[i].gameObject.SetActive(false);
            }
        }
    }
} 