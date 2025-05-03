using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayMainSceneShortCut : MonoBehaviour
{
    [MenuItem("Tools/Play Main Scene %h")] // Ctrl + Shift + H
    static void PlayMainScene()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            // 씬 저장 여부 확인
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                // 메인 씬 경로 (프로젝트 내 Main 씬 경로로 수정 필요!)
                string mainScenePath = "Assets/Scenes/Main.unity";

                EditorSceneManager.OpenScene(mainScenePath);
                EditorApplication.isPlaying = true;
            }
        }
    }
}
