using System.IO;
using UnityEditor;
using UnityEngine;

public class UpgradeDataToOptionConverter : MonoBehaviour
{
    [MenuItem("Tools/Convert UpgradeData to UpgradeOption")]
    public static void ConvertAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:UpgradeData");

        string targetFolder = "Assets/ScriptableObjects/Upgrades_Converted";
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            UpgradeData oldData = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);

            if (oldData == null) continue;

            UpgradeOption newData = ScriptableObject.CreateInstance<UpgradeOption>();
            newData.upgradeName = oldData.upgradeName;
            newData.description = oldData.description;
            newData.icon = oldData.icon;
            newData.upgradeType = oldData.upgradeType;
            newData.value = oldData.amount;

            string newPath = $"{targetFolder}/{oldData.name}_converted.asset";
            AssetDatabase.CreateAsset(newData, newPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ {guids.Length}개의 UpgradeData가 UpgradeOption으로 변환되었습니다.");
    }
}
