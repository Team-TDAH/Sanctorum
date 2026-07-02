using UnityEngine;

//proyectil super basico que va en linea recta y hace daño al player y se destruye
public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;
    private float lifeTimer;
    [SerializeField] private LayerMask playerMask;


    public void Initialize(Vector2 dir, float spd, float life, int dmg)
    {
        direction = dir.normalized;
        speed = spd;
        lifetime = life;
        damage = dmg;
        lifeTimer = 0f;
    }
    private void Update()
    {
        //movimiento del proyectil basico, con su ciclo de vida
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //cuando colisione con algo que este en el layer que pongamos, hace el daño y se destruye
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;
        var playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(damage);
        Destroy(gameObject);
    }
}