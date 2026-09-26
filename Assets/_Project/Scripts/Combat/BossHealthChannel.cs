using System;
using UnityEngine;
//parecido al de habilidades, es para que la ui escuche los eventos, la idea es que un solo canal para TODOS los jefes
[CreateAssetMenu(fileName = "BossHealthChannel", menuName = "Health/Boss Health Channel")]
public class BossHealthChannel : ScriptableObject
{
    //el jefe avisa que empieza la pelea, la UI muestra la barra con su nombre
    public event Action<string, int> OnBossFightStarted;
    //vida actual y maxima, para que la UI actualice la barra
    public event Action<int, int> OnBossHealthChanged;
    //el jefe murio, la UI esconde la barra
    public event Action OnBossDefeated;

    public void RaiseFightStarted(string bossName, int maxHealth)
    {
        OnBossFightStarted?.Invoke(bossName, maxHealth);
    }

    public void RaiseHealthChanged(int current, int max)
    {
        OnBossHealthChanged?.Invoke(current, max);
    }

    public void RaiseDefeated()
    {
        OnBossDefeated?.Invoke();
    }
}