using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;
    private Animator animator;
    private bool isDead = false;

    [Header("이펙트")]
    public GameObject hitEffectPrefab; // <- 이펙트 프리팹 연결
    void Start()
    {
        currentHP = maxHP;
        animator = GetComponent<Animator>();
    }
}
