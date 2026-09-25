using UnityEngine;
using UnityEngine.InputSystem;
//volvi al apuntado a donde este el puntero directamente y el facing sigue al puntero, habria que poner algo distinto de puntero
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera aimCamera;
    //direccion final, la leen las habilidades de ataque
    public Vector2 AimDirection { get; private set; } = Vector2.right;
    //para congelar el apuntado al morir o en pausa
    private bool inputEnabled = true;
    public bool InputEnabled
    {
        get => inputEnabled;
        set => inputEnabled = value;
    }
    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        if (aimCamera == null)
            aimCamera = Camera.main;
    }
    private void Update()
    {
        if (!inputEnabled) return;
        if (Mouse.current == null || playerController == null || aimCamera == null) return;

        //posicion del cursor en el mundo
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(aimCamera.transform.position.z);
        Vector2 mouseWorld = aimCamera.ScreenToWorldPoint(screenPos);

        Vector2 origin = playerController.WeaponPoint != null
            ? (Vector2)playerController.WeaponPoint.position
            : (Vector2)playerController.transform.position;

        //la direccion es directamente hacia el cursor
        Vector2 toCursor = mouseWorld - origin;
        if (toCursor.sqrMagnitude > 0.0001f)
            AimDirection = toCursor.normalized;

        //el player mira hacia el lado del cursor
        if (Mathf.Abs(AimDirection.x) > 0.05f)
            playerController.SetFacingDirection(Mathf.Sign(AimDirection.x));
    }
}