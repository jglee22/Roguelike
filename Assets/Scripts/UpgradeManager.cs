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
    MaxHealth,
    ElementalFire,
    ElementalIce,
}
public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanelDescText;
    public GameObject upgradePanel;

    public List<UpgradeOption> allUpgrades;

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
        // 업그레이드 가능한 항목만 필터링
        var availableUpgrades = allUpgrades
            .Where(data => !PlayerStatus.Instance.IsUpgradeMaxed(data.upgradeType))
            .ToList();

        // 업그레이드 가능한 항목이 하나도 없으면 UI 표시하지 않음
        if (availableUpgrades.Count == 0)
        {
            Debug.Log("모든 업그레이드를 완료했습니다.");
            return;
        }

        // 가능한 업그레이드 중에서 최대 3개 랜덤 선택
        var options = availableUpgrades
            .OrderBy(x => Random.value)
            .Take(Mathf.Min(3, availableUpgrades.Count))
            .ToList();

        foreach (var upgrade in options)
        {
            // 버튼 프리팹 생성 및 하위 요소 캐싱
            GameObject buttonGO = Instantiate(upgradeButtonPrefab, upgradePanel.transform);
            Image icon = buttonGO.transform.Find("Icon").GetComponent<Image>();
            TMP_Text descText = buttonGO.transform.Find("DescText").GetComponent<TMP_Text>();
            Button btn = buttonGO.GetComponent<Button>();

            // 버튼 이름 설정 (디버깅 용도)
            btn.name = upgrade.upgradeType.ToString();

            // 아이콘 및 설명 설정
            icon.sprite = upgrade.icon;
            descText.text = $"{upgrade.description}\n<color=#AAAAAA>{upgrade.GetStatusText()}</color>";

            // 클릭 시 업그레이드 적용 및 UI 비활성화
            btn.onClick.AddListener(() =>
            {
                ApplyUpgrade(upgrade);
                OnUpgradeSelected(); // 업그레이드 후 처리 (패널 끄기 등)
                Debug.Log($"업그레이드 : {upgrade.upgradeType}");
                upgradePanel.gameObject.SetActive(false);
                upgradePanelDescText.SetActive(false);
            });
        }

        // 게임 정지 및 패널 표시
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
        switch (upgradeOption.upgradeType)
        {
            case UpgradeType.AttackPower:
                PlayerStatus.Instance.UpgradeAttackPower();
                break;
            case UpgradeType.AttackSpeed:
                PlayerStatus.Instance.UpgradeAttackSpeed();
                break;
            case UpgradeType.HealthRegen:
                PlayerStatus.Instance.UpgradeHealthRegen();
                break;
            case UpgradeType.CritChance:
                PlayerStatus.Instance.UpgradeCritChance();
                break;
            case UpgradeType.MaxHealth:
                PlayerStatus.Instance.UpgradeMaxHealth();
                break;
            case UpgradeType.ElementalFire:
                //PlayerStatus.Instance.currentElement |= ElementType.Fire;
                PlayerStatus.Instance.ApplyElement(ElementType.Fire);
                PlayerStatus.Instance.UpgradeAttackPower();
                Debug.Log($"Fire");
                break;
            case UpgradeType.ElementalIce:
                //PlayerStatus.Instance.currentElement |= ElementType.Ice;
                PlayerStatus.Instance.ApplyElement(ElementType.Ice);
                PlayerStatus.Instance.UpgradeAttackPower();
                Debug.Log($"Ice");
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
