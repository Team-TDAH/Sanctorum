using System;
using UnityEngine;

//conexion de eventos entre PlayerHealth y la ui, animaciones y demas por suscripciones, sin necesidad de que conozcan HealthChannel

[CreateAssetMenu(fileName = "HealthChannel", menuName = "Health/Health Channel")]
public class HealthChannel : ScriptableObject
{
    //vida actual y vida maxima para que la ui pueda calcular el porcentaje (quizas luego lo calculo aca mismo, pero no seria correcto)
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    public void RaiseHealthChanged(int current, int max)
    {
        OnHealthChanged?.Invoke(current, max);
    }

    public void RaiseDeath()
    {
        OnDeath?.Invoke();
    }
}