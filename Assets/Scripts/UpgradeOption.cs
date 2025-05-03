using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Upgrades/UpgradeOption")]
public class UpgradeOption : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public UpgradeType upgradeType;
    public float value;  // 예: 증가 수치 (ex. +10%, +2 등)
    public string description;
}
