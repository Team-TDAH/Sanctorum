using UnityEngine;
//crosshair q sigue al puntero y cambia de color y tamaño al "tocar" a alguine en la capa enemies
public class Crosshair : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private SpriteRenderer crosshairSprite;
    private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Color normalColor = Color.white;
    private float normalScale = 1f;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float targetScale = 1.3f;
    //para q el crosshair se oculte en menu y demas
    private bool visibleCrosshair = true;
    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        if (crosshairSprite == null) crosshairSprite = GetComponent<SpriteRenderer>();

        //arranca visible y el cursor escondido de 1
        SetVisible(true);
    }
    //cambie update x fixedupdate porque daba saltos al moverse el player y el puntero
    private void FixedUpdate()
    {
        if (!visibleCrosshair) return;   //para q si este oculta no se preocupe x la logica

        if (aimCamera == null || UnityEngine.InputSystem.Mouse.current == null) return;

        Vector3 screenPos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(aimCamera.transform.position.z);
        Vector2 mouseWorld = aimCamera.ScreenToWorldPoint(screenPos);

        //la diana va justo donde esta el cursor
        transform.position = mouseWorld;

        //para q al estar en pausa no siga "detectando" enemigos y cambie de color y escala
        if (Time.timeScale == 0f)
        {
            if (crosshairSprite != null) crosshairSprite.color = normalColor;
            transform.localScale = Vector3.one * normalScale;
            return;
        }
        //el chequeo si es dañable
        bool onTarget = IsOverTarget(mouseWorld);
        if (crosshairSprite != null)
            crosshairSprite.color = onTarget ? targetColor : normalColor;

        float scale = onTarget ? targetScale : normalScale;
        transform.localScale = Vector3.one * scale;
    }
    private bool IsOverTarget(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(worldPos, detectionRadius, targetMask);
        if (hit == null) return false;
        
        //confirmamos que sea algo que puede recibir dano, no cualquier cosa en esa capa
        return hit.GetComponentInParent<IDamageable>() != null;
    }
    public void SetVisible(bool value)
    {
        visibleCrosshair = value;

        if (crosshairSprite != null)
            crosshairSprite.enabled = value;

        Cursor.visible = !value;
        Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.None;
    }
}