using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Barra de vida del jefe. Va en el HUD, el panel empieza desactivado.
/// Aparece cuando arranca una pelea de jefe y se esconde al derrotarlo.
/// Solo escucha el canal, no conoce a ningun jefe en particular.
/// </summary>
public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private BossHealthChannel bossChannel;

    //contenedor de la barra, empieza desactivado
    [SerializeField] private GameObject bossBarPanel;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text bossNameText;


    private void OnEnable()
    {
        if (bossChannel == null) return;

        bossChannel.OnBossFightStarted += HandleFightStarted;
        bossChannel.OnBossHealthChanged += HandleHealthChanged;
        bossChannel.OnBossDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        if (bossChannel == null) return;

        bossChannel.OnBossFightStarted -= HandleFightStarted;
        bossChannel.OnBossHealthChanged -= HandleHealthChanged;
        bossChannel.OnBossDefeated -= HandleDefeated;
    }

    private void HandleFightStarted(string bossName, int maxHealth)
    {
        if (bossBarPanel != null) bossBarPanel.SetActive(true);
        if (bossNameText != null) bossNameText.text = bossName;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (slider == null) return;

        slider.maxValue = max;
        slider.value = current;
    }

    private void HandleDefeated()
    {
        if (bossBarPanel != null) bossBarPanel.SetActive(false);
    }
}