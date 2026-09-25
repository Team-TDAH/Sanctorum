using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //-------
    //MOVIMIENTO Y SALTO
    [SerializeField] private float moveSpeed = 21f;
    [SerializeField] private float jumpForce = 37f;
    [SerializeField] private float gravity = -65f;//termino siento valores altos culpa de cambiar de tamaño del player
    //reduccion de velocidad al caminar hacia atras, vere si me agrada, sino se va
    [SerializeField] private float backwardSpeedMultiplier = 0.75f;
    //variables de mejora de salto
    private float jumpCutMultiplier = 0.5f;
    private float coyoteTime = 0.1f;
    private float jumpBufferTime = 0.1f;
    //contadores de las mejoras de salto
    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpReleaseFlag;
    //-------------------------
    //SALTO Q SE SIENTA BIEN
    //probar con furia sobre si estan correctos y comodos, una vez elegidos los valores quitar la serializacion
    //hace a la gravedad "asimetrica", para q se sienta mejor los saltos y no de la sensacion de flotar raro
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    //dond econdisera que la llegamos a la cima del salto
    [SerializeField] private float peakThreshold = 7f;
    //quita mayor parte de la gravedad en la cima, asi no se siente como flotar
    [SerializeField] private float peakGravityMultiplier = 0.5f;
    //es como un pequeño impulso al llegar a la cima, lo agregue porque sentia medio soso o pesado los saltos, y no como juegos estilo silkson
    [SerializeField] private float peakSpeed = 5f;
    //segunda tanda de "mejoras de salto"
    //aceleracion y frenado en el suelo y en el aire, en el aire se siente mucho mas, tendria que probar con las animacioens puestas
    [SerializeField] private float groundAcceleration = 130f;
    [SerializeField] private float groundDeceleration = 160f;
    //en el aire tenes menos control, es lo estandar en platformers
    [SerializeField] private float airAcceleration = 160f;
    [SerializeField] private float airDeceleration = 110f;
    //sentia que frenaba el salto al chocar con una esquina cualquiera sea, con eso se "soluciona"
    [SerializeField] private float cornerCorrectionDistance = 0.25f;
    //----
    //DOBLE SALTO (tuve que cambiarlo, tenia antes cuantos saltos queria dar y restaba, pero no tenia sentido) (la idea es q cierto jefe nos lo de, no tenerlo por defecto pero no iba en "habilidades")
    [SerializeField] private BoolVariable doubleJumpUnlocked;
    private int currentJumps;
    private int MaxJumps => (doubleJumpUnlocked != null && doubleJumpUnlocked.Value) ? 2 : 1;
    //---------------------------------
    //DASH (ajustar con furia)
    [SerializeField] private float dashDistance = 6f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.6f;
    //curva muuy necesaria para al final ajustarla y que se sienta lo mas profesional posible
    [SerializeField] private AnimationCurve dashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private BoolVariable dashUnlocked;
    //variables para gestionar el dash
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownCounter;
    private Vector2 dashDirection;
    //---
    //WeaponPoint que esta en el baston y el flip del player cuando se da vuelta (ya vere de agregar una animacion de darse la vuelta para q no parezca de papel)
    //no estaba centrado bien el pj, y acomodar por offset era un lio, termine agregando un gameobject vacio en el baston para disparar desde ahi lo que sea
    [SerializeField] private Transform weaponPoint;
    public Transform WeaponPoint => weaponPoint;
    private float lastFacingDirection = 1f;
    //para saber hacia donde mirar si no hay input
    public void SetFacingDirection(float sign)
    {
        lastFacingDirection = sign;
    }
    public float LastFacingDirection => lastFacingDirection;
    //---------------------------------
    //VARIABLES Y PROPIEDADES NECESARIAMENTE PUBLICAS
    public Vector2 MoveInput => moveInput;
    //ya me canse de agregar variables publicas nuevas, pero esta es necesaria para que al momento de morir, desactive el control del player y no el playercontroller completo
    //y otro cambio, para que al morir no se mueva en el ultimo input indefinidamente
    public bool InputEnabled
    {
        get => inputEnabled;
        set
        {
            inputEnabled = value;
            if (!value)
            {
                moveInput = Vector2.zero;
                currentVelocity = Vector2.zero;
            }
        }
    }
    private bool inputEnabled = true;
    public AbilityManager AbilityManager { get; private set; }
    public bool IsShielded { get; set; } //no estoy seguro que tan necesario es aca, pero lo dejo por ahora
    //para que la UI acceda a la info del cooldown y lo transforme a valores entre 0 y 1, me agrada la propiedad muy simple pero util
    public float DashCooldownProgress => dashCooldown > 0f
    ? Mathf.Clamp01(1f - (dashCooldownCounter / dashCooldown))
    : 1f;
    public bool IsGrounded { get; private set; }
    public bool IsDashing => isDashing;
    //para que el animator distinga entre estar subiendo o bajando
    public bool IsRising => currentVelocity.y > 0f;
    //----------
    //RAYCAST Y COLISIONES DEL PLAYER
    [SerializeField] private LayerMask collisionMask;
    //plataformas atravesables como el elden lilies
    [SerializeField] private LayerMask layerPlatforms;
    //cuanto se ignora de la plataforma
    //LUEGO AJUSTAR SEGUN ANIMACION 
    [SerializeField] private float dropDuration = 0.25f;
    private float dropThroughTimer;
    //subir en caso de sentir que transpasamos objetos
    [SerializeField] private int horizontalRayCount = 8;
    [SerializeField] private int verticalRayCount = 4;
    private float skinWidth = 0.015f;
    //Variables necesarias para el sistema "complejo" de Raycasts
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private Bounds currentBounds;
    //----------------------------
    //INPUTS Y REFER(Forma mas limpia que encontre para usar el sistema de inputs actual)
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    //referencias de componentes
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    //variables necesarias
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    //------------------------------COMIENZA ACA EL SCRIPT
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
    //literalmente puse esta verificacion de inputenabled porque es la unica solucion que encontre para que al morir, no puedas moverte pero si caigas al suelo
    if (InputEnabled)
    {
        //mucho mas limpio con este sistema de inputs
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();


        //regulador de salto
        if (jumpAction != null && jumpAction.WasReleasedThisFrame())
        {
            jumpReleaseFlag = true;
        }

        if (dashAction != null && dashAction.WasPressedThisFrame())
        {
            TryStartDash();
        }
        //jumpbuffer
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            //abajo + salto nos deja caer de la plataforma en vez de saltar
            if (moveInput.y < -0.5f)
            {
                dropThroughTimer = dropDuration;
                jumpBufferCounter = 0f;
            }
            else
            {
                jumpBufferCounter = jumpBufferTime;
            }
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
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
        //solo se inicia si el cooldown del dash termino y no estamos dasheando
        dashCooldownCounter -= Time.deltaTime;
        dropThroughTimer -= Time.deltaTime;
    }
    //fisica o movimiento en el fixed, es la forma correcta de hacerlo, y en el update inputs y demas correciones
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
        //----------
        //peakpoint q marca cuando estmaos en la cima
        float peakPoint = IsGrounded
            ? 0f
            : Mathf.InverseLerp(peakThreshold, 0f, Mathf.Abs(currentVelocity.y));

        //si se mueve en direccion contraria a donde apunta va mas lento (osea camina mas lentos yendo hacia atars)
        //comparamos la direccion del input con el facing (que lo da el mouse via PlayerAim)
        float speedForThisDirection = moveSpeed + peakSpeed * peakPoint;
        bool movingBackward = moveInput.x != 0f && Mathf.Sign(moveInput.x) != lastFacingDirection;
        if (movingBackward)
            speedForThisDirection *= backwardSpeedMultiplier;

        float targetSpeed = moveInput.x * speedForThisDirection;
        //si hay input aceleramos, si no hay frenamos, quizas con esto luego pueda crear superficies resbaladizas
        bool hasInput = Mathf.Abs(moveInput.x) > 0.01f;
        float accelRate = IsGrounded
            ? (hasInput ? groundAcceleration : groundDeceleration)
            : (hasInput ? airAcceleration : airDeceleration);

        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        //gravedad fuerte al caer, y "liviana" en la cima
        float gravityMultiplier = currentVelocity.y < 0f ? fallGravityMultiplier : 1f;
        gravityMultiplier *= Mathf.Lerp(1f, peakGravityMultiplier, peakPoint);
        currentVelocity.y += gravity * gravityMultiplier * Time.fixedDeltaTime;
        //---------
        //coyote time y doble salto
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
        //salto regulado (antes y depsues)
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
        //aca lo de esquivar esquinas
        bool corrected = TryCornerCorrection(ref deltaMovement);
        if (deltaMovement.y != 0 && !corrected) VerticalCollisions(ref deltaMovement);
        //aplicamos la posicion a la collision kinetica
        rb.MovePosition(rb.position + deltaMovement);
    }
    //--------DASH (ajustar valores con furia)
    //mas compleja por el uso de movimiento kinematico, ahora no puedo arrepentirme de usar ese movimiento
    private void TryStartDash()
    {
        //eñ dashunlocked es para prevenir que si me olvido de asignar el SO de dash, no se rompa el game
        bool dashAllowed = dashUnlocked == null || dashUnlocked.Value;
        if (!dashAllowed || dashCooldownCounter > 0f) return;
        //saque la posibilidad de dashear hacia arriba, parecia un tercer salto
        float dashX = moveInput.x != 0f ? Mathf.Sign(moveInput.x) : lastFacingDirection;
        dashDirection = new Vector2(dashX, 0f);
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
    //--------
    //si golpeamos el techo con una sola esquina, empujamos al player al costado para que pase y no se tosquee
    private bool TryCornerCorrection(ref Vector2 deltaMovement)
    {
        //solo aplica cuando vamos subiendo
        if (deltaMovement.y <= 0f) return false;

        float rayLength = Mathf.Abs(deltaMovement.y) + skinWidth;
        //rayos en las dos esquinas superiores del collider para detectaslo
        Vector2 leftOrigin = new Vector2(currentBounds.min.x, currentBounds.max.y);
        Vector2 rightOrigin = new Vector2(currentBounds.max.x, currentBounds.max.y);
        bool leftHit = Physics2D.Raycast(leftOrigin, Vector2.up, rayLength, collisionMask);
        bool rightHit = Physics2D.Raycast(rightOrigin, Vector2.up, rayLength, collisionMask);
        //facil de detectar, si ambos golpean es techo sino esquina
        if (leftHit == rightHit) return false;
        //corremos el player
        float direction = leftHit ? 1f : -1f;
        for (float offset = skinWidth; offset <= cornerCorrectionDistance; offset += skinWidth)
        {
            Vector2 testOrigin = (leftHit ? leftOrigin : rightOrigin) + Vector2.right * (direction * offset);
            if (!Physics2D.Raycast(testOrigin, Vector2.up, rayLength, collisionMask))
            {
                deltaMovement.x += direction * offset;
                return true;
            }
        }
        return false;
    }
    //-------------------------------------------------------
    //Todo lo de raycast, muchas formulas y demas cosas complicada q se copian de alguien q sepa. Creo que nunca hara falta cambiarle nada
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
                //las one way no frenan de costado, se atraviesan
                if (((1 << hit.collider.gameObject.layer) & layerPlatforms) != 0) continue;

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
                //solo frena cuando caemos, sino atravieza
                bool isOneWay = ((1 << hit.collider.gameObject.layer) & layerPlatforms) != 0;
                if (isOneWay && (directionY == 1f || dropThroughTimer > 0f)) continue;
                //fin plataforma atravesable
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