using UnityEngine;
using UnityEngine.InputSystem;
//la idea es que cualquier habilidad pueda utilizar esto para apuntar con el mouse nad amas
public class PlayerAim : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera aimCamera;
    //punto que sigue la direccion ya clampeada, no el mouse crudo
    [SerializeField] private Transform aimReticle;
    [SerializeField] private float reticleDistance = 1.5f;

    //direccion final, la idea es q la lean las hbailidades
    public Vector2 AimDirection { get; private set; } = Vector2.right;


    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        if (aimCamera == null)
            aimCamera = Camera.main;
            
    }

    private void Update()
    {
        if (Mouse.current == null || aimCamera == null || playerController == null) return;

        Vector2 origin = playerController.WeaponPoint != null
            ? (Vector2)playerController.WeaponPoint.position
            : (Vector2)playerController.transform.position;

        Vector2 mouseWorld = GetMouseWorldPosition();

        Vector2 toMouse = mouseWorld - origin;
        if (toMouse.sqrMagnitude < 0.0001f) return;

        //mira hacia donde el mouse este con respecto al player
        float facingSign = Mathf.Abs(toMouse.x) > 0.01f
            ? Mathf.Sign(toMouse.x)
            : playerController.LastFacingDirection;

        playerController.SetFacingDirection(facingSign);

        //sin limite ahora
        AimDirection = toMouse.normalized;

        //para que el punto de mira este en la direccion tambien 
        if (aimReticle != null)
            aimReticle.position = origin + AimDirection * reticleDistance;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(aimCamera.transform.position.z);
        return aimCamera.ScreenToWorldPoint(screenPos);
    }
}