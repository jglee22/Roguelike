using UnityEngine;

public enum AttackType
{
    Unarmed,    // 맨손 (주먹)
    OneHanded,  // 한손 무기
    TwoHanded,  // 양손 무기
    Magic       // 주문 시전
}

[System.Serializable]
public class Weapon : MonoBehaviour
{
    public string weaponName; // 무기 이름
    public AttackType attackType; // 무기의 공격 유형 (Enum)
    public AnimatorOverrideController weaponAnimator; // 무기별 애니메이션 컨트롤러
}
