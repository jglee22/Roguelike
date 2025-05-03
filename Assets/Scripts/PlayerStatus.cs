using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance;
    [Header("공격 관련 변수")]
    public float attackPower = 10f; // 데미지
    public float attackSpeed = 1.0f; // 공격 속도
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
    public float regenAmount = 2f;
    public float regenInterval = 3f;
    private float regenTimer = 0f;

    [Header("크리티컬")]
    public float critChance = 0f;       // 0~100%
    public float critMultiplier = 2f;   // 데미지 배수

    private float baseAttackSpeed = 1.0f;
    private float baseAttackPower = 10f;
    private int baseMaxHP = 100;

    public Slider expSlider;

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

        //// 능력치 증가
        //attackPower += 2f;
        //attackSpeed += 0.05f;
        //maxHealth += 10;
        //currentHealth = maxHealth; // 레벨업 시 체력 회복

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
        critChance = Mathf.Min(critChance, 100f); // 최대 100%
    }

    public void UpgradeHealthRegen()
    {
        PlayerHealth playerHealth = GameObject.Find("Player")?.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (!isHealthRegen)
            {
                isHealthRegen = true;
            }
            else
            {
                regenAmount += 0.2f;
                regenAmount = Mathf.Min(regenAmount, 1f);
            }
        }
    }
}
