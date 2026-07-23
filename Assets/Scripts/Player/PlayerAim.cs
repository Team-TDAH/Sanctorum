using UnityEngine;
using UnityEngine.InputSystem;
//despues de muchos ajustes, quedo como me gusta, sin salto por abajo ni angulos raros que hacian que no siempre iba la mira a donde queria
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float sensitivity = 0.017f;
    //para q tome de "0" el medio del pj
    private float center = 90f;
    //90+135=225 que seria el maximo de un lado, y del otro 90-135=-45, que es todo los angulos menos desde el 225 al 315 creo
    private float angleArc = 135f;
    [SerializeField] private Transform aim;
    [SerializeField] private float aimDistance = 5f;
    //punto virtual que el mouse mueve, siempre dentro del arco permitido
    private Vector2 aimOffset;

    //direccion final, la leen las habilidades de ataque
    public Vector2 AimDirection { get; private set; }
    // *** para que el RespawnManager y la pausa puedan congelar el apuntado ***
    //al desactivarse tambien se oculta la mirilla
    private bool inputEnabled = true;
    //por un bug que al morir seguia moviendose la "mira"
    public bool InputEnabled
    {
        get => inputEnabled;
        set
        {
            inputEnabled = value;
            if (aim != null)
                aim.gameObject.SetActive(value);
        }
    }

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //comienza a la derceha
        aimOffset = Vector2.right * aimDistance;
        AimDirection = Vector2.right;
    }
    private void Update()
    {
        if (!inputEnabled) return;
        if (Mouse.current == null || playerController == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        if (delta.sqrMagnitude > 0.0001f)
        {
            aimOffset += delta * sensitivity;
            //para que no pueda girar completamente por abajo, sentia que era innecesario e incomodo, asi que es por arriba o nada
            float angle = Mathf.Atan2(aimOffset.y, aimOffset.x) * Mathf.Rad2Deg;
            float deltaFromCenter = Mathf.DeltaAngle(center, angle);
            if (Mathf.Abs(deltaFromCenter) > angleArc)
            {
                float clampedAngle = center + Mathf.Sign(deltaFromCenter) * angleArc;
                float rad = clampedAngle * Mathf.Deg2Rad;
                aimOffset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
            aimOffset = aimOffset.normalized * aimDistance;
        }
        AimDirection = aimOffset.normalized;
        if (Mathf.Abs(AimDirection.x) > 0.05f)
            playerController.SetFacingDirection(Mathf.Sign(AimDirection.x));

        Vector2 origin = playerController.WeaponPoint != null
            ? (Vector2)playerController.WeaponPoint.position
            : (Vector2)playerController.transform.position;

        if (aim != null)
            aim.position = origin + AimDirection * aimDistance;
    }
}