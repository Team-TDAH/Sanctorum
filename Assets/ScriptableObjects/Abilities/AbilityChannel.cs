using System;
using UnityEngine;

//intermediario entre el player y la ui, vfx o audio y cada habilidad debe tener su propio channel
[CreateAssetMenu(fileName = "AbilityChannel", menuName = "Abilities/Ability Channel")]
public class AbilityChannel : ScriptableObject
{
    //cualquier sistema puede suscribirse sin conocer al player
    public event Action<AbilitySO> OnAbilityStarted;
    public event Action<AbilitySO> OnAbilityEnded;
    //progreso del cooldown para la ui en un futuro
    public event Action<AbilitySO, float> OnCooldownUpdated;

    public void RaiseStarted(AbilitySO ability)
    {
        OnAbilityStarted?.Invoke(ability);
    }

    public void RaiseEnded(AbilitySO ability)
    {
        OnAbilityEnded?.Invoke(ability);
    }

    public void RaiseCooldownUpdated(AbilitySO ability, float normalizedProgress)
    {
        OnCooldownUpdated?.Invoke(ability, normalizedProgress);
    }
}