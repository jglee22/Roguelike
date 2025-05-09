using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private static PlayerSpawner instance;

    [SerializeField] private GameObject playerPrefab;

    void Awake()
    {
        // 이미 존재하면 자기 자신 제거
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 최초 진입 시 유지
        instance = this;
        DontDestroyOnLoad(gameObject);

        // 플레이어가 없으면 생성
        if (PlayerStatus.Instance == null)
        {
            GameObject player = Instantiate(playerPrefab);
            player.name = "Player";
            DontDestroyOnLoad(player);
        }
    }
}
