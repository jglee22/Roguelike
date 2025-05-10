using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public UpgradeType upgradeType;
    public float value;  // 예: 증가 수치 (ex. +10%, +2 등)
    public string description;
    [Range(0, 1f)]
    public float spawnChance = 0f; // 선택지 등장 확률

    public string GetStatusText()
    {
        switch (upgradeType)
        {
            case UpgradeType.AttackSpeed:
                float curSpeed = PlayerStatus.Instance.attackSpeed;
                float maxSpeed = PlayerStatus.Instance.maxAttackSpeed;
                return $"공격 속도: {curSpeed:F2} / {maxSpeed:F2}";

            case UpgradeType.CritChance:
                float curCrit = PlayerStatus.Instance.critChance;
                float maxCrit = PlayerStatus.Instance.maxCriticalChance;
                return $"치명타 확률: {curCrit:F0}% / {maxCrit:F0}%";

            case UpgradeType.HealthRegen:
                float curRegen = PlayerStatus.Instance.regenAmount;
                float maxRegen = PlayerStatus.Instance.maxRegenAmount;
                return $"체력 재생: {curRegen:F1} / {maxRegen:F1}";

            // 필요시 다른 스탯도 추가
            default:
                return "";
        }
    }
}
