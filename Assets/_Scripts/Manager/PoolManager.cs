using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;
public class PoolManager : Singleton<PoolManager>
{

    #region 제네릭 풀 적용 전 예비 풀 로직(미완성)
    // public Dictionary<WorldObjectType, List<GameObject>>
    // worldObjPool = new Dictionary<WorldObjectType, List<GameObject>>();

    // public Queue<GameObject> expPool = new Queue<GameObject>();
    // protected override void Awake()
    // {
    //     base.Awake();

    // }

    // private void Start()
    // {
    //     InstantiateExp(1000);
    // }

    // public GameObject PopExp(Vector3 pos)
    // {
    //     if (expPool.Count > 0)
    //     {
    //         GameObject obj = expPool.Dequeue();
    //         obj.SetActive(true);
    //         obj.transform.SetParent(null);
    //         return obj;
    //     }
    //     else
    //     {
    //         InstantiateExp(100);
    //         GameObject obj = expPool.Dequeue();
    //         obj.SetActive(true);
    //         obj.transform.SetParent(null);
    //         return obj;
    //     }
    // }

    // public void InstantiateExp(int count)
    // {
    //     for (int i = 0; i < count; i++)
    //     {
    //         GameObject obj = Instantiate(UnitManager.Instance.GetEnvPrefab(WorldObjectType.ExpBlue));
    //         obj.SetActive(false);
    //         obj.transform.SetParent(transform);
    //         expPool.Enqueue(obj);
    //     }
    // }

    // public void PushExp(GameObject obj)
    // {
    //     obj.SetActive(false);
    //     obj.transform.SetParent(transform);
    //     expPool.Enqueue(obj);
    // }
    #endregion
}