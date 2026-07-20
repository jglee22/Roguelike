using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private const float DeathDestroyFallbackSeconds = 1.5f;

    public int maxHP;
    public bool IsDead => isDead;
    public bool isBoss = false;

    public int currentHP;
    public int GetCurrentHP() => currentHP;

    private EnemyHealthUI healthUI;
    private Animator animator;
    [SerializeField]
    private bool isDead = false;
    private bool enemyCountReduced = false;
    private Collider enemyCollider;
    [Header("이펙트")]
    public GameObject hitEffectPrefab;

    void Start()
    {
        var data = DifficultyManager.Instance.GetFloorData(GameManager.Instance.currentFloor);
        if (data != null)
        {
            maxHP = data.enemyHP;
        }

        currentHP = maxHP;
        healthUI = GetComponentInChildren<EnemyHealthUI>();
        if (healthUI != null)
            healthUI.SetMaxHealth(maxHP);

        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        float rand = Random.value;
        if (PlayerStatus.Instance != null && rand < PlayerStatus.Instance.critChance * 0.01f)
        {
            damage *= 2;
        }

        currentHP -= damage;
        if (isBoss)
        {
            if (BossUIManager.Instance != null)
                BossUIManager.Instance.SetCurrentHealth(currentHP);
        }
        else if (healthUI != null)
        {
            healthUI.SetCurrentHealth(currentHP);
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        }

        // 막타에서는 Hit를 건너뛰어 사망이 바로 보이도록 함
        if (currentHP <= 0)
        {
            Die();
            return;
        }

        if (animator != null)
            animator.SetTrigger("Hit");
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (enemyCollider != null)
            enemyCollider.enabled = false;

        ReduceEnemyCount();

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Death");
        }

        FindAnyObjectByType<PlayerStatus>()?.GainXP(10);

        StartCoroutine(ForceDestroy());
    }

    /// <summary>
    /// Death 애니메이션 이벤트로 호출. 애니 종료 시 즉시 제거.
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        if (!isDead) return;

        ReduceEnemyCount();
        Destroy(gameObject);
    }

    IEnumerator ForceDestroy()
    {
        yield return new WaitForSeconds(DeathDestroyFallbackSeconds);
        if (this != null && gameObject != null)
        {
            ReduceEnemyCount();
            Destroy(gameObject);
        }
    }

    private void ReduceEnemyCount()
    {
        if (enemyCountReduced)
            return;

        EnemyManager.EnemyCount--;
        enemyCountReduced = true;
    }
}
