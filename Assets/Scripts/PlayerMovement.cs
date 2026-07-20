using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isJumping;

    public Transform groundCheck; // 바닥 감지용 오브젝트 (캐릭터 발 아래) 
    public float groundDistance = 0.3f; // 바닥 체크 거리
    public LayerMask groundMask; // 바닥 레이어

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 바닥 감지 (Raycast 사용)
        bool isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 바닥에 완전히 닿도록 초기화
            isJumping = false; // 점프 종료
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        bool isMoving = moveDirection.magnitude > 0;

        animator.SetBool("isMoving", isMoving);

        if (isMoving)
        {
            transform.forward = moveDirection;
        }

        // 이동 적용
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 점프 처리 (공중에서도 점프 가능하도록 수정)
        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
            isJumping = true; // 점프 상태 유지
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
