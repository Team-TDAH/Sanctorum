using UnityEngine;
//debo manejar el cambio de color con los sprites
public class BossLamp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer lampSprite;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color activeColor = Color.violet;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float projectileLifetime = 5f;
    //estado runtime
    private bool isActive;
    private float fireTimer;
    private Transform playerTransform;
    private void Awake()
    {
        if (lampSprite == null)
            lampSprite = GetComponent<SpriteRenderer>();

        SetColor(normalColor);
    }
    private void Update()
    {
        if (!isActive) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }
    //la idea es q el jefe llame
    public void Activate(Transform player)
    {
        isActive = true;
        playerTransform = player;
        fireTimer = fireRate;
        SetColor(activeColor);
    }
    public void Deactivate()
    {
        isActive = false;
        SetColor(normalColor);
    }
    private void Shoot()
    {
        if (projectilePrefab == null || playerTransform == null) return;
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>()?.Initialize(direction, projectileSpeed, projectileLifetime, damage);
    }
    private void SetColor(Color color)
    {
        if (lampSprite != null)
            lampSprite.color = color;
    }
}