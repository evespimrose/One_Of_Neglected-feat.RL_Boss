using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;
using Random = UnityEngine.Random;
using static UnityEngine.EventSystems.EventTrigger;
using DG.Tweening;

public class UnitManager : Singleton<UnitManager>
{

    public class MonsterSpawnData
    {
        public float gameTime;  // 게임 시간 (분:초)
        public int spawnCount;  // 해당 시간대의 목표 스폰 수
    }

    [Header("몬스터 프리팹")]
    [SerializeField] private GameObject earlyNormalMonsterPrefab;
    [SerializeField] private GameObject rangedNormalMonsterPrefab;
    [SerializeField] private GameObject midNormalMonsterPrefab;
    [SerializeField] private GameObject lateNormalMonsterPrefab;
    [SerializeField] private GameObject damageUniqueMonsterPrefab;
    [SerializeField] private GameObject crowdControlUniqueMonsterPrefab;
    [SerializeField] private GameObject tankUniqueMonsterPrefab;
    [SerializeField] private GameObject bossMonsterPrefab;
    private GameObject boxMonsterPrefab;
    private int tankMonsterSpawnCount = 8;  // 탱크 몬스터 총 스폰 수
    private int tankMonsterKillCount = 0;   // 탱크 몬스터 처치 수

    [Header("스폰 설정")]
    [SerializeField] private int maxRangedMonsterCount = 20;
    [SerializeField] private int maxTotalMonsterCount = 100;
    [SerializeField] private float spawnInterval = 1f;
    private float nextSpawnTime = 0f;                        // 다음 스폰 시간
    private int currentSpawnIndex = 0;
    private MonsterSpawnData[] monsterSpawnTable = new MonsterSpawnData[]
    {
         new MonsterSpawnData { gameTime = 0.00f, spawnCount = 1 },   // n초 n마리
         new MonsterSpawnData { gameTime = 0.33f, spawnCount = 2 },  // 10초 20
         new MonsterSpawnData { gameTime = 0.50f, spawnCount = 7 },  // 20초 30
         new MonsterSpawnData { gameTime = 0.67f, spawnCount = 4 },  // 30초 40
         new MonsterSpawnData { gameTime = 0.83f, spawnCount = 5 },  // 40초 50
         new MonsterSpawnData { gameTime = 1.00f, spawnCount = 8 },  // 50초 60 
         new MonsterSpawnData { gameTime = 1.17f, spawnCount = 10 },  // 1분  70 
         new MonsterSpawnData { gameTime = 1.33f, spawnCount = 15 },  // 1분 10초  80
         new MonsterSpawnData { gameTime = 1.66f, spawnCount = 20 },
         new MonsterSpawnData { gameTime = 1.83f, spawnCount = 16 },
         new MonsterSpawnData { gameTime = 1.99f, spawnCount = 20 },
         new MonsterSpawnData { gameTime = 2.33f, spawnCount = 30 },
         new MonsterSpawnData { gameTime = 2.66f, spawnCount = 25 },
         new MonsterSpawnData { gameTime = 2.99f, spawnCount = 30 },
         new MonsterSpawnData { gameTime = 3.55f, spawnCount = 40 },
         new MonsterSpawnData { gameTime = 4.33f, spawnCount = 50 },
         new MonsterSpawnData { gameTime = 6.66f, spawnCount = 66 },
         new MonsterSpawnData { gameTime = 7.88f, spawnCount = 80 },
         new MonsterSpawnData { gameTime = 10.00f, spawnCount = 100 }  // 10분
    };
    [Header("맵 범위")]
    [SerializeField] private float mapMinX;
    [SerializeField] private float mapMaxX;
    [SerializeField] private float mapMinY;
    [SerializeField] private float mapMaxY;

    // 드랍 오브젝트 프리팹
    private GameObject expBlue;
    private GameObject expPurple;
    private GameObject expBlack;
    private GameObject bomb;
    private GameObject smallGold;
    private GameObject largeGold;
    private GameObject chicken;
    private GameObject timeStop;

