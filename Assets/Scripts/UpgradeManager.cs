using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class UpgradeSlot
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
}
public enum UpgradeType
{
    AttackPower,
    AttackSpeed,
    HealthRegen,
    CritChance,
    MaxHealth
}
public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanelDescText;
    public GameObject upgradePanel;

    public List<UpgradeOption> allUpgrades;
    //public List<UpgradeSlot> upgradeSlots;

    public GameObject upgradeButtonPrefab; // 버튼 프리팹

    private Queue<bool> upgradeQueue = new Queue<bool>();
    private bool isUpgradeShowing = false;


    private void Start()
    {
        upgradePanelDescText.SetActive(false);
        upgradePanel.SetActive(false);
    }
    public void ShowUpgradeOptions()
    {
        // 업그레이드 3개 랜덤 선택
        var options = allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        foreach (var upgrade in options)
        {
            GameObject buttonGO = Instantiate(upgradeButtonPrefab, upgradePanel.transform);
            var icon = buttonGO.transform.Find("Icon").GetComponent<Image>();
            //var nameText = buttonGO.transform.Find("NameText").GetComponent<TMP_Text>();
            var descText = buttonGO.transform.Find("DescText").GetComponent<TMP_Text>();
            var btn = buttonGO.GetComponent<Button>();
            btn.name = upgrade.upgradeType.ToString();
            icon.sprite = upgrade.icon;
            //nameText.text = upgrade.upgradeName;
            descText.text = upgrade.description;

            btn.onClick.AddListener(() =>
            {
                ApplyUpgrade(upgrade);
                OnUpgradeSelected();
                Debug.Log($"업그레이드 : {upgrade.upgradeType}");
                upgradePanel.gameObject.SetActive(false);
                upgradePanelDescText.SetActive(false);
            });
        }

        Time.timeScale = 0f;
        upgradePanelDescText.SetActive(true);
        upgradePanel.gameObject.SetActive(true);
        
    }

    public void SelectAttackPower()
    {
        PlayerStatus.Instance.UpgradeAttackPower();
        GameManager.Instance.savedAttackPower += 3f;
        ClosePanel();
    }

    public void SelectAttackSpeed()
    {
        PlayerStatus.Instance.UpgradeAttackSpeed();
        GameManager.Instance.savedAttackSpeed += 0.2f;
        ClosePanel();
    }

    public void SelectMaxHealth()
    {
        PlayerStatus.Instance.UpgradeMaxHealth();
        GameManager.Instance.savedMaxHP += 20;
        ClosePanel();
    }
    public void SelectHealthRegen()
    {
        PlayerStatus.Instance.UpgradeHealthRegen();
        
    }
    public void ApplyUpgrade(UpgradeOption upgradeOption)
    {
        var status = PlayerStatus.Instance;

        switch (upgradeOption.upgradeType)
        {
            case UpgradeType.AttackPower:
                status.UpgradeAttackPower();
                break;
            case UpgradeType.AttackSpeed:
                status.UpgradeAttackSpeed();
                break;
            case UpgradeType.HealthRegen:
                if (!status.isHealthRegen)
                    status.isHealthRegen = true;
                else
                    status.regenAmount += 1f;
                break;
            case UpgradeType.CritChance:
                status.UpgradeCritChance();
                break;
            case UpgradeType.MaxHealth:
                status.UpgradeMaxHealth();
                break;
        }
        
        ClosePanel();
    }
    void ClosePanel()
    {
        // 버튼들 제거
        foreach (Transform child in upgradePanel.transform)
        {
            Destroy(child.gameObject);
        }
        upgradePanel.SetActive(false);
        upgradePanelDescText.SetActive(false);
        Time.timeScale = 1f;
    }
    public void RequestUpgrade()
    {
        upgradeQueue.Enqueue(true);
        TryShowUpgrade(); // 중복 방지 체크 포함
    }
    private void TryShowUpgrade()
    {
        if (isUpgradeShowing || upgradeQueue.Count == 0)
            return;

        isUpgradeShowing = true;

        // 실제 UI 표시
        ShowUpgradeOptions();

        Debug.Log("업그레이드 UI 표시됨");
    }
    public void OnUpgradeSelected()
    {
        upgradeQueue.Dequeue();
        isUpgradeShowing = false;
        TryShowUpgrade();
    }
}
