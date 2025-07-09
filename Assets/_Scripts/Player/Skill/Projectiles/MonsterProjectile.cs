using UnityEngine;
using Cysharp.Threading.Tasks;
using static Enums;
using System.Threading;

public class MonsterProjectile : Projectile
{
    private float elapsedTime = 0f;

    public Vector3 StartPosition { get; private set; }
    public void InitProjectile(Vector3 startPos, Vector3 direction, float speed, float damage)
    {
        this.startPosition = startPos;
        StartPosition = startPos;
        Vector3 targetPos = startPos + direction * 100f; 
        base.InitProjectile(startPos, targetPos, speed, damage);
    }
    protected override void Start()
    {
        base.Start();
    }

    private bool isDestroyed = false;  // ???댘 ??? 筌ｋ똾寃?????삋域?

    protected override async UniTaskVoid MoveProjectileAsync(CancellationToken token)
    {
        try
        {
            while (!isDestroyed)  // isDestroyed 筌ｋ똾寃?
            {
                if (this == null || gameObject == null)  // null 筌ｋ똾寃??곕떽?
                {
                    return;
                }
                // 시간 체크
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= lifeTime)
                {
                    DestroyProjectile();
                    return;
                }

                // 부모 클래스와 동일한 이동 로직 사용
                Vector3 direction = (targetPosition - transform.position).normalized;
                transform.position += speed * Time.deltaTime * direction;

                await UniTask.Yield();
            }
        }
        catch (System.Exception e)
        {
            // ?癒?쑎 嚥≪뮄??(?醫뤾문??鍮?
            Debug.LogWarning($"Projectile movement interrupted: {e.Message}");
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // ?겸뫖猷?筌ｌ꼶??
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.OnProjectileHit(this);
                player.TakeDamage(damage);
            }
            DestroyProjectile();
        }
        //else if (collision.CompareTag("Wall"))
        //{
        //    DestroyProjectile();
        //}
    }

    private void DestroyProjectile()
    {
        if (isDestroyed) return;  // ???? ???댘??뤿???삠늺 ?귐뗪쉘

        isDestroyed = true;

        // ?袁⑥쨮?????筌띲끇????癒?퐣 ??볤탢
        ProjectileManager.Instance?.RemoveProjectile(this);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        isDestroyed = true;
    }
}