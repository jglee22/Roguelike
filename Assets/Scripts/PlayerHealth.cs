using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    private Animator animator;
    private bool isDead = false;
    private PlayerHealthUI healthUI;

    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
        healthUI = FindAnyObjectByType<PlayerHealthUI>();

        healthUI.SetMaxHealth(maxHP);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        Debug.Log($"[플레이어 피격] 현재 체력: {currentHP}");

        // UpperBody 레이어 weight 끄기 (0번이 Base, 1번이 UpperBody일 경우)
        animator.SetLayerWeight(1, 0f);
        animator.SetTrigger("Hit");

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
        // 조작 스크립트 비활성화
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;

        GameManager.Instance.GameOver();
    }

    IEnumerator RecoverUpperBodyLayer()
    {
        yield return new WaitForSeconds(1f); // Hit 애니메이션 길이만큼 대기
        animator.SetLayerWeight(1, 1f); // 다시 원래대로 복구
    }

    public void ResetHealth()
    {

    }
}
