using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class UpgradeOptionImporter : MonoBehaviour
{
    [MenuItem("Tools/Import UpgradeOptions from CSV")]
    public static void ImportUpgradeOptions()
    {
        string csvPath = "Assets/Data/UpgradeOptions.csv"; // CSV 파일 경로
        string savePath = "Assets/ScriptableObjects/Upgrades_CSV/"; // 저장될 ScriptableObject 경로
        string iconFolderPath = "Assets/Sprites/UpgradeIcons/"; // 실제 아이콘 경로
        string iconLoadPath = "Sprites/UpgradeIcons/"; // Resources.Load를 안 쓸 경우 AssetDatabase용 상대 경로

        if (!File.Exists(csvPath))
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다: " + csvPath);
            return;
        }

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        string[] lines = File.ReadAllLines(csvPath);

        for (int i = 1; i < lines.Length; i++) // 0번째는 헤더
        {
            string[] fields = lines[i].Split(',');

            if (fields.Length < 6)
            {
                Debug.LogWarning($"[{i}] 행의 데이터가 부족합니다. 건너뜁니다.");
                continue;
            }

            string upgradeName = fields[0].Trim();
            string iconFileName = fields[1].Trim(); // 예: attack_icon.png
            string upgradeTypeStr = fields[2].Trim();
            string valueStr = fields[3].Trim();
            string description = fields[4].Trim();
            string spawnChanceStr = fields[5].Trim();

            if (!System.Enum.TryParse(upgradeTypeStr, out UpgradeType upgradeType))
            {
                Debug.LogWarning($"[{i}] 알 수 없는 UpgradeType: {upgradeTypeStr}. 건너뜁니다.");
                continue;
            }

            if (!float.TryParse(valueStr, out float value))
            {
                Debug.LogWarning($"[{i}] 잘못된 value 값: {valueStr}. 건너뜁니다.");
                continue;
            }

            if (!float.TryParse(spawnChanceStr, out float spawnChance))
            {
                Debug.LogWarning($"[{i}] 잘못된 spawnChance 값: {spawnChanceStr}. 기본값 1f로 설정합니다.");
                spawnChance = 1f;
            }

            // ScriptableObject 생성
            UpgradeOption upgrade = ScriptableObject.CreateInstance<UpgradeOption>();
            upgrade.upgradeName = upgradeName;
            upgrade.description = description;
            upgrade.upgradeType = upgradeType;
            upgrade.value = value;
            upgrade.spawnChance = spawnChance;

            // 아이콘 로드 (AssetDatabase 방식)
            string fullIconPath = iconFolderPath + iconFileName + ".png";
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(fullIconPath);
            if (icon == null)
            {
                Debug.LogWarning($"[{i}] 아이콘 로드 실패: {fullIconPath}");
            }
            upgrade.icon = icon;

            // 저장
            string safeName = upgradeName.Replace(" ", "_");
            string assetPath = $"{savePath}{safeName}.asset";

            AssetDatabase.CreateAsset(upgrade, assetPath);
            Debug.Log($"생성됨: {assetPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("모든 업그레이드 옵션 생성 완료");
    }
}
