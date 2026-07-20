using UnityEngine;

/// <summary>
/// 플레이어 생성은 GameManager가 담당합니다.
/// 씬에 남아 있는 경우 충돌을 막기 위해 비활성화합니다.
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
    private void Awake()
    {
        Debug.LogWarning("[PlayerSpawner] 사용되지 않습니다. 플레이어 생성은 GameManager가 담당합니다.");
        Destroy(gameObject);
    }
}
