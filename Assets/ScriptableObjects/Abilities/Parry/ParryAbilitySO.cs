using UnityEngine;

//pense que seria mas dificil, sirvieron los tutos
[CreateAssetMenu(fileName = "ParryAbility", menuName = "Abilities/Parry")]
public class ParryAbilitySO : AbilitySO
{
    public float parryWindow = 0.2f;
    //esto de sysmtem.nonserialized evita que recuerde el timer y el hitbox al reiniciar el script, nunca los habia usado pero veo que son muy utiles cuando un SO se activa y desactiva bastante, pero no tener que reiniciar las variables antes de desactivar el script
    [System.NonSerialized] private float windowTimer;
    [System.NonSerialized] private ParryHitbox hitbox;
    public override void Execute(AbilityContext ctx)
    {
        windowTimer = 0f;

        //Con esto busca la HitboxParry que tiene el player
        if (hitbox == null)
            hitbox = ctx.Player.GetComponentInChildren<ParryHitbox>(true);

        if (hitbox == null)
        {
            //prevenir que tenga fallos si no esta la hitbox
            Debug.LogWarning("BUG ACA, NO TENES LA ParryHitbox en el player como hijo");
            ctx.Player.AbilityManager.EndAbility(this);
            return;
        }
        hitbox.Activate(ctx.LastFacingDirection);
    }

    public override void Tick(AbilityContext ctx, float deltaTime)
    {
        windowTimer += deltaTime;
        //simplemente si no toca nada, finaliza y ya
        if (windowTimer >= parryWindow)
        {
            ctx.Player.AbilityManager.EndAbility(this);
        }
    }
    public override void End(AbilityContext ctx)
    {
        if (hitbox != null)
        {
            hitbox.Deactivate();
        }
        base.End(ctx);
    }



    //ACA VAN LOS EFECTOS SI EL PARRY ES EXITOSO (curacion, potenciador de proximo ataque, stun, orbes que persigue, etc)
    public void OnParrySuccess(AbilityContext ctx, Vector2 incomingDirection)
    {
        Debug.Log("Buen parry!");
        ctx.Player.AbilityManager.EndAbility(this);
    }
}