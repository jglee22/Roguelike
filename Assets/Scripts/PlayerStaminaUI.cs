using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    public Slider staminaSlider;

    void Update()
    {   

        staminaSlider.maxValue = PlayerStatus.Instance.maxStamina;
        staminaSlider.value = PlayerStatus.Instance.currentStamina;
    }
}
