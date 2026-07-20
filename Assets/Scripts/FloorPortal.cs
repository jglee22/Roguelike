using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorPortal : MonoBehaviour
{
    public GameObject portalEffect;

    public bool IsPortalActivated => isPortalActivated;

    private bool isPortalActivated = false;

    private void Start()
    {
        if (portalEffect != null)
            portalEffect.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (UpgradeManager.Instance != null && UpgradeManager.Instance.IsUpgradeBlocking)
        {
            Debug.Log("강화 선택 전에는 다음 층으로 이동할 수 없습니다.");
            return;
        }

        if (GameManager.Instance == null)
            return;

        GameManager.Instance.currentFloor++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ActivatePortal()
    {
        if (portalEffect != null)
            portalEffect.SetActive(true);
        isPortalActivated = true;
    }
}
