using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//script que va unido al player que administra todas las habilidades del mismo
public class AbilityManager : MonoBehaviour
{
    //lista donde tengo que agregar cada habilidad nueva
    [SerializeField] private List<AbilitySO> abilities = new();

    private PlayerController playerController;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    //mapa de InputAction por nombre para no buscar cada frame
    private Dictionary<string, InputAction> inputMap = new();

    //contexto reutilizable
    private AbilityContext ctx = new();


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        BuildInputMap();
    }

    private void Start()
    {
        //resetear estado runtime de cada SO por si se recarga la escena
        foreach (var ability in abilities)
            ability.ResetRuntimeState();
    }

    private void OnEnable()
    {
        foreach (var action in inputMap.Values)
            action.Enable();
    }

    private void OnDisable()
    {
        foreach (var action in inputMap.Values)
            action.Disable();
    }

    private void Update()
    {
        UpdateContext();
        TickCooldowns();
        CheckInputs();
    }




    private void BuildInputMap()
    {
        if (playerInput == null) return;

        foreach (var ability in abilities)
        {
            if (string.IsNullOrEmpty(ability.inputActionName)) continue;
            if (inputMap.ContainsKey(ability.inputActionName)) continue;

            var action = playerInput.actions[ability.inputActionName];
            if (action != null)
                inputMap[ability.inputActionName] = action;
            else
                Debug.LogWarning($"[AbilityManager] No se encontro la InputAction '{ability.inputActionName}' para '{ability.abilityName}'");
        }
    }




    private void UpdateContext()
    {
        ctx.Player = playerController;
        ctx.Rb = rb;
        ctx.IsGrounded = playerController.IsGrounded;
        ctx.IsDashing = playerController.IsDashing;
        ctx.MoveInput = playerController.MoveInput;
        ctx.LastFacingDirection = playerController.LastFacingDirection;
    }

    private void TickCooldowns()
    {
        foreach (var ability in abilities)
        {
            if (ability.CooldownRemaining > 0f)
            {
                ability.CooldownRemaining -= Time.deltaTime;
                if (ability.CooldownRemaining < 0f) ability.CooldownRemaining = 0f;

                //Se notifica el progreso del cooldown para la UI en un futuro
                ability.channel?.RaiseCooldownUpdated(ability, ability.CooldownRemaining / ability.cooldown);
            }

            if (ability.IsActive)
                ability.Tick(ctx, Time.deltaTime);
        }
    }

    private void CheckInputs()
    {
        foreach (var ability in abilities)
        {
            if (!inputMap.TryGetValue(ability.inputActionName, out var action)) continue;

            if (action.WasPressedThisFrame() && ability.CanUse(ctx))
                ActivateAbility(ability);
        }
    }




    private void ActivateAbility(AbilitySO ability)
    {
        ability.IsActive = true;

        //el cooldown empieza al activar excepto que tenga activado lo de que sea al final
        if (!ability.cooldownAfterEnd)
            ability.CooldownRemaining = ability.cooldown;

        ability.channel?.RaiseStarted(ability);
        ability.Execute(ctx);
    }

    //las habilidades instantaneas llaman esto desde Execute, las de duracion desde el tick
    public void EndAbility(AbilitySO ability)
    {
        ability.End(ctx);
        ability.channel?.RaiseEnded(ability);
    }
}