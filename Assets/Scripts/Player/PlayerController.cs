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
    //doble salto
    [SerializeField] private int maxJumps = 1;
    private int currentJumps;

    [Header("Collision y raycast")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private int horizontalRayCount = 4;
    [SerializeField] private int verticalRayCount = 4;
    [SerializeField] private float skinWidth = 0.015f;



    //Forma mas limpia que encontre para usar el sistema de inputs actual
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    //Referencias de componentes
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    //Variables necesarias
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private bool jumpFlag;
    public bool isGrounded { get; private set; }

    //Variables necesarias para el sistema complejo de Raycasts
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private Bounds currentBounds;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        //Unica forma que encontre para referenciar a los inputs, no estoy seguro si es la forma mas correcta
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
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
    }
    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (jumpAction != null) jumpAction.Disable();
    }


    private void Update()
    {
        //mucho mas limpio con este sistema de inputs
        if (moveAction != null)
            moveInput = moveAction.ReadValue<Vector2>();
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
        if (isGrounded)
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

    }
    /*
    private void FixedUpdate()
    {
        // Lógica de gravedad base
        if (isGrounded && currentVelocity.y < 0)
        {
            currentVelocity.y = 0f;
        }

        currentVelocity.x = moveInput.x * moveSpeed;
        currentVelocity.y += gravity * Time.fixedDeltaTime;

        //salto teniendo en cuenta coyote time y jumpbuffer
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            currentVelocity.y = jumpForce;

            //evito dobles saltos raros
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        //regulacion de salto
        if (jumpReleaseFlag)
        {
            if (currentVelocity.y > 0f)
            {
                currentVelocity.y *= jumpCutMultiplier;
            }
            jumpReleaseFlag = false;
        }

        Vector2 deltaMovement = currentVelocity * Time.fixedDeltaTime;

        UpdateRaycastBounds();
        isGrounded = false;

        if (deltaMovement.x != 0)
            HorizontalCollisions(ref deltaMovement);

        if (deltaMovement.y != 0)
            VerticalCollisions(ref deltaMovement);

        //Dar la posicion que calculamos directamente al rigibody kinematico que elegi
        rb.MovePosition(rb.position + deltaMovement);
    }
    */


    private void FixedUpdate()
    {
        //gravedad artificial
        if (isGrounded)
        {
            if (currentVelocity.y < 0)
            {
                currentVelocity.y = 0f;
            }
            //devolvemos la cantidad maxima de saltos al tocar sujelo
            currentJumps = maxJumps;
        }
        else if (coyoteCounter <= 0f && currentJumps == maxJumps)
        {
            //clave para que al tirarse de una plataforma, no siga dejandome hacer la misma cantidad de saltos
            currentJumps = maxJumps - 1;
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
        isGrounded = false;

        //detecta colisiones en ambos ejes
        if (deltaMovement.x != 0) HorizontalCollisions(ref deltaMovement);
        if (deltaMovement.y != 0) VerticalCollisions(ref deltaMovement);

        //aplicamos la posicion a la collision kinetica
        rb.MovePosition(rb.position + deltaMovement);
    }






    //----------------------------------------------------------------------------------------------
    //Todo lo de raycast, muchas formulas y demas cosas de videos de youtube largos vistos a x1.5 :)
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
        float rayLength = Mathf.Abs(deltaMovement.y) +skinWidth;

        for (int i = 0; i<verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY ==-1) ? new Vector2(currentBounds.min.x, currentBounds.min.y) : new Vector2(currentBounds.min.x, currentBounds.max.y);
            rayOrigin += Vector2.right *(verticalRaySpacing *i+deltaMovement.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up*directionY, rayLength, collisionMask);
            if (hit)
            {
                deltaMovement.y =(hit.distance - skinWidth)*directionY;
                rayLength = hit.distance;
                if (directionY ==-1)
                {
                    isGrounded =true;
                }
                else
                {
                    currentVelocity.y =0;
                }
            }
        }
    }
}