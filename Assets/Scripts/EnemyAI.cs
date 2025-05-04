using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float detectRange = 10f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;
    public int damage = 10;

    private Animator animator;
    private float lastAttackTime;
    [SerializeField]
    private bool isAttacking = false;
    [SerializeField]
    private EnemyHealth enemyHealth;
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

        var data = DifficultyManager.Instance?.GetFloorData(GameManager.Instance.currentFloor);
        if (data != null)
        {
            moveSpeed = data.enemySpeed;
            damage = data.enemyDamage;
        }
    }

    void Update()
    {
        if (enemyHealth.IsDead) return;
        if (target == null || isAttacking) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectRange)
        {
            if (distance > attackRange)
            {
                MoveTowardTarget();
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);

                // 일정 쿨타임이 지난 후에만 공격
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    Attack();
                }
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    void MoveTowardTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void Attack()
    {
        if(enemyHealth.IsDead) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("Attack"); // 애니메이션이 데미지 타이밍 제어

        // 공격이 끝난 후 다시 공격 가능하도록
        StartCoroutine(ResetAttack());
    }
    public void DealDamageToPlayer()
    {
        if (enemyHealth != null && enemyHealth.IsDead) return; // 죽었으면 공격 안 함

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }
    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1f); // 애니메이션 길이 또는 Hit 시점 이후
        if (!enemyHealth.IsDead)
            isAttacking = false;
    }
}
