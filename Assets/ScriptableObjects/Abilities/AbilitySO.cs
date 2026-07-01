using UnityEngine;

//base para las habilidades que creare

public abstract class AbilitySO : ScriptableObject
{
    [Header("Identificacion")]
    public string abilityName = "Nueva Habilidad";
    [TextArea] public string description;
    public Sprite icon;

    [Header("Input")]
    //debe coincidir exactamente con el nombre de la accion en el InputAction
    public string inputActionName = "Ability1";

    [Header("Cooldown")]
    public float cooldown = 1f;
    //para ver desde cuando quiero que inicie el cooldown, si desde que lanza la habilidad o desde que termina
    public bool cooldownAfterEnd = false;

    [Header("Condiciones de uso")]
    public bool usableOnGround = true;
    public bool usableInAir = true;
    public bool usableDuringDash = false;

    [Header("Canal de eventos")]
    public AbilityChannel channel;

    [System.NonSerialized] public float CooldownRemaining;
    [System.NonSerialized] public bool IsActive;

    //verificacion de condiciones basicas, entre ellas que este en el suelo o aire, y luego pondria alguna condicion de mana por ejemplo
    public virtual bool CanUse(AbilityContext ctx)
    {
        if (CooldownRemaining > 0f) return false;
        if (!usableOnGround && ctx.IsGrounded) return false;
        if (!usableInAir && !ctx.IsGrounded) return false;
        if (!usableDuringDash && ctx.IsDashing) return false;
        return true;
    }

    //logica principal de la habilidad
    public abstract void Execute(AbilityContext ctx);

    //llamado cada frame mientras la habilidad esta activa (si es que hace falta)
    public virtual void Tick(AbilityContext ctx, float deltaTime) { }

    //llamado cuando la habilidad termina por duracion o interrupcion
    public virtual void End(AbilityContext ctx)
    {
        IsActive = false;
        if (cooldownAfterEnd)
            CooldownRemaining = cooldown;
    }

    //necesario para no arrastrar estado de la sesion anterior en el manager
    public void ResetRuntimeState()
    {
        CooldownRemaining = 0f;
        IsActive = false;
    }
}

//datos que le pasá el manager a la habilidad, que puedo expandir en caso de necesitar mas datos del player
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
}