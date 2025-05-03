using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;
    public Dictionary<int, FloorData> difficultyMap = new();

    void Awake()
    {
        Instance = this;
        LoadCSV();
    }

    void LoadCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("difficulty_data");
        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string[] values = lines[i].Split(',');
            if (values.Length < 4) continue;

            FloorData data = new()
            {
                floor = int.Parse(values[0]),
                enemyHP = int.Parse(values[1]),
                enemyDamage = int.Parse(values[2]),
                enemySpeed = float.Parse(values[3])
            };

            difficultyMap[data.floor] = data;
        }
    }

    public FloorData GetFloorData(int floor)
    {
        return difficultyMap.ContainsKey(floor) ? difficultyMap[floor] : null;
    }
}
