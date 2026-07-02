using UnityEngine;
using UnityEngine.UI;

//para mostrar en ui las habilidades creadas como SO, por eso el dash esta aparte
public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private AbilityChannel channel;
    [SerializeField] private Image fillImage;
    //tendre que luego agregar un background que aparezca solo cuando la habilidad este desbloqueada, pero como todavia no hice la posibilidad de desbloquearla, dejo asi

    //llamadas a los eventos que cree en el abilitychannel de cada habilidad
    private void OnEnable()
    {
        if (channel != null)
            channel.OnCooldownUpdated += HandleCooldownUpdated;
    }
    private void OnDisable()
    {
        if (channel != null)
            channel.OnCooldownUpdated -= HandleCooldownUpdated;
    }
    //muy parecido al script del dash cooldown
    private void HandleCooldownUpdated(AbilitySO ability, float progress)
    {
        float fill = 1f - progress;

        if (fillImage != null)
            fillImage.fillAmount = fill;
    }
}