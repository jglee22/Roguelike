using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject portal;
    public bool IsPortalActivated => isPortalActivated;

    private bool isPortalActivated = false;
    public void PortalEnable()
    {
        portal.SetActive(true);
    }
}
