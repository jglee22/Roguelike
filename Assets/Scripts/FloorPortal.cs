using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorPortal : MonoBehaviour
{
    public GameObject portalEffect;
    //public GameObject upgradeUI;

    public bool IsPortalActivated => isPortalActivated;
    private bool canMoveNext = false;

    private bool isPortalActivated = false;
    private void Start()
    {
        portalEffect.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.currentFloor++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 적 전멸 후 호출
    public void ActivatePortal()
    {
        portalEffect.SetActive(true); // 숨겨져 있던 포탈 활성화
        isPortalActivated = true;
    }
}
