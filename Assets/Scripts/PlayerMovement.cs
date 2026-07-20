using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float DefaultDodgeStaminaCost = 20f;

    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("회피")]
    [SerializeField] private KeyCode dodgeKey = KeyCode.LeftShift;
    [SerializeField] private float dodgeDistance = 4f;
    [SerializeField] private float dodgeDuration = 0.25f;
    [SerializeField] private float dodgeCooldown = 0.6f;
    [SerializeField] private float invincibleDuration = 0.3f;
    [SerializeField] private float dodgeStaminaCost = DefaultDodgeStaminaCost;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isJumping;
    private bool isDodging;
    private float lastDodgeTime = -999f;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    public bool IsDodging => isDodging;

    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        if (UpgradeManager.Instance != null && UpgradeManager.Instance.IsUpgradeBlocking)
        {
            if (animator != null)
                animator.SetBool("isMoving", false);
            return;
        }

        if (isDodging)
            return;

        bool isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        bool isMoving = moveDirection.magnitude > 0;

        animator.SetBool("isMoving", isMoving);

        if (isMoving)
            transform.forward = moveDirection;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
            isJumping = true;
        }

        if (Input.GetKeyDown(dodgeKey))
            TryDodge(moveDirection);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void TryDodge(Vector3 moveDirection)
    {
        if (Time.time - lastDodgeTime < dodgeCooldown)
            return;

        if (PlayerStatus.Instance == null)
            return;

        if (PlayerStatus.Instance.currentStamina < dodgeStaminaCost)
        {
            Debug.Log("스태미나 부족으로 회피 불가!");
            return;
        }

        if (playerHealth != null && playerHealth.IsDead)
            return;

        Vector3 dodgeDir = moveDirection.sqrMagnitude > 0.01f
            ? moveDirection.normalized
            : transform.forward;

        dodgeDir.y = 0f;
        if (dodgeDir.sqrMagnitude < 0.01f)
            dodgeDir = transform.forward;

        PlayerStatus.Instance.currentStamina -= dodgeStaminaCost;
        PlayerStatus.Instance.lastStaminaUseTime = Time.time;
        lastDodgeTime = Time.time;

        StartCoroutine(DodgeRoutine(dodgeDir.normalized));
    }

    private IEnumerator DodgeRoutine(Vector3 direction)
    {
        isDodging = true;
        animator.SetBool("isMoving", false);

        if (playerCombat != null)
            playerCombat.ResetAttackState();

        if (playerHealth != null)
            playerHealth.SetInvincible(true);

        transform.forward = direction;

        float elapsed = 0f;
        float speed = dodgeDistance / Mathf.Max(dodgeDuration, 0.01f);

        while (elapsed < dodgeDuration)
        {
            controller.Move(direction * speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isDodging = false;

        // 무적 시간이 회피 이동보다 길 수 있음
        float remainingIFrame = invincibleDuration - dodgeDuration;
        if (remainingIFrame > 0f)
            yield return new WaitForSeconds(remainingIFrame);

        if (playerHealth != null)
            playerHealth.SetInvincible(false);
    }
}
