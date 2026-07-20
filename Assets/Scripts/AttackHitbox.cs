using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public string attackerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (attackerTag == "Player" && other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy == null)
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
}
