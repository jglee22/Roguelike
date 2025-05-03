using UnityEngine;

public class TestAttackAnimations : MonoBehaviour
{
    public Animator animator;
    public AttackType testType = AttackType.Unarmed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 왼공격
        {
            PlayAttack("Left");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 오른공격
        {
            PlayAttack("Right");
        }
    }

    void PlayAttack(string side)
    {
        switch (testType)
        {
            case AttackType.Unarmed:
                animator.SetTrigger($"{side}Punch");
                break;
            case AttackType.OneHanded:
                animator.SetTrigger($"OneHandedAttack");
                break;
            case AttackType.TwoHanded:
                animator.SetTrigger($"TwoHandedAttack");
                break;
            case AttackType.Magic:
                animator.SetTrigger("MagicAttack"); // side 무시
                break;
        }

        Debug.Log($"[테스트] {testType} - {side} 공격 애니메이션 실행됨");
    }
}
