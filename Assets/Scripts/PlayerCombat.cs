using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private bool canAttack = true; // 공격 가능 여부
    private bool isCombo = false; // 콤보 상태
    private bool comboQueued = false; // 다음 공격 예약
    private float lastClickTime = 0f;
    private float comboTimeLimit = 1f; // 콤보 입력 가능 시간

    public WeaponData currentWeapon; // 현재 장착한 무기 (없으면 null)
    public AnimatorOverrideController defaultAnimator; // 기본 애니메이터 (맨손)

    [SerializeField] private GameObject leftHitbox; // 왼손 히트박스 (적 충돌 체크)
    [SerializeField] private GameObject rightHitbox; // 오른손 히트박스

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateAnimator(); // 처음 애니메이터 설정
        leftHitbox.SetActive(false);
        rightHitbox.SetActive(false);
        animator.SetFloat("AnimSpeed", PlayerStatus.Instance.attackSpeed);
    }

    // 왼펀치 애니메이션에 연결
    public void EnableLeftHitbox() => leftHitbox.SetActive(true);
    public void DisableLeftHitbox() => leftHitbox.SetActive(false);

    // 오른펀치 애니메이션에 연결
    public void EnableRightHitbox() => rightHitbox.SetActive(true);
    public void DisableRightHitbox() => rightHitbox.SetActive(false);
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 클릭 감지
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // UI 클릭 중일 때는 무시
                return;
            }

            if (canAttack)
            {
                PerformAttack();
            }
            else
            {
                comboQueued = true; // 애니메이션 도중 추가 입력이 있으면 콤보 예약
            }
        }
    }

    void PerformAttack()
    {
        float staminaCost = 25f;

        if (PlayerStatus.Instance.currentStamina <= 0)
        {
            Debug.Log("스태미나 부족으로 공격 불가!");
            return;
        }

        PlayerStatus.Instance.currentStamina -= staminaCost;
        PlayerStatus.Instance.lastStaminaUseTime = Time.time;

        canAttack = false;

        if (comboQueued || isCombo)
        {
            ExecuteAttack("Right");
            isCombo = false;
            comboQueued = false;
        }
        else
        {
            ExecuteAttack("Left");
            isCombo = true;
        }

        lastClickTime = Time.time;
    }

    void ExecuteAttack(string side)
    {
        string triggerName = "";

        if (currentWeapon != null)
        {
            switch (currentWeapon.attackType)
            {
                case AttackType.Unarmed:
                    triggerName = $"{side}Punch";
                    SoundManager.Instance.PlaySFX(SoundManager.Instance.punchClip);
                    break;
                case AttackType.OneHanded:
                    triggerName = $"OneHandedAttack";
                    isCombo = false; // 콤보 없음
                    comboQueued = false;
                    break;
                case AttackType.TwoHanded:
                    triggerName = $"TwoHandedAttack";
                    isCombo = false; // 콤보 없음
                    comboQueued = false;
                    break;
                case AttackType.Magic:
                    triggerName = "MagicAttack";
                    isCombo = false; // 콤보 없음
                    comboQueued = false;
                    break;
            }
        }
       
        animator.SetTrigger(triggerName);
        Debug.Log($"공격 트리거 실행: {triggerName}");
    }

    // 애니메이션이 끝나면 호출: 공격 가능 상태 복구
    public void EnableAttack()
    {
        canAttack = true;
        if (comboQueued)
        {
            comboQueued = false;
            PerformAttack();
        }
        Debug.Log($"canAttack: {canAttack}, isCombo: {isCombo}, comboQueued: {comboQueued}");
    }

    // 무기를 장착/해제할 때 애니메이터 업데이트
    public void EquipWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateAnimator();
    }

    public void UnequipWeapon()
    {
        currentWeapon = null;
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        if (currentWeapon != null && currentWeapon.weaponAnimator != null)
        {
            animator.runtimeAnimatorController = currentWeapon.weaponAnimator;
        }
        else
        {
            animator.runtimeAnimatorController = defaultAnimator;
        }
    }

    public void ResetAttackState()
    {
        isCombo = false;
        canAttack = true;
        comboQueued = false;
    }
}
