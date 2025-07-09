using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Player player;

    private void Start()
    {
        //이게 성능상 좋긴 할건데 인스펙터 수정시 반영 안됨
        //player.Stats.OnHealthChanged += UpdateHPBar;
        //UpdateHPBar(player.Stats.Hp);
    }
     
    private void Update()
    {
        transform.rotation = Quaternion.identity;

        float ratio = player.Stats.currentHp / (float)player.Stats.CurrentMaxHp;
        hpBar.value = ratio;
    }

    private void UpdateHPBar(float currentHp)
    {
        float ratio = currentHp / player.Stats.CurrentMaxHp;
        hpBar.value = ratio;
    }

    private void OnDestroy()
    {
        player.Stats.OnHpChanged -= UpdateHPBar;
    }
}
