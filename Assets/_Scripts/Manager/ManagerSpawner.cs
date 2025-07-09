using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject UI_Manager_Prefab;
    [SerializeField] private GameObject DataManger_Prefab;
    private void Awake()
    {

        // try
        // {
        ;
        // }
        // catch
        // {
        //     Debug.Log("UI매니저없음");
        //     Instantiate(UI_Manager_Prefab);
        // }
        if (FindAnyObjectByType<DataManager>() == null) Instantiate(DataManger_Prefab);
        if (FindAnyObjectByType<UI_Manager>() == null) Instantiate(UI_Manager_Prefab);

    }

}