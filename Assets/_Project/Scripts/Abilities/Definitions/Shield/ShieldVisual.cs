using UnityEngine;

/// <summary>
/// Componente opcional para el prefab visual del escudo.
/// Si no necesitas logica extra (solo un sprite/particula que aparece y desaparece),
/// no hace falta ni este script, alcanza con el prefab solo.
/// </summary>
public class ShieldVisual : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseScale = 0.1f;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        //pequeno efecto de pulso para que el escudo no se vea estatico
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
        transform.localScale = baseScale * pulse;
    }
}