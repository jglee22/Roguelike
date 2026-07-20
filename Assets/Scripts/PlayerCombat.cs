using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    private const float DefaultStaminaCost = 25f;

    private Animator animator;
    private bool canAttack = true;
    private bool comboQueued = false;
    private int comboIndex = 0; // 0: Left, 1: Right
    private float lastAttackTime = 0f;

    [SerializeField] private float comboTimeLimit = 1.2f;

    public WeaponData currentWeapon;
    public AnimatorOverrideController defaultAnimator;

    [SerializeField] private GameObject leftHitbox;
    [SerializeField] private GameObject rightHitbox;

    private PlayerMovement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        UpdateAnimator();

        if (PlayerStatus.Instance == null)
        {
            Debug.LogError("[PlayerCombat] PlayerStatus.Instance가 없습니다.");
            return;
        }

        animator.SetFloat("AnimSpeed", PlayerStatus.Instance.attackSpeed);

        if (leftHitbox == null || rightHitbox == null)
        {
            Debug.LogError("[PlayerCombat] leftHitbox/rightHitbox가 할당되지 않았습니다.");
            return;
        }

        PlayerStatus.Instance.InitializeElementTargets(leftHitbox.transform, rightHitbox.transform);
    }

    public void EnableLeftHitbox()
    {
        BeginHitboxSwing(leftHitbox);
    }

    public void DisableLeftHitbox()
    {
        SetHitboxEnabled(leftHitbox, false);
    }

    public void EnableRightHitbox()
    {
        BeginHitboxSwing(rightHitbox);
    }

    public void DisableRightHitbox()
    {
        SetHitboxEnabled(rightHitbox, false);
    }

    private void BeginHitboxSwing(GameObject hitboxObject)
    {
        if (hitboxObject == null)
            return;

        AttackHitbox attackHitbox = hitboxObject.GetComponent<AttackHitbox>();
        if (attackHitbox != null)
            attackHitbox.BeginSwing();

        SetHitboxEnabled(hitboxObject, true);
    }

    private void SetHitboxEnabled(GameObject hitboxObject, bool enabled)
    {
        if (hitboxObject == null)
            return;

        Collider col = hitboxObject.GetComponent<Collider>();
        if (col != null)
            col.enabled = enabled;
    }

    void Update()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.IsUpgradeBlocking)
            return;

        if (playerMovement != null && playerMovement.IsDodging)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (canAttack)
                PerformAttack(fromBuffer: false);
            else
                comboQueued = true;
        }
    }

    void PerformAttack(bool fromBuffer)
    {
        if (PlayerStatus.Instance == null)
            return;

        if (PlayerStatus.Instance.currentStamina <= 0)
        {
            Debug.Log("스태미나 부족으로 공격 불가!");
            return;
        }

        // 버퍼로 이어지는 공격은 콤보를 끊지 않음
        if (!fromBuffer && Time.time - lastAttackTime > comboTimeLimit)
            comboIndex = 0;

        PlayerStatus.Instance.currentStamina -= DefaultStaminaCost;
        PlayerStatus.Instance.lastStaminaUseTime = Time.time;

        canAttack = false;
        comboQueued = false;

        if (currentWeapon != null && currentWeapon.attackType != AttackType.Unarmed)
        {
            ExecuteWeaponAttack();
            comboIndex = 0;
        }
        else
        {
            string side = (comboIndex % 2 == 0) ? "Left" : "Right";
            ExecuteUnarmedAttack(side);
            comboIndex = (comboIndex + 1) % 2;
        }

        lastAttackTime = Time.time;
    }

    private void ExecuteUnarmedAttack(string side)
    {
        string triggerName = $"{side}Punch";
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundManager.Instance.punchClip);

        // 이전 트리거가 남아 다음 공격이 먹히지 않는 경우 방지
        animator.ResetTrigger("LeftPunch");
        animator.ResetTrigger("RightPunch");
        animator.SetTrigger(triggerName);
        Debug.Log($"공격 트리거 실행: {triggerName}");
    }

    private void ExecuteWeaponAttack()
    {
        string triggerName = currentWeapon.attackType switch
        {
            AttackType.OneHanded => "OneHandedAttack",
            AttackType.TwoHanded => "TwoHandedAttack",
            AttackType.Magic => "MagicAttack",
            _ => "LeftPunch"
        };

        animator.ResetTrigger("OneHandedAttack");
        animator.ResetTrigger("TwoHandedAttack");
        animator.ResetTrigger("MagicAttack");
        animator.SetTrigger(triggerName);
        Debug.Log($"공격 트리거 실행: {triggerName}");
    }

    public void EnableAttack()
    {
        canAttack = true;

        if (comboQueued)
        {
            comboQueued = false;
            PerformAttack(fromBuffer: true);
            return;
        }

        if (Time.time - lastAttackTime > comboTimeLimit)
            comboIndex = 0;
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateAnimator();
        ResetAttackState();
    }

    public void UnequipWeapon()
    {
        currentWeapon = null;
        UpdateAnimator();
        ResetAttackState();
    }

    private void UpdateAnimator()
    {
        if (currentWeapon != null && currentWeapon.weaponAnimator != null)
            animator.runtimeAnimatorController = currentWeapon.weaponAnimator;
        else
            animator.runtimeAnimatorController = defaultAnimator;
    }

    public void ResetAttackState()
    {
        comboIndex = 0;
        canAttack = true;
        comboQueued = false;
        SetHitboxEnabled(leftHitbox, false);
        SetHitboxEnabled(rightHitbox, false);
    }
}
