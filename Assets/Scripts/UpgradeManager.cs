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
    public static UpgradeManager Instance { get; private set; }

    public GameObject upgradePanelDescText;
    public GameObject upgradePanel;

    public List<UpgradeOption> allUpgrades;

    public GameObject upgradeButtonPrefab; // 버튼 프리팹

    private Queue<bool> upgradeQueue = new Queue<bool>();
    private bool isUpgradeShowing = false;
    private List<Button> upgradeButtons = new List<Button>(); // 생성된 버튼 추적용

    /// <summary>강화 선택 중에는 이동·전투·포탈 진입을 막습니다.</summary>
    public bool IsUpgradeBlocking => isUpgradeShowing || upgradeQueue.Count > 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        upgradePanelDescText.SetActive(false);
        upgradePanel.SetActive(false);
    }
    public void ShowUpgradeOptions(List<UpgradeOption> selectedOptions)
    {
        ClearUpgradeButtons();

        upgradePanel.gameObject.SetActive(true);
        upgradePanelDescText.SetActive(true);
        Time.timeScale = 0f;

        foreach (var option in selectedOptions)
        {
            CreateUpgradeSlot(option);
        }
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
        foreach (Transform child in upgradePanel.transform)
        {
            Destroy(child.gameObject);
        }
        upgradePanel.SetActive(false);
        upgradePanelDescText.SetActive(false);
        // timeScale은 OnUpgradeSelected에서 큐가 비었을 때만 복구합니다.
    }

    public void RequestUpgrade()
    { 
        upgradeQueue.Enqueue(true);
        TryShowUpgrade();
    }

    private void TryShowUpgrade()
    {
        if (isUpgradeShowing || upgradeQueue.Count == 0)
            return;

        isUpgradeShowing = true;

        var availableUpgrades = allUpgrades
            .Where(data => !PlayerStatus.Instance.IsUpgradeMaxed(data.upgradeType))
            .ToList();

        if (availableUpgrades.Count == 0)
        {
            upgradeQueue.Clear();
            isUpgradeShowing = false;
            Time.timeScale = 1f;
            Debug.Log("선택 가능한 강화가 없어 강화 UI를 건너뜁니다.");
            return;
        }

        var selectedOptions = availableUpgrades
            .OrderBy(x => Random.value)
            .Take(3)
            .ToList();

        ShowUpgradeOptions(selectedOptions);

        Debug.Log("업그레이드 UI 표시됨");
    }

    public void OnUpgradeSelected()
    {
        if (upgradeQueue.Count > 0)
            upgradeQueue.Dequeue();

        isUpgradeShowing = false;
        TryShowUpgrade();

        // 대기 중인 강화가 더 없으면 시간 스케일 복구
        if (!IsUpgradeBlocking)
            Time.timeScale = 1f;
    }
    private List<UpgradeOption> GetRandomUpgradeOptions()
    {
        List<UpgradeOption> available = new List<UpgradeOption>();

        foreach (var option in allUpgrades)
        {
            // 등장 확률 체크 + 최대 수치 미달 조건
            if ((option.spawnChance >= 1f || Random.value <= option.spawnChance) && !HasReachedMax(option))
            {
                available.Add(option);
            }
        }

        return available.OrderBy(x => Random.value).Take(3).ToList(); // 랜덤 3개 선택
    }
    private bool HasReachedMax(UpgradeOption option)
    {
        // 현재 플레이어 스탯에서 최대치 도달 여부 확인
        return PlayerStatus.Instance.IsUpgradeMaxed(option.upgradeType);
    }

    // 업그레이드 슬롯(버튼)을 생성하고 UI에 설정하는 함수
    private void CreateUpgradeSlot(UpgradeOption upgrade)
    {
        GameObject buttonGO = Instantiate(upgradeButtonPrefab, upgradePanel.transform);

        Image icon = buttonGO.transform.Find("Icon").GetComponent<Image>();
        TMP_Text descText = buttonGO.transform.Find("DescText").GetComponent<TMP_Text>();
        Button btn = buttonGO.GetComponent<Button>();

        btn.name = upgrade.upgradeType.ToString();
        icon.sprite = upgrade.icon;
        descText.text = $"{upgrade.description}\n<color=#AAAAAA>{upgrade.GetStatusText()}</color>";

        btn.onClick.AddListener(() =>
        {
            ApplyUpgrade(upgrade);
            OnUpgradeSelected();
        });

        upgradeButtons.Add(btn); // 정리용 리스트에 저장
    }
    // 기존 버튼들 정리
    private void ClearUpgradeButtons()
    {
        foreach (Button btn in upgradeButtons)
        {
            if (btn != null && btn.gameObject != null)
            {
                Destroy(btn.gameObject);
            }
        }
        upgradeButtons.Clear();
    }
}
