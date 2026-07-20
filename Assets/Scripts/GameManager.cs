using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private const string MainSceneName = "Main";
    private const string PlayerObjectName = "Player";

    public int currentFloor = 1;

    public float savedAttackSpeed = 1.0f;
    public float savedAttackPower = 10f;
    public int savedMaxHP = 100;

    public GameObject gameOverPanel;
    [SerializeField]
    private GameObject playerPrefab;

    public GameObject player;

    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        EnsurePlayerExists();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MainSceneName)
            return;

        EnsurePlayerExists();
    }

    private void EnsurePlayerExists()
    {
        if (player != null)
            return;

        player = GameObject.FindGameObjectWithTag(PlayerObjectName);
        if (player != null)
            return;

        if (playerPrefab == null)
        {
            Debug.LogError("[GameManager] playerPrefab이 할당되지 않았습니다.");
            return;
        }

        player = Instantiate(playerPrefab);
        player.name = PlayerObjectName;
        DontDestroyOnLoad(player);
    }

    public void IncreaseFloor()
    {
        currentFloor++;
        Debug.Log($"다음층으로 이동 : 현재층 : {currentFloor}");
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        else
            Debug.LogError("[GameManager] gameOverPanel이 등록되지 않았습니다.");
    }

    public void ResetGameData()
    {
        currentFloor = 1;
        EnemyManager.EnemyCount = 0;
        isGameOver = false;

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.ResetStats();
    }

    public void RegisterGameOverPanel(GameObject panel)
    {
        gameOverPanel = panel;
        gameOverPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        DestroyPlayer();
        ResetGameData();
        SceneManager.LoadScene(MainSceneName);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        ResetGameData();
        RespawnPlayer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RespawnPlayer()
    {
        DestroyPlayer();
        EnsurePlayerExists();
    }

    private void DestroyPlayer()
    {
        if (player != null)
        {
            Destroy(player);
            player = null;
            return;
        }

        GameObject found = GameObject.FindGameObjectWithTag(PlayerObjectName);
        if (found != null)
            Destroy(found);
    }
}
