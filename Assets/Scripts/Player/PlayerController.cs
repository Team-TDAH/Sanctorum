using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravity = -30f;

    //variables de mejora de salto
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    //contadores de las mejoras de salto
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpReleaseFlag;
    //doble salto (tuve que cambiarlo, tenia antes cuantos saltos queria dar y restaba, pero no tenia sentido)
    [SerializeField] private BoolVariable doubleJumpUnlocked;
    private int currentJumps;
    private int MaxJumps => (doubleJumpUnlocked != null && doubleJumpUnlocked.Value) ? 2 : 1;
    //dash
    [SerializeField] private float dashDistance = 6f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;
    //curva muuy necesaria para al final ajustarla y que se sienta lo mas profesional posible
    [SerializeField] private AnimationCurve dashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    //era por si en algun momento usaba un desliz por el suelo, pero mejor no
    [SerializeField] private bool canDashInAir = true;
    [SerializeField] private BoolVariable dashUnlocked;

    //variables para gestionar el dash
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownCounter;
    private Vector2 dashDirection;
    private float lastFacingDirection = 1f; 
    //para saber hacia donde mirar si no hay input

    public Vector2 MoveInput => moveInput;
    public float LastFacingDirection => lastFacingDirection;
    public AbilityManager AbilityManager { get; private set; } 
    public bool IsShielded { get; set; } //no estoy seguro que tan necesario es aca, pero lo dejo por ahora
    //para que la UI acceda a la info del cooldown y lo transforme a valores entre 0 y 1, me agrada la propiedad muy simple pero util
    public float DashCooldownProgress => dashCooldown > 0f
    ? Mathf.Clamp01(1f - (dashCooldownCounter / dashCooldown))
    : 1f;


    [Header("Collision y raycast")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private float skinWidth = 0.015f;



    //Forma mas limpia que encontre para usar el sistema de inputs actual
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;

    //Referencias de componentes
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    //Variables necesarias
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool jumpFlag;
    public bool IsGrounded { get; private set; }
    public bool IsDashing => isDashing;

    //Variables necesarias para el sistema complejo de Raycasts
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private Bounds currentBounds;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        AbilityManager = GetComponent<AbilityManager>();

        //Unica forma que encontre para referenciar a los inputs, no estoy seguro si es la forma mas correcta
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            dashAction = playerInput.actions["Dash"];
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.simulated = true;
    }

    private void Start()
    {
        CalculateRaySpacing();
    }

    //Necesarios para el sistema de inputs
    private void OnEnable()
    {
        if (moveAction != null) moveAction.Enable();
        if (jumpAction != null) jumpAction.Enable();
        if (dashAction != null) dashAction.Enable();
    }
    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
        if (dashAction != null) dashAction.Disable();
    }


    private void Update()
    {
        //mucho mas limpio con este sistema de inputs
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();

        //guardamos la ultima direccion horizontal para usarla en el dash sin input
        if (moveInput.x != 0f)
            lastFacingDirection = Mathf.Sign(moveInput.x);

        //jumpbuffer
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        //coyoteTime
        if (IsGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }
        //regulador de salto
        if (jumpAction != null && jumpAction.WasReleasedThisFrame())
        {
            jumpReleaseFlag = true;
        }

        //solo se inicia si el cooldown del dash termino y no estamos dasheando
        dashCooldownCounter -= Time.deltaTime;
        if (dashAction != null && dashAction.WasPressedThisFrame())
        {
            TryStartDash();
        }
    }


    private void FixedUpdate()
    {
        //si estamos dasheando, ejecutamos la logica del dash y salimos
        if (isDashing)
        {
            ProcessDash();
            return;
        }

        //gravedad artificial
        if (IsGrounded)
        {
            if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0f;
            }
            //devolvemos la cantidad maxima de saltos al tocar suelo
            currentJumps = MaxJumps;
        }
        else if (coyoteCounter <= 0f && currentJumps == MaxJumps)
        {
            //clave para que al tirarse de una plataforma, no siga dejandome hacer la misma cantidad de saltos
            currentJumps = MaxJumps - 1;
        }

        currentVelocity.x = moveInput.x * moveSpeed;
        currentVelocity.y += gravity * Time.fixedDeltaTime;

        //Coyote time y doble salto
        if (jumpBufferCounter > 0f)
        {
            //permite saltar si hay coyotetime o hay saltos restantes
            if (coyoteCounter > 0f || currentJumps > 0)
            {
                currentVelocity.y = jumpForce;
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;

                //restamos el salto que hicimos cada vez que saltamos con estas condiciones
                currentJumps--;
            }
        }

        //salto regulado
        if (jumpReleaseFlag)
        {
            if (currentVelocity.y > 0f)
            {
                currentVelocity.y *= jumpCutMultiplier;
            }
            jumpReleaseFlag = false;
        }

        Vector2 deltaMovement = currentVelocity * Time.fixedDeltaTime;

        //calcula los raycast complicados
        UpdateRaycastBounds();
        IsGrounded = false;

        //detecta colisiones en ambos ejes
        if (deltaMovement.x != 0) HorizontalCollisions(ref deltaMovement);
        if (deltaMovement.y != 0) VerticalCollisions(ref deltaMovement);

        //aplicamos la posicion a la collision kinetica
        rb.MovePosition(rb.position + deltaMovement);
    }

    //Logica del movimiento del dash (mas compleja por el uso de movimiento kinematico, ahora no puedo arrepentirme de usar ese movimiento)
    private void TryStartDash()
    {
        //eñ dashunlocked es para prevenir que si me olvido de asignar el SO de dash, no se rompa el game
        bool dashAllowed = (IsGrounded || canDashInAir) && (dashUnlocked == null || dashUnlocked.Value);
        if (!dashAllowed || dashCooldownCounter > 0f) return;

        //direccion del input actual, o la ultima direccion mirando si no hay input
        Vector2 inputDir = moveInput;
        dashDirection = inputDir.sqrMagnitude > 0.01f
            ? inputDir.normalized
            : new Vector2(lastFacingDirection, 0f);

        isDashing = true;
        dashTimer = 0f;
        dashCooldownCounter = dashCooldown;

        //necesario para que se vea limpio en el aire
        currentVelocity = Vector2.zero;
    }

    //con del curva que todavia no siento que haga tanto cambio, luego con algunos ajustes de la curva vere si llego a un punto que me guste
    private void ProcessDash()
    {
        dashTimer += Time.fixedDeltaTime;
        float progress = dashTimer / dashDuration;

        if (progress >= 1f)
        {
            //necesario para que no caiga como una piedra luego del dash
            isDashing = false;
            currentVelocity = new Vector2(dashDirection.x * moveSpeed, 0f);
            return;
        }

        //la curva ayuda a que no sea lineal la velocidad del dash, sino tenga mas dinamismo 
        float speedThisFrame = dashCurve.Evaluate(progress) * (dashDistance / dashDuration);
        Vector2 deltaMovement = dashDirection * speedThisFrame * Time.fixedDeltaTime;

        UpdateRaycastBounds();
        IsGrounded = false;

        //para respetar collisiones, tambien esta en el movimiento normal
        if (deltaMovement.x != 0) HorizontalCollisions(ref deltaMovement);
        if (deltaMovement.y != 0) VerticalCollisions(ref deltaMovement);

        //necesario para frenarnos en caso de chocar con una pared
        bool hitWall = Mathf.Abs(deltaMovement.x) < Mathf.Abs(dashDirection.x * speedThisFrame * Time.fixedDeltaTime) * 0.5f
                       && dashDirection.y == 0f;
        if (hitWall)
        {
            isDashing = false;
            currentVelocity = Vector2.zero;
        }

        rb.MovePosition(rb.position + deltaMovement);
    }


    //----------------------------------------------------------------------------------------------
    //Todo lo de raycast, muchas formulas y demas cosas de videos de youtube largos vistos a x1.5 :)   Creo que nunca hara falta cambiarle nada
    private void UpdateRaycastBounds()
    {
        currentBounds = boxCollider.bounds;
        currentBounds.Expand(-skinWidth * 2f);
    }

    private void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(-skinWidth * 2f);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    private void HorizontalCollisions(ref Vector2 deltaMovement)
    {
        float directionX = Mathf.Sign(deltaMovement.x);
        float rayLength = Mathf.Abs(deltaMovement.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? new Vector2(currentBounds.min.x, currentBounds.min.y) : new Vector2(currentBounds.max.x, currentBounds.min.y);
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                deltaMovement.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;
            }
        }
    }

    private void VerticalCollisions(ref Vector2 deltaMovement)
    {
        float directionY = Mathf.Sign(deltaMovement.y);
        float rayLength = Mathf.Abs(deltaMovement.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? new Vector2(currentBounds.min.x, currentBounds.min.y) : new Vector2(currentBounds.min.x, currentBounds.max.y);
            rayOrigin += Vector2.right * (verticalRaySpacing * i + deltaMovement.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            if (hit)
            {
                deltaMovement.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
                if (directionY == -1)
                {
                    IsGrounded = true;
                }
                else
                {
                    currentVelocity.y = 0;
                }
            }
        }
    }
}