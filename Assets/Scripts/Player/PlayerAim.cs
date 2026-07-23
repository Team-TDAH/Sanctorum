using UnityEngine;
using UnityEngine.InputSystem;
//ahora el apuntado dejo de ser por el "lugar" donde este el mouse, sino por su movimiento, asi no se siente raro
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float sensitivity = 0.017f; //deberia cambiar este valor en un futuro para cambiar la sensiblidad, aunque no creo que haga falta pensandolo
    [SerializeField] private Transform aim;
    [SerializeField] private float aimDistance = 5f;
    //punto "artificial" del mouse
    private Vector2 aimOffset = Vector2.right;
    //direccion final, para que la puedan utilizar las habilidades en un futuro para disparar en esas direcciones
    public Vector2 AimDirection { get; private set; } = Vector2.right;
    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        //q no se vea el mouse y no se salga de la ventana al prog
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        aimOffset = Vector2.right * aimDistance;
    }

    private void Update()
    {
        if (Mouse.current == null || playerController == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();

        if (delta.sqrMagnitude > 0.0001f)
        {
            //toma los movimiento del mouse, y no su lugar en la pantalla
            aimOffset += delta * sensitivity;
            //para q este en un radio alrededor dle player
            aimOffset = aimOffset.normalized * aimDistance;
        }
        AimDirection = aimOffset.normalized;
        //para que el player mire hacia donde este apuntadno
        if (Mathf.Abs(AimDirection.x) > 0.05f)
            playerController.SetFacingDirection(Mathf.Sign(AimDirection.x));

        Vector2 origin = playerController.WeaponPoint != null
            ? (Vector2)playerController.WeaponPoint.position
            : (Vector2)playerController.transform.position;

        if (aim != null)
            aim.position = origin + AimDirection * aimDistance;
    }
}