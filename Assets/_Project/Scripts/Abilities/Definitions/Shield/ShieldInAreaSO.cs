using UnityEngine;


[CreateAssetMenu(fileName = "ShieldInAreaSO", menuName = "Abilities/ShieldInAreaSO")]
public class ShieldInAreaSO : AbilitySO
{
    public float shieldDuration = 3f;
    //prefab visual del escudo, debe tener el componente ShieldVisual
    public GameObject shieldPrefab;
 
    //timer interno, NonSerialized porque es estado runtime, no dato del asset
    [System.NonSerialized] private float shieldTimer;
    [System.NonSerialized] private GameObject activeShieldInstance;
 
 
    public override void Execute(AbilityContext ctx)
    {
        //conseguimos la referencia directamente para no tener que buscarla cada vez que se recibe daño
        ctx.PlayerHealth.IsShielded = true;

        shieldTimer = 0f;
 
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
        ctx.PlayerHealth.IsShielded = false;
 
        if (activeShieldInstance != null)
            Destroy(activeShieldInstance);
 
        //importante: siempre llamar al base para que se aplique el cooldown
        base.End(ctx);
    }
}
