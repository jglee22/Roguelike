using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
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
    public GameObject hitEffectPrefab; // <- 이펙트 프리팹 연결

  
    void Start()
    {
        // 엑셀 기반 난이도 데이터 적용
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
        healthUI = GetComponentInChildren<EnemyHealthUI>();
        enemyCollider = GetComponent<Collider>();
    }

    public void TakeDamage(int damage)
    {
        float rand = Random.value;
        if (rand < PlayerStatus.Instance.critChance * 0.01f)
        {
            damage *= 2; // 크리티컬
        }

        currentHP -= damage;
        if (isBoss)
        {
            BossUIManager.Instance.SetCurrentHealth(currentHP);
        }
        else if (healthUI != null)
        {
            healthUI.SetCurrentHealth(currentHP);
        }
        animator.SetTrigger("Hit");

        // 피격 이펙트 출력
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        enemyCollider.enabled = false;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
        animator.SetTrigger("Death");

        FindAnyObjectByType<PlayerStatus>()?.GainXP(10); // 적당히 10XP 주는 식

        // 애니메이션 이벤트가 유실될 경우를 대비해 안전장치로도 파괴
        StartCoroutine(ForceDestroy());
    }

    /// <summary>
    /// Death 애니메이션 종료 후 2초 뒤에 오브젝트 제거
    /// 애니메이션 이벤트로 호출
    /// </summary>
    public void OnDeathAnimationEnd()
    {
        // 일정 시간 뒤 오브젝트 제거 (애니메이션 재생 보장)
        if (!isDead) return; // 죽은 적이 아닌 경우 무시

        if (!enemyCountReduced)
        {
            EnemyManager.EnemyCount--;
            enemyCountReduced = true;
        }

        Destroy(gameObject, 2f);
    }

    IEnumerator ForceDestroy()
    {  
        yield return new WaitForSeconds(3.5f); // 죽는 애니메이션 길이에 맞춰 (예: 3.5초)
        if (this != null && gameObject.activeInHierarchy)
        {
            if (!enemyCountReduced)
            {
                EnemyManager.EnemyCount--;
                enemyCountReduced = true;
            }

            Destroy(gameObject);
        }
    }
}
