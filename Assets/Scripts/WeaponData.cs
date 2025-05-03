using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Create New Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public AttackType attackType;
    public AnimatorOverrideController weaponAnimator;
}
