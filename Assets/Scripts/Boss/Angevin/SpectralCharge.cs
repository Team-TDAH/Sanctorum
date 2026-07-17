using UnityEngine;
public class SpectralCharge : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    //recibe mas daño 
    [SerializeField] private float damageMultiplier = 1.15f;
    [SerializeField] private float debuffDuration = 30f;
    //poquito daño para que se note que fue golpeado
    [SerializeField] private int impactDamage = 15;
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private float lifeTimer;
    //no aplica 2 veces el debuff
    private bool hasHit;
    public void Initialize(Vector2 dir, float spd, float life)
    {
        direction = dir.normalized;
        speed = spd;
        lifetime = life;
        lifeTimer = 0f;
    }
    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;
        hasHit = true;
        //aplica el daño y el debuff
        playerHealth.TakeDamage(impactDamage);
        playerHealth.ApplyDamageDebuff(damageMultiplier, debuffDuration);
    }
}