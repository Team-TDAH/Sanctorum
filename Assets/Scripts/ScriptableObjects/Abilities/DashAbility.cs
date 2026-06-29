using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash")]
public class DashAbility : AbilitySO
{
    public float dashForce;

    public override void Activate(GameObject caster)
    {
        Rigidbody2D rb = caster.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Detectar direcci�n basada en la escala o el movimiento del caster
            Vector2 dashDirection = new Vector2(caster.transform.localScale.x, 0).normalized;

            // Aplicar velocidad limpia ignorando las fuerzas previas
            rb.linearVelocity = dashDirection * dashForce;
        }
    }
}