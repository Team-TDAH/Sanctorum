using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityEventChannelSO", menuName = "Events/Ability Event Channel")]
public class AbilityEventChannelSO : ScriptableObject
{
    // Evento que se disparará cuando una habilidad se active con éxito
    public event Action<AbilitySO, GameObject> OnAbilityActivated;

    public void RaiseEvent(AbilitySO ability, GameObject caster)
    {
        if (OnAbilityActivated != null)
        {
            OnAbilityActivated.Invoke(ability, caster);
        }
    }
}
