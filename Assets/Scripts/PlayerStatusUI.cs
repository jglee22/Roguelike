using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 스탯을 화면에 표시합니다. (검증/플레이용 HUD)
/// Canvas가 있으면 자동으로 텍스트를 생성합니다.
/// </summary>
public class PlayerStatusUI : MonoBehaviour
{
    private const string UiObjectName = "PlayerStatusHUD";
    private const int FontSize = 22;
    private const float Margin = 16f;
    private const float PanelWidth = 320f;
    private const float PanelHeight = 240f;

    [SerializeField] private TextMeshProUGUI statusText;

    public static void EnsureExists()
    {
        if (FindAnyObjectByType<PlayerStatusUI>() != null)
            return;

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[PlayerStatusUI] Canvas를 찾을 수 없어 스탯 UI를 생성하지 않습니다.");
            return;
        }

        GameObject uiRoot = new GameObject(UiObjectName, typeof(RectTransform));
        uiRoot.transform.SetParent(canvas.transform, false);
        uiRoot.AddComponent<PlayerStatusUI>();
    }

    private void Awake()
    {
        if (statusText == null)
            statusText = CreateStatusText();
    }

    private void Update()
    {
        if (statusText == null)
            return;

        if (PlayerStatus.Instance == null)
        {
            statusText.text = "스탯: PlayerStatus 없음";
            return;
        }

        PlayerStatus status = PlayerStatus.Instance;
        int floor = GameManager.Instance != null ? GameManager.Instance.currentFloor : 0;
        int currentHp = status.currentHealth;
        PlayerHealth health = GetPlayerHealth();
        if (health != null)
            currentHp = health.currentHP;

        string regenText = status.isHealthRegen
            ? $"{status.regenAmount:F1} / {status.maxRegenAmount:F1}"
            : "없음";

        statusText.text =
            $"층: {floor}\n" +
            $"레벨: {status.level}\n" +
            $"공격력: {status.attackPower:F0}\n" +
            $"공격속도: {status.attackSpeed:F2}\n" +
            $"체력: {currentHp} / {status.maxHealth}\n" +
            $"체력 재생: {regenText}\n" +
            $"치명타: {status.critChance:F0}%\n" +
            $"속성: {status.currentElement}";
    }

    private PlayerHealth GetPlayerHealth()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
            return GameManager.Instance.player.GetComponent<PlayerHealth>();
        return null;
    }

    private TextMeshProUGUI CreateStatusText()
    {
        RectTransform root = GetComponent<RectTransform>();
        root.anchorMin = new Vector2(0f, 1f);
        root.anchorMax = new Vector2(0f, 1f);
        root.pivot = new Vector2(0f, 1f);
        root.anchoredPosition = new Vector2(Margin, -Margin);
        root.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        Image background = gameObject.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.55f);

        GameObject textObject = new GameObject("StatusText", typeof(RectTransform));
        textObject.transform.SetParent(transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 8f);
        textRect.offsetMax = new Vector2(-12f, -8f);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = FontSize;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.text = "스탯 로딩...";
        text.raycastTarget = false;

        return text;
    }
}
