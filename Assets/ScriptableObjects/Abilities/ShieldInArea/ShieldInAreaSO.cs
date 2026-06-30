using UnityEngine;


/// <summary>
/// Escudo temporal: bloquea dano durante un tiempo limitado mientras el jugador
/// se sigue moviendo normalmente. Habilidad defensiva con cooldown largo.
/// Crear via: clic derecho en Project > Abilities > Shield
/// </summary>

[CreateAssetMenu(fileName = "ShieldInAreaSO", menuName = "Abilities/ShieldInAreaSO")]
public class ShieldInAreaSO : AbilitySO
{

    [Header("Escudo")]
    public float shieldDuration = 3f;
    //prefab visual del escudo, debe tener el componente ShieldVisual
    public GameObject shieldPrefab;
 
    //timer interno, NonSerialized porque es estado runtime, no dato del asset
    [System.NonSerialized] private float shieldTimer;
    [System.NonSerialized] private GameObject activeShieldInstance;
 
 
    public override void Execute(AbilityContext ctx)
    {
        shieldTimer = 0f;
 
        //le avisamos al jugador que esta protegido, IsShielded lo chequea quien recibe dano
        ctx.Player.IsShielded = true;
 
        //instanciamos el visual como hijo del jugador para que lo siga sin logica extra
        if (shieldPrefab != null)
            activeShieldInstance = Instantiate(shieldPrefab, ctx.Player.transform);
    }
 
    public override void Tick(AbilityContext ctx, float deltaTime)
    {
        shieldTimer += deltaTime;
 
        if (shieldTimer >= shieldDuration)
            ctx.Player.AbilityManager.EndAbility(this);
    }
 
    public override void End(AbilityContext ctx)
    {
        ctx.Player.IsShielded = false;
 
        if (activeShieldInstance != null)
            Destroy(activeShieldInstance);
 
        //importante: siempre llamar al base para que se aplique el cooldown
        base.End(ctx);
    }
}
