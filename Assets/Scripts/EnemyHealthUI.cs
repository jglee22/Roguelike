using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider healthSlider;

    public void SetMaxHealth(int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
    }

    public void SetCurrentHealth(int currentHP)
    {
        healthSlider.value = currentHP;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
