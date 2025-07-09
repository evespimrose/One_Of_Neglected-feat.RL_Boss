using UnityEngine;
using static Enums;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class WorldObject : MonoBehaviour
{
    public WorldObjectType objectType;
    private int ExpAmount;

    private float expConcentrationInterval = 25f;
    private float expConcentrationRadius = 1f;
    private LayerMask expLayer;
    private bool isConcentrating = false;
    private CancellationTokenSource cts;


    public int GetExpAmount()
    {
        return ExpAmount;
    }


    private void Awake()
    {
        switch (objectType)
        {
            case WorldObjectType.ExpBlue:
                ExpAmount = 10;
                if (!isConcentrating)
                {
                    cts = new CancellationTokenSource();
                    StartExpConcentration(cts.Token).Forget();
                    isConcentrating = true;
                }
                break;
        }

        expLayer = LayerMask.GetMask("EnvObject");
    }

    private async UniTaskVoid StartExpConcentration(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(expConcentrationInterval), cancellationToken: cancellationToken);
                
                if (this == null || !gameObject || GameManager.Instance.isPaused) 
                    continue;

                await ConcentrateNearbyExp(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception ex)
        {
            //Debug.LogError($"StartExpConcentration 에러: {ex}");
        }
    }

    private async UniTask ConcentrateNearbyExp(CancellationToken cancellationToken)
    {
        try
        {
            if (objectType != WorldObjectType.ExpBlue || !gameObject) return;

            var colliders = Physics2D.OverlapCircleAll(
                transform.position, 
                expConcentrationRadius,
                expLayer
            );

            int expCount = 0;
            int totalExpAmount = ExpAmount;
            var expObjects = new System.Collections.Generic.List<WorldObject>();

            foreach (var collider in colliders)
            {
                if (cancellationToken.IsCancellationRequested) return;

                WorldObject worldObj = collider.GetComponent<WorldObject>();
                
                if (worldObj != null && worldObj != this && worldObj.objectType == WorldObjectType.ExpBlue)
                {
                    expObjects.Add(worldObj);
                    totalExpAmount += worldObj.GetExpAmount();
                    expCount++;
                }
            }

            if (expCount > 0 && gameObject != null && !cancellationToken.IsCancellationRequested)
            {
                GameObject expBlackPrefab = Resources.Load<GameObject>("Using/Env/Env_BlueExp");
                Vector3 spawnPosition = transform.position;
                
                GameObject blackExp = Instantiate(expBlackPrefab, spawnPosition, Quaternion.identity);
                WorldObject newExpObject = blackExp.GetComponent<WorldObject>();
                if (newExpObject != null)
                {
                    newExpObject.SetExpAmount(totalExpAmount);
                    
                    float scaleFactor = 1f + (totalExpAmount - 10) * 0.1f;
                    scaleFactor = Mathf.Clamp(scaleFactor, 1f, 1.3f);
                    blackExp.transform.localScale = Vector3.one * scaleFactor;
                }

                foreach (var expObj in expObjects)
                {
                    if (cancellationToken.IsCancellationRequested) 
                    {
                        if (blackExp != null)
                        {
                            Destroy(blackExp);
                        }
                        return;
                    }

                    if (expObj != null && expObj.gameObject != null)
                    {
                        Destroy(expObj.gameObject);
                    }
                }

                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            if (!(ex is OperationCanceledException))
            {
               // Debug.LogError($"ConcentrateNearbyExp 에러: {ex}");
            }
        }
    }

    public void SetExpAmount(int amount)
    {
        ExpAmount = amount;
    }

    public void selfDestroy()
    {
        cts?.Cancel();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }
}
