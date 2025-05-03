using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public string attackerTag = "Player"; // 누가 공격하는지 구분 (플레이어 or 몬스터)

    private void OnTriggerEnter(Collider other)
    {
        // 적인 경우에만 데미지 처리
        if (attackerTag == "Player" && other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Debug.Log("Enemy Hit");
        }

        // 추후 몬스터 공격용 처리도 추가 가능
        // if (attackerTag == "Enemy" && other.CompareTag("Player")) { ... }
    }
}
