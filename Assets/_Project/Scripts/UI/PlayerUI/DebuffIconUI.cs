using UnityEngine;
using TMPro;
//la idea es poder reutilizar esto para los buff y debuff, pero no creo que sea posible, ya vere cuando vaya metiendo mas habilidades
public class DebuffIconUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    //lo que se activa y desactiva para ver el contador
    [SerializeField] private GameObject iconContainer;
    [SerializeField] private TMP_Text timerText;
    private void Start()
    {
        //POR LAS DUDAS, cuando este temrinado el game podria borrarlo
        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (iconContainer != null)
            iconContainer.SetActive(false);
    }
    private void Update()
    {
        if (playerHealth == null || iconContainer == null) return;

        bool active = playerHealth.HasDamageDebuff;
        iconContainer.SetActive(active);

        if (!active) return;

        //redondeo hacia arriba, no me gustaria q se vea el 0 en el contador
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(playerHealth.DebuffTimeRemaining).ToString();
    }
}