    private BossMonster currentBoss;
    private bool isGameStarted = false;
    private Player currentPlayer;
    public List<MonsterBase> activeMonsters = new List<MonsterBase>();
    private List<WorldObjectType> activeExpObjects = new List<WorldObjectType>();
    private Camera mainCamera;

    public Player GetPlayer() => currentPlayer;

    private float boxSpawnInterval = 15f;
    private float nextBoxSpawnTime = 0f;

    private Sprite[] blueExpSprites;

    private class WorldObjectData
    {
        public GameObject gameObject;
        public WorldObjectType type;
        public bool isMoving;
        public Tweener moveTween;

        public WorldObjectData(GameObject obj, WorldObjectType objType)
        {
            gameObject = obj;
            type = objType;
            isMoving = false;
            moveTween = null;
        }
    }

    private List<WorldObjectData> activeWorldObjects = new List<WorldObjectData>();
    private float magnetMoveTime = 1.5f;
    private float magnetMinMoveTime = 0.5f;
    private Ease magnetEaseType = Ease.OutQuint;

    protected override void Awake()
    {
        base.Awake();

        expBlue = Resources.Load<GameObject>("Using/Env/Env_BlueExp");
        expPurple = Resources.Load<GameObject>("Using/Env/Env_PurpleExp");
        expBlack = Resources.Load<GameObject>("Using/Env/Env_BlackExp");
        bomb = Resources.Load<GameObject>("Using/Env/Env_Bomb");
        smallGold = Resources.Load<GameObject>("Using/Env/Env_SmallGold");
        largeGold = Resources.Load<GameObject>("Using/Env/Env_LargeGold");
        chicken = Resources.Load<GameObject>("Using/Env/Env_Chicken");
        timeStop = Resources.Load<GameObject>("Using/Env/Env_TimeStop");
        boxMonsterPrefab = Resources.Load<GameObject>("Using/Env/Item_Box");

        mainCamera = Camera.main;

        SpawnPlayerByType(DataManager.Instance.classSelect_Type);
        // SpawnPlayerByType(ClassType.Magician);

        blueExpSprites = new Sprite[]
        {
            Resources.Load<Sprite>("Using/UI/Icon/Icons_24x24_140"),
            Resources.Load<Sprite>("Using/UI/Icon/Icons_24x24_107"),
            Resources.Load<Sprite>("Using/UI/Icon/Icons_24x24_188"),
            Resources.Load<Sprite>("Using/UI/Icon/Icons_24x24_172")
        };
    }

    private MonsterType currentNormalMonsterType = MonsterType.EarlyNormal;

