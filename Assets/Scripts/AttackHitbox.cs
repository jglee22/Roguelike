using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public string attackerTag = "Player";

    private readonly HashSet<int> hitEnemyIdsThisSwing = new();

    /// <summary>히트박스 활성화 직전에 호출해 이번 스윙의 히트 기록을 초기화합니다.</summary>
    public void BeginSwing()
    {
        hitEnemyIdsThisSwing.Clear();
    }

    private void OnDisable()
    {
        hitEnemyIdsThisSwing.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (attackerTag != "Player" || !other.CompareTag("Enemy"))
            return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null)
            enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null || enemy.IsDead)
            return;

        int enemyId = enemy.GetInstanceID();
        if (!hitEnemyIdsThisSwing.Add(enemyId))
            return;

        if (PlayerStatus.Instance == null)
        {
            Debug.LogError("[AttackHitbox] PlayerStatus.Instance가 없습니다.");
            return;
        }

        int damage = Mathf.RoundToInt(PlayerStatus.Instance.attackPower);
        enemy.TakeDamage(damage);
        Debug.Log($"Enemy Hit (damage: {damage})");
    }
}
