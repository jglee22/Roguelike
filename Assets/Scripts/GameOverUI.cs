using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button mainmenuButton;
    public Button retryButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainmenuButton.onClick.AddListener(() =>
        {
            GameManager.Instance.GoToMainMenu();
        });
        retryButton.onClick.AddListener(() =>
        {
            GameManager.Instance.Retry();
        });
        GameManager.Instance.RegisterGameOverPanel(this.gameObject);
    }

    
   
}
