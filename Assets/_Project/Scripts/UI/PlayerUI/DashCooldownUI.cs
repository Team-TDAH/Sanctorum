using UnityEngine;
using UnityEngine.UI;

//se me complico la vida queriendo poner un background al dash, para que quede mejor, y visible pero daba errores en itchio
public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BoolVariable dashUnlocked;

    //usar Slider o Image con fillAmount, no ambos a la vez(quite el slider, no creo usarlo)
    [SerializeField] private Image fillImage;
    [SerializeField] private Image fillImageBackground;

    private void Update()
    {
        bool unlocked = dashUnlocked == null || dashUnlocked.Value;

        //ocultamos solo el visual, no el gameobject, ya me dio varios errores en itchio

        if (fillImage != null)
            fillImage.enabled = unlocked;
    
        if (fillImageBackground != null)
            fillImageBackground.enabled = unlocked;

        if (!unlocked) return;

        float progress = playerController != null ? playerController.DashCooldownProgress : 1f;

        if (fillImage != null)
            fillImage.fillAmount = progress;
    }
}