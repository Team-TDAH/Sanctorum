using UnityEngine;
using UnityEngine.InputSystem;
public class Crosshair : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField] private SpriteRenderer crosshairSprite;
    //para notificarnos de cuando esta peleando o no
    [SerializeField] private BossHealthChannel bossChannel;
    private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Color normalColor = Color.white;
    private float normalScale = 1f;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float targetScale = 1.3f;
    //crosshair solo visible durante una pelea
    private bool inFight;
    //fix de bug q hacia q el punteor de combate aparezca cuando no debia
    private bool hiddenByMenu;
    private void Awake()
    {
        if (aimCamera == null) aimCamera = Camera.main;
        if (crosshairSprite == null) crosshairSprite = GetComponent<SpriteRenderer>();

        inFight = false;
        hiddenByMenu = false;
        RefreshVisibility();
    }
    //suscripcion a la pelea
    private void OnEnable()
    {
        if (bossChannel != null)
        {
            bossChannel.OnBossFightStarted += HandleFightStarted;
            bossChannel.OnBossDefeated += HandleFightEnded;
        }
    }
    private void OnDisable()
    {
        if (bossChannel != null)
        {
            bossChannel.OnBossFightStarted -= HandleFightStarted;
            bossChannel.OnBossDefeated -= HandleFightEnded;
        }
    }
    private void HandleFightStarted(string bossName, int maxHealth)
    {
        inFight = true;
        RefreshVisibility();
    }
    private void HandleFightEnded()
    {
        inFight = false;
        RefreshVisibility();
    }
    //para q el respawnmana y el pausemanger sepan 
    public void SetHiddenByMenu(bool hidden)
    {
        hiddenByMenu = hidden;
        RefreshVisibility();
    }
    //la diana se ve solo si estamos en pelea Y no hay un menu tapando
    private void RefreshVisibility()
    {
        bool visible = inFight && !hiddenByMenu;

        if (crosshairSprite != null)
            crosshairSprite.enabled = visible;

        Cursor.visible = !visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.None;
    }
    private void LateUpdate()
    {
        //solo procesa si esta realmente visible
        if (!inFight || hiddenByMenu) return;
        //si no estamos en pelea,  no hace nada
        if (!inFight) return;
        if (aimCamera == null || Mouse.current == null) return;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = Mathf.Abs(aimCamera.transform.position.z);
        Vector2 mouseWorld = aimCamera.ScreenToWorldPoint(screenPos);
        transform.position = mouseWorld;
        bool onTarget = IsOverTarget(mouseWorld);
        if (crosshairSprite != null)
            crosshairSprite.color = onTarget ? targetColor : normalColor;

        transform.localScale = Vector3.one * (onTarget ? targetScale : normalScale);
    }
    private bool IsOverTarget(Vector2 worldPos)
    {
        Collider2D hit = Physics2D.OverlapCircle(worldPos, detectionRadius, targetMask);
        if (hit == null) return false;
        return hit.GetComponentInParent<IDamageable>() != null;
    }
    //si el crosshair esta, el cursor no y vice
    public void SetVisible(bool value)
    {
        inFight = value;
        if (crosshairSprite != null)
            crosshairSprite.enabled = value;

        Cursor.visible = !value;
        Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.None;
    }
}