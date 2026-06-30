using UnityEngine;

//script para el prefab del proyectil de luz, va en linea recta, hace daño al impactar en algo que este en la capa Enemies o la que coloque, y se destruye luego de 3 seg
public class LightOrbProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;

    private Vector2 direction;
    private float speed;
    private float lifetime;
    private int damage;

    private float lifeTimer;


    public void Initialize(Vector2 dir, float spd, float life, int dmg)
    {
        direction = dir;
        speed = spd;
        lifetime = life;
        damage = dmg;
        lifeTimer = 0f;

        //orientamos el sprite hacia la direccion de vuelo
        if (dir.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    private void Update()
    {
        //movimiento en linea recta sin gravedad (luego confirmare si quieren que tenga o no gravedad, creo que quedara asi)
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //ver si esta en la capa enemigo, sino no hacer nada
        if (((1 << other.gameObject.layer) & enemyMask) == 0) return;

        //el enemigo debe implementar IDamageable para recibir dano
        var damageable = other.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);

        Destroy(gameObject);
    }
}

//interfaz que cualquier entidad dañable debe implementar para no acoplarse
public interface IDamageable
{
    void TakeDamage(int amount);
}