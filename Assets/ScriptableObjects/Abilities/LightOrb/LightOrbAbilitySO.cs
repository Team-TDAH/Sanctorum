using UnityEngine;

//habilidad de ataque basico, la cree como una habilidad para no agregar demasiadas cosas al playercontroller y dejar solo el movimiento en el mismo.
//la habilidad dispara una bola de luz en la direccion donde este mirando el player, ya vere si quiero que sea siempre horizontal o poder elegir arriba o diagonal tambien

[CreateAssetMenu(fileName = "LightOrbAbility", menuName = "Abilities/Light Orb")]
public class LightOrbAbilitySO : AbilitySO
{
    //prefab de la bola de luz
    public GameObject orbPrefab;
    public float orbSpeed = 14f;
    public float orbLifetime = 3f;
    public int damage = 10;

    //offset para que no salga desde el centro del player, sino desde un baston
    public Vector2 spawnOffset = new Vector2(0.5f, 0.2f);
    //si no hay input direccional, dispara hacia donde mira el jugador
    public bool aimWithInput = true;


    public override void Execute(AbilityContext ctx)
    {
        if (orbPrefab == null)
        {
            Debug.LogWarning("te olvidaste asignar el prefab del orbe");
            ctx.Player.AbilityManager.EndAbility(this);
            return;
        }
        //calc direccion donde mira
        Vector2 fireDirection = GetFireDirection(ctx);

        //ahora si, usamos el punto del arma y no el off set que tenia que ajustar cada que cambiaba algo
        Vector2 spawnPos = ctx.WeaponPoint != null
            ? (Vector2)ctx.WeaponPoint.position
            : (Vector2)ctx.Player.transform.position;

        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

        var projectile = orb.GetComponent<LightOrbProjectile>();
        if (projectile != null)
            projectile.Initialize(fireDirection, orbSpeed, orbLifetime, damage);

        ctx.Player.AbilityManager.EndAbility(this);
    }

    private Vector2 GetFireDirection(AbilityContext ctx)
    {
        if (aimWithInput && ctx.MoveInput.sqrMagnitude > 0.01f)
            return ctx.MoveInput.normalized;

        //si no hay input, dispara horizontal hacia donde mira el jugador
        return new Vector2(ctx.LastFacingDirection, 0f);
    }
}