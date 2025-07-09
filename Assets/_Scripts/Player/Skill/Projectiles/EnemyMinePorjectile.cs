using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinePorjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("[6. 지뢰 생성] EnemyMinePorjectile 초기화 완료");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[7. 충돌 감지] 플레이어와 지뢰 충돌 발생");
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(30f);
            }
            Destroy(gameObject);
        }
    }
}
