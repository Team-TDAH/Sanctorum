using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO")]
public abstract class AbilitySO : ScriptableObject
{
    public string abilityName;
    public float cooldownTime;
    public float activeTime;
    public Sprite icon;

    // Método abstracto que cada habilidad implementará a su manera
    public abstract void Activate(GameObject caster);
}
