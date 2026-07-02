using UnityEngine;
using UnityEngine.UI;

//script para mostrar  el cooldown del dash en la UI, ya sea slider o imagen con fillAmount
public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BoolVariable dashUnlocked;

    //usar Slider o Image con fillAmount, no ambos a la vez
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private void Update()
    {
        //para que cuando el dash se habilite, aparezca la ui del cooldown
    bool unlocked = dashUnlocked == null || dashUnlocked.Value;
    if (gameObject.transform.parent != null)
    {
        gameObject.transform.parent.gameObject.GetComponent<Image>().enabled = unlocked;
    }

    gameObject.GetComponent<Image>().enabled = unlocked;

    if (!unlocked) return;

        float progress = playerController != null ? playerController.DashCooldownProgress : 1f;

        //era por si usaba un slider sobre el player, pero no creo usarlo
        if (slider != null)
            slider.value = progress;

        //todavia no se si se hara una visualizacion del cooldown del dash, pero me queda para futuros cooldowns esta "plantilla"
        if (fillImage != null)
            fillImage.fillAmount = progress;
    }
}