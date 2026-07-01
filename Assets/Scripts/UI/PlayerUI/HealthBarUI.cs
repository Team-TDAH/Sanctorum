using UnityEngine;
using UnityEngine.UI;

//script basico y complejo para la vida del player, basico por lo corto y complejo por los llamados
public class HealthBarUI : MonoBehaviour
{
    //con solo estas 2 referencias ya se puede mostrar la vida sin problemas, luego cuando quiera mostrar el cooldown de las habilidades sera mas complejo en otro script
    [SerializeField] private HealthChannel healthChannel;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        if (healthChannel != null)
            healthChannel.OnHealthChanged += HandleHealthChanged;
    }
    private void OnDisable()
    {
        if (healthChannel != null)
            healthChannel.OnHealthChanged -= HandleHealthChanged;
    }
    private void HandleHealthChanged(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}