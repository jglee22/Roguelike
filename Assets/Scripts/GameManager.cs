using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
        }
        
    }
    private void Start()
    {
        SpawnPlayerIfNotExists();
    }
    private void SpawnPlayerIfNotExists()
    {
        player = Instantiate(playerPrefab);
        player.name = "Player";
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
        Time.timeScale = 0f; // 게임 정지
        gameOverPanel.SetActive(true);
        //ResetGameData();
    }
    public void ResetGameData()
    {
        if (player == null)
            player = GameObject.Find("Player")?.gameObject;
        currentFloor = 1;
        EnemyManager.EnemyCount = 0;
        isGameOver = false;

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
        SceneManager.LoadScene("Main");
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
