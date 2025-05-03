using UnityEditor;
using UnityEngine;

public class UpgradeCreator : MonoBehaviour
{
    [MenuItem("Tools/Create New Upgrade Option")]
    public static void CreateUpgradeOption()
    {
        // 폴더 경로
        string folderPath = "Assets/ScriptableObjects/Upgrades";

        // 폴더 없으면 생성
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Upgrades");

        // 새 ScriptableObject 인스턴스 생성
        UpgradeOption asset = ScriptableObject.CreateInstance<UpgradeOption>();

        // 파일 이름 지정
        string assetName = "NewUpgrade_" + System.Guid.NewGuid().ToString("N").Substring(0, 6);
        string fullPath = $"{folderPath}/{assetName}.asset";

        // 생성 및 저장
        AssetDatabase.CreateAsset(asset, fullPath);
        AssetDatabase.SaveAssets();

        // 선택 상태로 만들기
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log($"[UpgradeCreator] 생성됨: {fullPath}");
    }
}
