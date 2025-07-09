using UnityEngine;
using UnityEngine.UI;

public class BarrierUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image barrierRechargeImage;
    [SerializeField] private GameObject uiObject;  

    private void Start()
    {
        uiObject.SetActive(player.Stats.CurrentBarrier);
        UpdateBarrierImage();
    }

    private void LateUpdate()
    {
        if (!player.Stats.CurrentBarrier) return;

        transform.rotation = Quaternion.identity;
        UpdateBarrierImage();
    }

    private void UpdateBarrierImage()
    {
        if (player.HasBarrierCharge)
        {
            barrierRechargeImage.fillAmount = 1;
        }
        else
        {
            barrierRechargeImage.fillAmount = player.BarrierRechargeTimer / player.BarrierRechargeTime;
        }
    }
} 