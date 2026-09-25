using UnityEngine;

//script para el prefab del proyectil de luz, va en linea recta, hace daño al impactar en algo que este en la capa Enemies o la que coloque, y se destruye luego de 3 seg
public class LightOrbProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask enemyMask;
    //agregando gameFeel
    //hitspot para q pause un poco al impactar 
    [SerializeField] private HitStopChannel hitStopChannel;
    //para que sacuda la camara al impactar(cinemachine trae esto que es muuuy comodo de usar con componentes)
    [SerializeField] private Unity.Cinemachine.CinemachineImpulseSource impulseSource;
    //fuerza del sacudon al impactar, se multiplica por el Default Velocity del componente
    [SerializeField] private float shakeForce = 0.3f;

    //fin de gameFeel
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
    private void Awake()
    {
        if (impulseSource == null)
            impulseSource = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
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
        //para agreagr el gamefeel de congelado al impactar 
        hitStopChannel?.RequestHitStop();
        //lo del mov de camara al impactar
        impulseSource?.GenerateImpulse(shakeForce);
        Destroy(gameObject);
    }
}

//interfaz que cualquier entidad dañable debe implementar para no acoplarse
public interface IDamageable
{
    void TakeDamage(int amount);
}