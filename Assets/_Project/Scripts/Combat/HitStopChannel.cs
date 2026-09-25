using System;
using UnityEngine;
//canal para pedir un hitstop, el proyectil lo dispara y el manager lo escucha 
[CreateAssetMenu(fileName = "HitStopChannel", menuName = "Feedback/Hit Stop Channel")]
public class HitStopChannel : ScriptableObject
{
    //la duracion la alije quien la pide
    public event Action<float> OnHitStopRequested;
    public void RequestHitStop(float duration = 0f)
    {
        OnHitStopRequested?.Invoke(duration);
    }
}