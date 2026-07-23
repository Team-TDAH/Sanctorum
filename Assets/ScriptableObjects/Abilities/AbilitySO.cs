using UnityEngine;

//base para las habilidades que creare

public abstract class AbilitySO : ScriptableObject
{
    public string abilityName = "New Skill";
    //para no perderme de como se obtiene la skill
    [TextArea] public string UnlockSkill;
    //icono para los cooldowns
    public Sprite icon;


    //el nombre este debe ser IDENTICO al que ponga en el inputaction
    public string inputActionName = "Ability1";


    public float cooldown = 1f;
    //creo que es importante, asi no se complica mucho el cooldown para habilidades que duren cierto tiempo
    public bool cooldownAfterEnd = false;

    //condiciones para usar, mejor ponerlo aca que en cada uno, asi veo donde usarlo
    public bool usableOnGround = true;
    public bool usableInAir = true;
    public bool usableDuringDash = false;
    //importante para desbloquear habilidades
    public BoolVariable unlockedVariable;

    //canal de eventos de la habilidad, todos deben tener uno propio
    public AbilityChannel channel;
    //en runtime para que se reinicien siempre y no tener bugs futuros, no me adapto bien cuando usarlos
    [System.NonSerialized] public float CooldownRemaining;
    [System.NonSerialized] public bool IsActive;
    //verifica las condiciones que ande pusimos
    public virtual bool CanUse(AbilityContext ctx)
    {
        //importante para verificar que tenes desbloqueada la habilidad (fue mas facil de lo que pense)
        if (unlockedVariable != null && !unlockedVariable.Value) return false;

        if (CooldownRemaining > 0f) return false;
        if (!usableOnGround && ctx.IsGrounded) return false;
        if (!usableInAir && !ctx.IsGrounded) return false;
        if (!usableDuringDash && ctx.IsDashing) return false;
        return true;
    }
    //logica de la habilidad
    public abstract void Execute(AbilityContext ctx);
    //se llama cada tik de habilidad si es que hace falta, ponele el escudo de area o algo estilo veneno, no crreo usarlo tanto
    public virtual void Tick(AbilityContext ctx, float deltaTime) { }
    //llamar cuando la habilidad termina o se interrumpe, re importante

    public virtual void End(AbilityContext ctx)
    {
        IsActive = false;
        if (cooldownAfterEnd)
            CooldownRemaining = cooldown;
    }

    //lo mismo de antes, para resetear y prevenir errores
    public void ResetRuntimeState()
    {
        CooldownRemaining = 0f;
        IsActive = false;
    }
}
//LA MEJOR FUNCIONALODAD DE ESTO, pasa referencias y demas cosas para que la creacion de habilidades sea mas facil (en caso de necesitar mas datos, agregar aca)
public class AbilityContext
{
    public PlayerController Player;
    public Rigidbody2D Rb;
    //agregado al final para que las habilidades puedas acceder a la vida y al shield en caso de tenerlo activado
    public PlayerHealth PlayerHealth;
    public Vector2 MoveInput;
    public float LastFacingDirection;
    public bool IsGrounded;
    public bool IsDashing;
    //punto de disparo compartido para las habilidades
    public Transform WeaponPoint;
    //dirrecion de apuntado, ya "acotada" sacada del playeraim via abilitymanager(termine quitando lo del "acotado")
    public Vector2 AimDirection;
}