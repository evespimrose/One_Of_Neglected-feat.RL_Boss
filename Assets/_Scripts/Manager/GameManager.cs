using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] Camera main_Cam;
    public bool isPaused = false;
    public bool isGameStarted = false;
    public List<SpriteRenderer> allSpriteRenderer = new List<SpriteRenderer>();
    Vector3 pos;

    Vector3 externalPos;
    private GameObject enemyMinePrefab;
    public UnityEvent EnemyMineCreate = new UnityEvent();

    protected override void Awake()
    {
        base.Awake();
        main_Cam = Camera.main;
        enemyMinePrefab = Resources.Load<GameObject>("Using/Projectile/enemy_Mine1");
        if (enemyMinePrefab == null)
        {
            Debug.LogError("enemy_Mine1 프리팹을 로드할 수 없습니다!");
        }
        EnemyMineCreate.AddListener(CreateEnemyMine);
    }
    private void Start()
    {
        StartGame();

    }
    private void Update()
    {
        HandleInputs();
        RendererHandler();
    }

    public void RendererHandler()
    {

        foreach (SpriteRenderer targetObj in allSpriteRenderer)
        {
            pos = Camera.main.WorldToViewportPoint(targetObj.transform.position);
            if (pos.x > 1.15 || pos.x < -0.15 || pos.y < -0.32 || pos.y > 1.12)
                targetObj.enabled = false;
            else
                targetObj.enabled = true;
        }

    }

    private void HandleInputs()
    {
        if (UI_Manager.Instance.panel_Dic["LevelUp_Panel"].gameObject.activeSelf ||
        UI_Manager.Instance.panel_Dic["Result_Panel"].gameObject.activeSelf) return;
        // ESC로 일시정지/재개
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UI_Manager.Instance.panel_Dic["Option_Panel"].PanelClose(true);
                Time.timeScale = 1;
                isPaused = false;
            }
            else
            {
                UI_Manager.Instance.panel_Dic["Option_Panel"].PanelOpen();
                Time.timeScale = 0;
                isPaused = true;
            }
        }

    }

    public void StartGame()
    {
        isPaused = false;
        isGameStarted = true;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ResetTime();
        }
        else
        {
            // Debug.LogError("TimeManager is not initialized!");
        }

        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.StartGame();
        }
        else
        {
            // Debug.LogError("UnitManager is not initialized!");
        }
    }

    public void SetExternalPosition(Vector3 position)
    {
        externalPos = position;
        Debug.Log("[5. 이벤트 발생] EnemyMineCreate 이벤트 발생");
        EnemyMineCreate.Invoke();
    }

    public void CreateEnemyMine()
    {
        if (enemyMinePrefab != null)
        {
            Instantiate(enemyMinePrefab, externalPos, Quaternion.identity);
        }
    }
}