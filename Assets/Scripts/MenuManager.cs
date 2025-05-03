using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResetGameData();
        SceneManager.LoadScene("GameScene"); // 실제 플레이 씬 이름
    }

    public void ExitGame()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("게임 종료"); // 에디터에서 확인용
    }
}
