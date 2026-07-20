using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    private Animator animator;
    private bool isDead = false;
    private bool isInvincible = false;
    private PlayerHealthUI healthUI;

    public bool IsInvincible => isInvincible;
    public bool IsDead => isDead;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthUI = FindAnyObjectByType<PlayerHealthUI>();
        SyncFromStatus(fullHeal: true);
        EnableControl(true);
    }

    /// <summary>
    /// 스탯 소스(PlayerStatus) 기준으로 최대 체력을 맞춥니다.
    /// </summary>
    public void SyncFromStatus(bool fullHeal)
    {
        if (PlayerStatus.Instance != null)
            maxHP = PlayerStatus.Instance.maxHealth;

        if (fullHeal)
            currentHP = maxHP;
        else
            currentHP = Mathf.Min(currentHP, maxHP);

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.currentHealth = currentHP;

        if (healthUI != null)
        {
            healthUI.SetMaxHealth(maxHP);
            healthUI.SetCurrentHealth(currentHP);
        }
    }

    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible)
            return;

        currentHP -= damage;
        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.currentHealth = currentHP;

        Debug.Log($"[플레이어 피격] 현재 체력: {currentHP}");

        animator.SetLayerWeight(1, 0f);
        animator.SetTrigger("Hit");

        if (healthUI != null)
            healthUI.SetCurrentHealth(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
        else
            StartCoroutine(RecoverUpperBodyLayer());
    }

    void Die()
    {
        isDead = true;
        animator.SetLayerWeight(1, 0f);
        animator.SetTrigger("Death");
        Debug.Log("[플레이어 사망]");
        EnableControl(false);

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }

    IEnumerator RecoverUpperBodyLayer()
    {
        yield return new WaitForSeconds(1f);
        if (!isDead)
            animator.SetLayerWeight(1, 1f);
    }

    public void ResetHealth()
    {
        isDead = false;
        isInvincible = false;
        SyncFromStatus(fullHeal: true);
        EnableControl(true);

        if (animator != null)
        {
            animator.SetLayerWeight(1, 1f);
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Death");
        }
    }

    private void EnableControl(bool enabled)
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = enabled;

        var combat = GetComponent<PlayerCombat>();
        if (combat != null)
            combat.enabled = enabled;
    }
}
