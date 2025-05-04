using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PortalManager portalManager;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    public bool bossTest = false;

    public static int EnemyCount;
    public int enemyCount;
    private bool enemySpawned = false;

    private void Awake()
    {
       portalManager = FindAnyObjectByType<PortalManager>();
        EnemyCount = 0;
    }
    private void Start()
    {
        SpawnEnemiesOrBoss();
    }
    void Update()
    {
        if(enemyCount != EnemyCount)
            enemyCount = EnemyCount;

        if (!enemySpawned) return;
        if (GameManager.Instance != null)
            if (GameManager.Instance.isGameOver) return;
       
        if (EnemyCount == 0)
        {
            Debug.Log($"Enemy All Die");
            if (!portalManager.IsPortalActivated)
            {
                portalManager.PortalEnable();
                enabled = false; // 한 번만 호출
                Debug.Log($"Portal 호출 / 업그레이드 UI 호출");
                FindAnyObjectByType<UpgradeManager>().RequestUpgrade();
            }
        }
    }

    public void SpawnEnemiesOrBoss()
    {
        EnemyCount++;
        int currentFloor = GameManager.Instance.currentFloor;

        if (!bossTest)
        {
            if (currentFloor % 10 == 0)
            {
                SpawnBoss(); // 10층 단위로 보스 생성
            }
            else
            {
                SpawnEnemies(); // 일반 적 생성
            }
        }
        else
            SpawnBoss();

        enemySpawned = true;
    }

    // 아래처럼 간단한 더미 함수라도 만들어줘야 해
    void SpawnEnemies()
    {
        // 예시: 일반 적 여러 명 생성하는 코드
        Debug.Log("일반 적 생성");

        Vector3 spawnPos = GetBossSpawnPosition(); // 원하는 위치 지정
        GameObject boss = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
    void SpawnBoss()
    {
        Vector3 spawnPos = GetBossSpawnPosition(); // 원하는 위치 지정
        GameObject boss = Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        // 보스 전용 설정 (예: 보스 체력 UI 표시)
        boss.GetComponent<EnemyHealth>().isBoss = true;
        BossUIManager.Instance.ShowBossHPBar(boss.GetComponent<EnemyHealth>());
    }
    Vector3 GetBossSpawnPosition()
    {
        // 예시: 씬 중앙 근처 위치
        return new Vector3(-5.22f, 0, -3.4f); // 원하는 위치로 조정 가능
    }
}
