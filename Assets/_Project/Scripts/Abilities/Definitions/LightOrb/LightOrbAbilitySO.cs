using UnityEngine;

//habilidad de ataque basico, la cree como una habilidad para no agregar demasiadas cosas al playercontroller y dejar solo el movimiento en el mismo.
//dispara en la direccion que da PlayerAim, clampeada a un arco alrededor de hacia donde mira el player

[CreateAssetMenu(fileName = "LightOrbAbility", menuName = "Abilities/Light Orb")]
public class LightOrbAbilitySO : AbilitySO
{
    //prefab de la bola
    public GameObject orbPrefab;
    public float orbSpeed = 14f;
    public float orbLifetime = 3f;
    public int damage = 10;


    public override void Execute(AbilityContext ctx)
    {
        if (orbPrefab == null)
        {
            Debug.LogWarning("te olvidaste asignar el prefab del orbe");
            ctx.Player.AbilityManager.EndAbility(this);
            return;
        }

        //el apuntado viene dle mouse ahora(tambien saque la limitacion de angulo para disparar, sentia tosco)
        Vector2 fireDirection = ctx.AimDirection.sqrMagnitude > 0.01f
            ? ctx.AimDirection
            : new Vector2(ctx.LastFacingDirection, 0f);

        //usamos el punto del arma y no un offset a mano
        Vector2 spawnPos = ctx.WeaponPoint != null
            ? (Vector2)ctx.WeaponPoint.position
            : (Vector2)ctx.Player.transform.position;

        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);

        var projectile = orb.GetComponent<LightOrbProjectile>();
        if (projectile != null)
            projectile.Initialize(fireDirection, orbSpeed, orbLifetime, damage);

        ctx.Player.AbilityManager.EndAbility(this);
    }
}