    private void Update()
    {
        if (!isGameStarted || GameManager.Instance.isPaused || currentBoss != null) return;

        float currentGameTime = TimeManager.Instance.GameTime / 60f;

        if (currentGameTime >= 10.0f)
        {
            // Debug.Log("[UnitManager] 10분 경과 - 보스전 시작");
            SpawnBossMonster();
            return;
        }

        if (Time.time < nextSpawnTime) return;  // 스폰 인터벌 체크
        nextSpawnTime = Time.time + spawnInterval;  // 다음 스폰 시간 설정 (if문 밖으로 이동)
        int spawnCount = 5; // 기본값
        for (int i = 0; i < monsterSpawnTable.Length; i++)
        {
            if (currentGameTime < monsterSpawnTable[i].gameTime)
            {
                spawnCount = i > 0 ? monsterSpawnTable[i - 1].spawnCount : monsterSpawnTable[0].spawnCount;
                break;
            }
        }
        int curseBonus = Mathf.FloorToInt(DataManager.Instance.BTS.Curse / 10f);
        spawnCount += curseBonus;

        // Debug.Log($"현재 스폰 수: {spawnCount} (기본 + 저주 보너스: {curseBonus})");
        for (int i = 0; i < spawnCount; i++)
        {
            if (GetActiveMonsterCount() >= maxTotalMonsterCount) break;

            // 20% 확률로 원거리 몬스터 생성
            if (Random.value >= 0.2f || GetRangedMonsterCount() >= maxRangedMonsterCount)
            {
                SpawnMonsterAtRandomPosition(currentNormalMonsterType);
            }
            else
            {
                SpawnMonsterAtRandomPosition(MonsterType.RangedNormal);
            }
        }

        if (Time.time >= nextBoxSpawnTime)
        {
            SpawnBoxMonster();
            // Debug.Log("상자 생성됨");
            nextBoxSpawnTime = Time.time + boxSpawnInterval;
        }

        // 테스트용 키 입력
        // if (Input.GetKeyDown(KeyCode.U))  // U키: 유니크 몬스터 소환 테스트
        // {
        //     Debug.Log("유니크 몬스터 소환 테스트 시작");
        //     SpawnUniqueMonster();
        // }

        // if (Input.GetKeyDown(KeyCode.T))  // T키: 탱크 유니크 몬스터 진형 테스트
        // {
        //     Debug.Log("탱크 유니크 몬스터 진형 테스트 시작");
        //     SpawnTankUniquesInFormation();
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha1))  // 1키: 세로 진형
        // {
        //     Debug.Log("탱크 유니크 몬스터 세로 진형 테스트");
        //     SpawnVerticalFormation(currentPlayer.transform.position);  // 플레이어 위치 사용
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha2))  // 2키: 가로 진형
        // {
        //     Debug.Log("탱크 유니크 몬스터 가로 진형 테스트");
        //     SpawnHorizontalFormation(currentPlayer.transform.position);  // 플레이어 위치 사용
        // }

        // if (Input.GetKeyDown(KeyCode.Alpha3))  // 3키: 원형 진형
        // {
        //     Debug.Log("탱크 유니크 몬스터 원형 진형 테스트");
        //     SpawnCircularFormation(currentPlayer.transform.position);  // 플레이어 위치 사용
        // }

        // 자석 효과로 이동 중인 오브젝트들 업데이트
        UpdateMagnetizedObjects();
    }

    private void OnEnable()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnKillCountReached += OnKillCountReachedHandler;
        }
    }

    private void OnDisable()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOneMinFiftySecondsPassed -= SpawnUniqueMonster;
            TimeManager.Instance.OnMinutePassed -= SpawnStrongMonsters;
        }

        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnKillCountReached -= OnKillCountReachedHandler;
        }
    }
    private void OnKillCountReachedHandler(int killCount)
    {
        if (killCount >= 1000)
        {
            SpawnUniqueMonster();
            // Debug.Log($"킬 카운트 {killCount} 달성으로 유니크 몬스터 소환");
        }
    }
    private int GetRangedMonsterCount()
    {
        return activeMonsters.Count(monster =>
            monster != null &&
            monster.GetType() == typeof(RangedNormalMonster));
    }
    private void SpawnBossMonster()
    {
        // 기존 몬스터들 제거
        ClearAllMonsters();

        // 보스 몬스터 생성
        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject bossObj = Instantiate(bossMonsterPrefab, spawnPosition, Quaternion.identity);
        currentBoss = bossObj.GetComponent<BossMonster>();

        if (currentBoss != null)
        {
            activeMonsters.Add(currentBoss);
            // Debug.Log("[UnitManager] 보스 몬스터 생성 완료");
        }
        else
        {
            // Debug.LogError("[UnitManager] 보스 몬스터 컴포넌트를 찾을 수 없음");
        }
    }

    private void SpawnUniqueMonster()
    {
        int randomValue = UnityEngine.Random.Range(1, 101);
        MonsterType monsterType;

        if (randomValue <= 40)
        {
            monsterType = MonsterType.DamageUnique;
            SpawnMonsterAtRandomPosition(monsterType);
            Debug.Log("[UnitManager] ?????? ????? ???? ?????");
        }
        else if (randomValue <= 80)
        {
            monsterType = MonsterType.CrowdControlUnique;
            SpawnMonsterAtRandomPosition(monsterType);
            Debug.Log("[UnitManager] CC ????? ???? ?????");
        }
        else
        {
            SpawnTankUniquesInFormation();
        }
    }
    private void SpawnTankUniquesInFormation()
    {
        if (currentPlayer == null) return;

        int formationRoll = Random.Range(1, 101);
        Vector2 spawnCenter = currentPlayer.transform.position;

        // if (formationRoll <= 40)  // 40% 확률로 세로 진형
        // {
        //     SpawnVerticalFormation(spawnCenter);
        //     Debug.Log("탱크 유니크 몬스터 세로 진형 소환");
        // }
        // else if (formationRoll <= 80)  // 40% 확률로 가로 진형
        // {
        //     SpawnHorizontalFormation(spawnCenter);
        //     Debug.Log("탱크 유니크 몬스터 가로 진형 소환");
        // }
        // else  // 20% 확률로 원형 진형
        // {
        //     SpawnCircularFormation(spawnCenter);
        //     Debug.Log("탱크 유니크 몬스터 원형 진형 소환");
        // }
    }
    // 탱크 몬스터 킬 카운트 증가 메서드
    public void IncreaseTankMonsterKillCount()
    {
        tankMonsterKillCount++;
        // Debug.Log($"탱크 몬스터 처치 수: {tankMonsterKillCount}");
    }

    //  탱크 몬스터 킬 카운트 리셋 메서드
    public void ResetTankMonsterKillCount()
    {
        tankMonsterKillCount = 0;
        // Debug.Log("탱크 몬스터 킬 카운트 리셋");
    }

    //  마지막 탱크 몬스터 체크 메서드
    public bool IsLastTankMonster()
    {
        return tankMonsterKillCount == (tankMonsterSpawnCount - 1);
    }
    private void SpawnVerticalFormation(Vector2 playerPos)
    {
        ResetTankMonsterKillCount();
        float spacing = 0.5f; // 몬스터 간 간격
        int monstersPerLine = 8; // 한 줄당 몬스터 수
        float distanceFromPlayer = 4f; // 플레이어로부터의 거리

        // 한쪽 방향에만 8마리 소환 (왼쪽 또는 오른쪽)
        int side = Random.Range(0, 2) * 2 - 1; // -1 또는 1
        float xOffset = side * distanceFromPlayer;

        for (int i = 0; i < monstersPerLine; i++)
        {
            float yOffset = (i - (monstersPerLine - 1) / 2f) * spacing;
            Vector2 spawnPos = playerPos + new Vector2(xOffset, yOffset);
            MonsterBase monster = SpawnMonster(MonsterType.TankUnique, spawnPos);
            if (monster != null)
            {
                // Debug.Log($"탱크 몬스터 생성 - 위치: {spawnPos}, 플레이어와의 거리: {Vector2.Distance(playerPos, spawnPos)}");
            }
        }
    }

    private void SpawnHorizontalFormation(Vector2 playerPos)
    {
        float spacing = 0.5f; // 몬스터 간 간격
        int monstersPerLine = 8; // 한 줄당 몬스터 수
        float distanceFromPlayer = 4f; // 플레이어로부터의 거리

        // 한쪽 방향에만 8마리 소환 (위 또는 아래)
        int side = Random.Range(0, 2) * 2 - 1; // -1 또는 1
        float yOffset = side * distanceFromPlayer;

        for (int i = 0; i < monstersPerLine; i++)
        {
            float xOffset = (i - (monstersPerLine - 1) / 2f) * spacing;
            Vector2 spawnPos = playerPos + new Vector2(xOffset, yOffset);
            MonsterBase monster = SpawnMonster(MonsterType.TankUnique, spawnPos);
            if (monster != null)
            {
                // Debug.Log($"탱크 몬스터 생성 - 위치: {spawnPos}, 플레이어와의 거리: {Vector2.Distance(playerPos, spawnPos)}");
            }
        }
    }

    private void SpawnCircularFormation(Vector2 playerPos)
    {
        int monsterCount = 8;
        float radius = 4f; // 원형 진형의 반지름 (플레이어로부터의 거리)

        for (int i = 0; i < monsterCount; i++)
        {
            float angle = i * (360f / monsterCount);
            Vector2 spawnPos = playerPos + new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );
            MonsterBase monster = SpawnMonster(MonsterType.TankUnique, spawnPos);
            if (monster != null)
            {
                // Debug.Log($"탱크 몬스터 생성 - 위치: {spawnPos}, 플레이어와의 거리: {Vector2.Distance(playerPos, spawnPos)}");
            }
        }
    }
    private void SpawnStrongMonsters()
    {
        float gameTime = TimeManager.Instance.GameTime;

        if (gameTime <= 180f)
        {
            currentNormalMonsterType = MonsterType.EarlyNormal;
            // Debug.Log("[UnitManager] 몬스?????????? EarlyNormal");
        }
        else if (gameTime <= 420f)
        {
            currentNormalMonsterType = MonsterType.MidNormal;
            // Debug.Log("[UnitManager] 몬스?????????? MidNormal");
        }
        else if (gameTime <= 600f)
        {
            currentNormalMonsterType = MonsterType.LateNormal;
        }
        else if (gameTime >= 600f)
        {
            // Debug.Log("[UnitManager] 보스전 시작!");

            // 게임 진행 중단 (몬스터 스폰 중지)
            isGameStarted = false;

            // 기존 몬스터 제거
            ClearAllMonsters();

            // 보스 몬스터 생성
            Vector2 spawnPosition = GetBossSpawnPosition();
            GameObject bossObj = Instantiate(bossMonsterPrefab, spawnPosition, Quaternion.identity);
            currentBoss = bossObj.GetComponent<BossMonster>();
            activeMonsters.Add(currentBoss);

            // TimeManager 이벤트 구독 해제 (더 이상 몬스터 스폰 이벤트가 필요 없음)
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.OnOneMinFiftySecondsPassed -= SpawnUniqueMonster;
                TimeManager.Instance.OnMinutePassed -= SpawnStrongMonsters;
            }

            // Debug.Log("[UnitManager] 보스 생성 완료, 일반 몬스터 스폰 중지");
        }
    }

    public Player SpawnPlayerByType(Enums.ClassType classType)
    {
        if (currentPlayer != null)
        {
            return currentPlayer;
        }

        GameObject _player;

        if (classType == Enums.ClassType.Archer)
        {
            _player = Resources.Load<GameObject>("Using/Player/Archer");
        }
        else if (classType == Enums.ClassType.Magician)
        {
            _player = Resources.Load<GameObject>("Using/Player/Magician");
        }
        else
        {
            _player = Resources.Load<GameObject>("Using/Player/Warrior");
        }

        GameObject playerObj = Instantiate(_player, Vector2.zero, Quaternion.identity);
        playerObj.AddComponent<SkillDispenser>();
        currentPlayer = playerObj.GetComponent<Player>();

        return currentPlayer;
    }

    public void StartGame()
    {
        isGameStarted = true;
        ClearAllMonsters();
        currentNormalMonsterType = MonsterType.EarlyNormal;
        nextBoxSpawnTime = Time.time + boxSpawnInterval;

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnOneMinFiftySecondsPassed += SpawnUniqueMonster;
            TimeManager.Instance.OnMinutePassed += SpawnStrongMonsters;
        }
    }

    public void ResumeGame()
    {
        isGameStarted = true;
    }
    private Vector2 GetBossSpawnPosition()
    {
        if (currentPlayer == null) return Vector2.zero;

        Vector2 playerPos = currentPlayer.transform.position;
        Vector2 spawnPos;
        int attempts = 0;
        const int MAX_ATTEMPTS = 10;

        do
        {
            // 맵 범위 내에서 랜덤한 위치 생성
            spawnPos = new Vector2(
                Random.Range(mapMinX, mapMaxX),
                Random.Range(mapMinY, mapMaxY)
            );

            // 보스는 플레이어와 최소 10유닛 이상 떨어진 위치에 스폰
            if (Vector2.Distance(spawnPos, playerPos) >= 10f)
            {
                return spawnPos;
            }

            attempts++;
        } while (attempts < MAX_ATTEMPTS);

        return spawnPos;
    }
    public MonsterBase SpawnMonsterAtRandomPosition(MonsterType type)
    {
        Vector2 randomPosition = GetRandomSpawnPosition();
        return SpawnMonster(type, randomPosition);
    }
    public MonsterBase SpawnMonster(MonsterType type, Vector2 position)
    {
        GameObject prefab = GetMonsterPrefab(type);
        if (prefab == null) return null;

        GameObject monsterObj = Instantiate(prefab, position, Quaternion.identity);
        MonsterBase monster = monsterObj.GetComponent<MonsterBase>();

        if (monster != null)
        {
            activeMonsters.Add(monster);
        }

        return monster;
    }

    public void RemoveMonster(MonsterBase monster)
    {
        if (monster != null)
        {
            activeMonsters.Remove(monster);
        }
    }

    public void ClearAllMonsters()
    {
        foreach (var monster in activeMonsters.ToArray())
        {
            if (monster != null)
            {
                Destroy(monster.gameObject);
            }
        }
        activeMonsters.Clear();
    }

    private Vector2 GetRandomSpawnPosition()
    {
        if (mainCamera == null) return Vector2.zero;

        Vector2 playerPos = currentPlayer.transform.position;
        Vector2 spawnPos;
        int attempts = 0;
        const int MAX_ATTEMPTS = 10;

        do
        {
            spawnPos = new Vector2(
                Random.Range(mapMinX, mapMaxX),
                Random.Range(mapMinY, mapMaxY)
            );

            if (Vector2.Distance(spawnPos, playerPos) >= 4f)
            {
                return spawnPos;
            }

            attempts++;
        } while (attempts < MAX_ATTEMPTS);

        return spawnPos;
    }

    private GameObject GetMonsterPrefab(MonsterType type)
    {
        if (type == MonsterType.EarlyNormal)
            return earlyNormalMonsterPrefab;
        else if (type == MonsterType.RangedNormal)
            return rangedNormalMonsterPrefab;
        else if (type == MonsterType.MidNormal)
            return midNormalMonsterPrefab;
        else if (type == MonsterType.LateNormal)
            return lateNormalMonsterPrefab;
        else if (type == MonsterType.DamageUnique)
            return damageUniqueMonsterPrefab;
        else if (type == MonsterType.CrowdControlUnique)
            return crowdControlUniqueMonsterPrefab;
        else if (type == MonsterType.TankUnique)
            return tankUniqueMonsterPrefab;
        else
            return null;
    }

    public int GetActiveMonsterCount() => activeMonsters.Count;

    public MonsterBase GetNearestMonster()
    {
        MonsterBase nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (var monster in activeMonsters)
        {
            if (monster == null) continue;

            float distance = Vector2.Distance(currentPlayer.transform.position, monster.transform.position);
            if (distance < nearestDistance)
            {
                nearestMonster = monster;
                nearestDistance = distance;
            }
        }
        return nearestMonster;
    }
    public Vector3? GetNearestMonsterPosition()
    {
        var nearestMonster = activeMonsters
        .Where(monster => monster != null)
        .Select(monster => new { monster.transform.position, distance = Vector2.Distance(currentPlayer.transform.position, monster.transform.position) })
        .OrderBy(data => data.distance)
        .FirstOrDefault();

        return nearestMonster?.position;
    }
    public List<MonsterBase> GetMonstersInRange(float minRange, float maxRange)
    {
        var monstersInRange = activeMonsters.FindAll(monster =>
        {
            if (monster == null) return false;
            float distance = Vector2.Distance(currentPlayer.transform.position, monster.transform.position);
            return distance >= minRange && distance <= maxRange;
        });

        monstersInRange.Sort((a, b) =>
        {
            float distanceA = Vector2.Distance(currentPlayer.transform.position, a.transform.position);
            float distanceB = Vector2.Distance(currentPlayer.transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });

        return monstersInRange;
    }
    public List<Vector3> GetMonsterPositionsInRange(float minRange, float maxRange)
    {
        var positionsInRange = activeMonsters
            .Where(monster => monster != null)
            .Select(monster => new { monster.transform.position, distance = Vector3.Distance(currentPlayer.transform.position, monster.transform.position) })
            .Where(data => data.distance >= minRange && data.distance <= maxRange)
            .OrderBy(data => data.distance)
            .Select(data => data.position)
            .ToList();

        return positionsInRange;
    }
    public List<Vector3> GetMonsterRamdomPositionsInRange(float minRange, float maxRange, int targetCount)
    {
        var randomPositionsInRange = activeMonsters
            .Where(monster => monster != null)
            .Select(monster => new { monster.transform.position, distance = Vector3.Distance(currentPlayer.transform.position, monster.transform.position) })
            .Where(data => data.distance >= minRange && data.distance <= maxRange)
            .OrderBy(data => data.distance)
            .Select(data => data.position)
            .OrderBy(_ => Random.value)
            .Take(targetCount)
            .ToList();

        return randomPositionsInRange;
    }
    private GameObject GetEnvPrefab(WorldObjectType type)
    {
        if (type == WorldObjectType.ExpBlue) return expBlue;
        else if (type == WorldObjectType.ExpPurple) return expPurple;
        else if (type == WorldObjectType.ExpBlack) return expBlack;
        else if (type == WorldObjectType.Boom) return bomb;
        else if (type == WorldObjectType.Gold_1) return smallGold;
        else if (type == WorldObjectType.Gold_2) return largeGold;
        else if (type == WorldObjectType.Time_Stop) return timeStop;
        else if (type == WorldObjectType.Chicken) return chicken;
        else return null;
    }

    public void SpawnWorldObject(WorldObjectType objectType, Vector2 position)
    {
        GameObject Object = Instantiate(GetEnvPrefab(objectType), position, Quaternion.identity);
        if (objectType == WorldObjectType.ExpBlue)
        {
            int val = Random.Range(0, blueExpSprites.Length);
            Object.GetComponent<SpriteRenderer>().sprite = blueExpSprites[val];
        }

        activeWorldObjects.Add(new WorldObjectData(Object, objectType));
    }

    public void RemoveWorldObject(WorldObjectType worldObject)
    {
        activeWorldObjects.RemoveAll(wo => wo.type == worldObject);
    }
    private void SpawnBoxMonster()
    {
        if (boxMonsterPrefab == null) return;

        Vector2 randomPosition = GetRandomSpawnPosition();
        GameObject boxObj = Instantiate(boxMonsterPrefab, randomPosition, Quaternion.identity);

        BoxMonster boxMonster = boxObj.GetComponent<BoxMonster>();
        if (boxMonster != null)
        {
            activeMonsters.Add(boxMonster);
        }
    }

    private void UpdateMagnetizedObjects()
    {
        if (currentPlayer == null) return;

        Vector3 playerPosition = currentPlayer.transform.position;

        for (int i = activeWorldObjects.Count - 1; i >= 0; i--)
        {
            var worldObj = activeWorldObjects[i];
            if (worldObj.gameObject == null)
            {
                activeWorldObjects.RemoveAt(i);
                continue;
            }
        }
    }

    public void ActivateMagnet()
    {
        if (currentPlayer == null) return;

        foreach (var worldObj in activeWorldObjects)
        {
            if (worldObj.gameObject == null) continue;

            worldObj.moveTween?.Kill();
            float distance = Vector3.Distance(currentPlayer.transform.position, worldObj.gameObject.transform.position);
            float moveTime = Mathf.Max(magnetMinMoveTime, magnetMoveTime * (distance / 10f));

            // 플레이어를 계속 추적하는 트윈 생성
            worldObj.moveTween = worldObj.gameObject.transform
                .DOMove(currentPlayer.transform.position, moveTime)
                .SetTarget(currentPlayer.transform)  // 타겟을 플레이어로 설정
                .SetAutoKill(false)  // 자동 종료 방지
                .SetSpeedBased()  // 시간 기반이 아닌 속도 기반으로 변경
                .SetEase(magnetEaseType)
                .OnComplete(() =>
                {
                    worldObj.isMoving = false;
                    worldObj.moveTween = null;
                });

            worldObj.isMoving = true;
        }
    }
}

public enum MonsterType
{
    EarlyNormal,
    RangedNormal,
    MidNormal,
    LateNormal,
    DamageUnique,
    CrowdControlUnique,
    TankUnique
}

