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
    private const int dash=7;//falta q lo haga seba
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Animator animator;
    //agregar cuando vaya agregando mas habilidades, es para conseguir sus activaciones
    [SerializeField] private AbilitySO attackAbility;
    [SerializeField] private AbilitySO parryAbility;
    //ajuste lindo para que land no se active siempre que se esta cayendo, sino que solamente cuando sea gran altuera, queda menos torpe pero igualmente hay que cambiar la animacion de land
    [SerializeField] private float minFallTimeForLand = 0.4f;
    //pense que con esto arreglaria la anim de caer pero no
    [SerializeField] private float landDuration = 0.25f;
    //runtime
    private float fallTimer;
    private float landTimer;
    private bool wasGrounded;
    //cachea el ultimo estado para no setear el parametro cada frame al pedo
    private int currentState = -1;
    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (playerController == null) playerController = GetComponentInParent<PlayerController>();
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

        wasGrounded = grounded;
    }
    //prioridades de animaciones
    private int DecideState()
    {
        //el parry y ataque prioridad 1 siempre, ver como queda con la anim de salto y tal
        if (parryAbility != null && parryAbility.IsActive) return parry;
        if (attackAbility != null && attackAbility.IsActive) return attack;

        if (playerController.IsDashing) return dash;

        if (!playerController.IsGrounded)
        {
            return playerController.IsRising ? jump : falling;
        }
        if (landTimer > 0f) return land;

        if (Mathf.Abs(playerController.MoveInput.x) > 0.01f) return run;

        return idle;
    }
}