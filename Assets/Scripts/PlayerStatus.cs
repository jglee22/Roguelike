using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Flags]
public enum ElementType
{
    None = 0,
    Fire = 1 << 0,     // 1
    Ice = 1 << 1,      // 2
    Poison = 1 << 2,   // 4
    Electric = 1 << 3  // 8 등등 필요 시 추가
}

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance;
    [Header("공격 관련 변수")]
    public float attackPower = 10f; // 데미지
    public float attackSpeed = 1f; // 공격 속도
    public float maxAttackSpeed = 2f;
    [Header("체력 관련 변수")]
    public int maxHealth = 100; // 최대 체력
    public int currentHealth; // 현재 체력
    [Header("스태미나 관련 변수")]
    public float maxStamina = 100f; // 최대 스태미나
    public float currentStamina = 100f; // 현재 스태미나
    public float staminaRegenRate = 20f; // 스태미나 회복 시간
    public float staminaRegenDelay = 1.5f; // 스태미나 회복 딜레이
    public float lastStaminaUseTime; // 마지막으로 스태미나 사용한 시간
    [Header("레벨 관련 변수")]
    // 경험치 / 레벨 관련 변수
    public int level = 1;
    public int currentXP = 0;
    public int requiredXP = 100;

    [Header("체력 재생")]
    public bool isHealthRegen = false;
    public float regenAmount = 0f;
    public float maxRegenAmount = 2f;
    public float regenInterval = 3f;
    private float regenTimer = 0f;

    [Header("크리티컬")]
    public float critChance = 0f;       // 0~50%
    public float critMultiplier = 2f;   // 데미지 배수
    public float maxCriticalChance = 50f; // 최대 크리티컬 확률

    private float baseAttackSpeed = 1f;
    private float baseAttackPower = 10f;
    private int baseMaxHP = 100;

    public Slider expSlider;

    public ElementType currentElement = ElementType.None;

    // 속성별 파티클 프리팹 (인스펙터에서 연결)
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private GameObject iceEffectPrefab;
    // 파티클을 붙일 위치 (예: 주먹 위치의 히트박스)
    [SerializeField] private GameObject leftHitbox;
    [SerializeField] private GameObject rightHitbox;
    // 현재 손에 붙어 있는 속성 파티클 관리용
    private Dictionary<ElementType, GameObject> leftElementEffects = new();
    private Dictionary<ElementType, GameObject> rightElementEffects = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        attackSpeed = GameManager.Instance.savedAttackSpeed;
        attackPower = GameManager.Instance.savedAttackPower;
        maxHealth = GameManager.Instance.savedMaxHP;

        currentHealth = maxHealth;
        expSlider = GameObject.Find("ExpBar").GetComponent<Slider>();
    }

    public void UpgradeAttackPower() => attackPower += 3f;
    public void UpgradeAttackSpeed()
    {
        attackSpeed += 0.2f;
        attackSpeed = Mathf.Min(attackSpeed, 2f); //공격속도 상한선 설정
    }
    public void UpgradeMaxHealth()
    {
        maxHealth += 20;
        currentHealth = maxHealth;
    }
    public bool CanUpgradeAttackSpeed() => attackSpeed < maxAttackSpeed;
    public bool CanUpgradeHealthRegen() => regenAmount < maxHealth || !isHealthRegen;

    public bool CanUpgradeCritChance() => critChance < maxCriticalChance;


    public void ResetStats()
    {
        attackSpeed = baseAttackSpeed;
        attackPower = baseAttackPower;
        maxHealth = baseMaxHP;
        currentHealth = maxHealth;

        // GameManager에 저장된 값도 초기화
        GameManager.Instance.savedAttackSpeed = baseAttackSpeed;
        GameManager.Instance.savedAttackPower = baseAttackPower;
        GameManager.Instance.savedMaxHP = baseMaxHP;

        level = 1;
        currentXP = 0;
        requiredXP = 100;
    }
    void Update()
    {
        // 스태미나 회복 타이밍
        if (Time.time - lastStaminaUseTime >= staminaRegenDelay)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        UpdateXPUI(); // 추가: 경험치 UI 업데이트

        if (isHealthRegen && currentHealth > 0 && currentHealth < maxHealth)
        {
            regenTimer += Time.deltaTime;
            if (regenTimer >= regenInterval)
            {
                currentHealth += Mathf.RoundToInt(regenAmount);
                currentHealth = Mathf.Min(currentHealth, maxHealth);
                regenTimer = 0f;
            }
        }
    }

    // 경험치 획득
    public void GainXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= requiredXP)
        {
            LevelUp();
        }
    }

    // 레벨업
    private void LevelUp()
    {
        level++;
        currentXP -= requiredXP;
        requiredXP = Mathf.RoundToInt(requiredXP * 1.2f); // 난이도 점점 증가

        FindAnyObjectByType<UpgradeManager>().RequestUpgrade();

        Debug.Log($"레벨업! 현재 레벨: {level}");
    }

    private void UpdateXPUI()
    {
        if(expSlider == null)
            expSlider = GameObject.Find("ExpBar").GetComponent<Slider>();

        if (expSlider != null)
        {
            expSlider.maxValue = requiredXP;
            expSlider.value = currentXP;
        }
    }
    public void UpgradeCritChance()
    {
        critChance += 10f;
        critChance = Mathf.Min(critChance, maxCriticalChance); // 최대 100%
    }

    public void UpgradeHealthRegen()
    {
        PlayerHealth playerHealth = GameObject.Find("Player")?.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            isHealthRegen = true;
            regenAmount += 0.2f;
            regenAmount = Mathf.Min(regenAmount, 1f);
        }
    }
    public bool IsUpgradeMaxed(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.CritChance:
                return critChance >= maxCriticalChance;
            case UpgradeType.HealthRegen:
                return regenAmount >= maxRegenAmount;
            case UpgradeType.AttackSpeed:
                return attackSpeed >= maxAttackSpeed;
            // 필요 시 다른 항목도 추가
            case UpgradeType.ElementalFire:
                return (currentElement & ElementType.Fire) != 0; // 이미 화염 속성 있음
            case UpgradeType.ElementalIce:
                return (currentElement & ElementType.Ice) != 0; // 이미 냉기 속성 있음
            default:
                return false;
        }
    }

    /// <summary>
    /// 선택한 속성을 플레이어에게 부여하고 손에 파티클 이펙트 생성.
    /// 이미 적용된 속성은 무시됩니다.
    /// </summary>
    public void ApplyElement(ElementType newElement)
    {
        // 중복 속성은 무시
        Debug.Log($"currentElement: {currentElement}, newElement: {newElement}");

        if ((currentElement & newElement) != 0)
        {
            Debug.Log("속성 중복: 스킵");
            return;
        }
        Debug.Log($"ApplyElement 2");
        currentElement |= newElement; // 속성 추가 (플래그 누적)

        GameObject effectPrefab = GetEffectPrefab(newElement);
        if (effectPrefab == null) return;
        Debug.Log($"ApplyElement 3");
        // 왼손, 오른손 각각 파티클 생성 및 부착
        GameObject left = Instantiate(effectPrefab, leftHitbox.transform);
        GameObject right = Instantiate(effectPrefab, rightHitbox.transform);
        Debug.Log($"left : {left.name}");
        leftElementEffects[newElement] = left;
        rightElementEffects[newElement] = right;
    }

    /// <summary>
    /// 속성 타입에 따른 파티클 프리팹 반환
    /// </summary>
    private GameObject GetEffectPrefab(ElementType type)
    {
        switch (type)
        {
            case ElementType.Fire: return fireEffectPrefab;
            case ElementType.Ice: return iceEffectPrefab;
            // 추가 속성도 여기서 계속 확장 가능
            default: return null;
        }
    }
    public void InitializeElementTargets(Transform left, Transform right)
    {
        leftHitbox = left.gameObject;
        rightHitbox = right.gameObject;
    }
}
