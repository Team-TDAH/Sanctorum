using UnityEngine;
//Para no hacer un lio de flachas en el animator, ya aprendi culpa dle anterior juego que hice, se bugeaban las animaciones y eran un lio
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    //facil de entender, si coincide con el numero hace esa animacion, todas salen del anystaet
    private const int idle=0;
    private const int run=1;//falta q lo haga seba(estaba pero vacio), agregado
    private const int jump=2;
    private const int falling=3;
    private const int land=4;//a futuro hay que fixearlo, queda raro al momento de caer
    private const int attack=5;
    private const int parry=6;
    // *** CAMBIO: saque el dash, todavia no existe el estado en el animator ***
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    //agregar cuando vaya agregando mas habilidades, es para conseguir sus activaciones
    [SerializeField] private AbilitySO parryAbility;
    //Canal del orbe para literlamente enterarnos si se activo o no, ya que solo es un frame
    [SerializeField] private AbilityChannel attackChannel;
    //duracion de la animacion, necesario por la estructura q estoy usando de animaciones
    [SerializeField] private float attackDuration = 0.25f;
    //ajuste lindo para que land no se active siempre que se esta cayendo, sino que solamente cuando sea gran altuera, queda menos torpe pero igualmente hay que cambiar la animacion de land
    [SerializeField] private float minFallTimeForLand = 0.4f;
    //pense que con esto arreglaria la anim de caer pero no
    [SerializeField] private float landDuration = 0.25f;
    //runtime
    private float fallTimer;
    private float landTimer;
    private float attackTimer;
    private bool wasGrounded;
    //cachea el ultimo estado para no setear el parametro cada frame al pedo
    private int currentState = -1;
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerController == null) playerController = GetComponentInParent<PlayerController>();
    }
    private void OnEnable()
    {
        if (attackChannel != null)
            attackChannel.OnAbilityStarted += HandleAttackStarted;
    }
    private void OnDisable()
    {
        if (attackChannel != null)
            attackChannel.OnAbilityStarted -= HandleAttackStarted;
    }
    //este canal es solo del orbe, asi que cualquier aviso es el ataque basico
    private void HandleAttackStarted(AbilitySO ability)
    {
        attackTimer = attackDuration;
    }
    private void Update()
    {
        if (playerController == null || animator == null) return;

        UpdateTimers();
        int newState = DecideState();
        if (newState != currentState)
        {
            currentState = newState;
            animator.SetInteger("AnimState", newState);
        }
    }
    private void UpdateTimers()
    {
        bool grounded = playerController.IsGrounded;
        //cuenta cuanto tiempo eta cayendo, para ver si reproduccir land o no
        if (!grounded && !playerController.IsDashing)
            fallTimer += Time.deltaTime;

        //al tocar ground ve si paso el suficiente timepo para activar land
        if (grounded && !wasGrounded)
        {
            if (fallTimer >= minFallTimeForLand)
                landTimer = landDuration;

            fallTimer = 0f;
        }
        if (landTimer > 0f)
            landTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        wasGrounded = grounded;
    }
    //prioridades de animaciones
    private int DecideState()
    {
        //el parry y ataque prioridad 1 siempre, ver como queda con la anim de salto y tal
        if (parryAbility != null && parryAbility.IsActive) return parry;
        // *** CAMBIO: el ataque ahora se decide por el timer, no por IsActive ***
        if (attackTimer > 0f) return attack;


        if (!playerController.IsGrounded)
        {
            return playerController.IsRising ? jump : falling;
        }
        if (landTimer > 0f) return land;

        if (Mathf.Abs(playerController.MoveInput.x) > 0.01f) return run;

        return idle;
    }
}