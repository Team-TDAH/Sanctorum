using UnityEngine;
using UnityEngine.UI;

//script para mostrar  el cooldown del dash en la UI, ya sea slider o imagen con fillAmount
public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    //usar Slider o Image con fillAmount, no ambos a la vez
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        float progress = playerController != null ? playerController.DashCooldownProgress : 1f;

        //era por si usaba un slider sobre el player, pero no creo usarlo
        if (slider != null)
            slider.value = progress;

        //todavia no se si se hara una visualizacion del cooldown del dash, pero me queda para futuros cooldowns esta "plantilla"
        if (fillImage != null)
            fillImage.fillAmount = progress;
    }
}