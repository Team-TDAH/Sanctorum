using UnityEngine;
using UnityEngine.UI;

//para mostrar en ui las habilidades creadas como SO, por eso el dash esta aparte
public class AbilityCooldownUI : MonoBehaviour
{
    //para verificar si esta desbloqueada la habilidad y mostrar la ui
    [SerializeField] private BoolVariable unlockedVariable;
    //para el cooldown y eso
    [SerializeField] private AbilityChannel channel;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundIMG;
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

        private void Update()
    {
        //no me agrada la solucion, porque agrega un update donde no deberia pero asi es mas facil para verificar todo elr ato si la habilidad esta o no desbloqueada y ver si mostrar la ui o no
        bool unlocked = unlockedVariable == null || unlockedVariable.Value;
        if (fillImage != null)
        {
            fillImage.gameObject.GetComponent<Image>().enabled = unlocked;
        }
        if(backgroundIMG != null)
        {
            backgroundIMG.gameObject.GetComponent<Image>().enabled = unlocked;
        }
            
    }
    //muy parecido al script del dash cooldown
    private void HandleCooldownUpdated(AbilitySO ability, float progress)
    {
        float fill = 1f - progress;

        if (fillImage != null)
        {
            fillImage.GetComponent<Image>().fillAmount = fill;
        }
    }
}