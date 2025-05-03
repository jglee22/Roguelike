using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;

    public void SetMaxHealth(int maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = maxHP;
    }

    public void SetCurrentHealth(int currentHP)
    {
        healthSlider.value = currentHP;

        // 색상 변화를 주고 싶다면 여기에 처리 가능
        // fillImage.color = Color.Lerp(Color.red, Color.green, (float)currentHP / healthSlider.maxValue);
    }
}
