using UnityEngine;
//proyectil raro que no se mueve recto
/// </summary>
public class ZigzagProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    private float zigzagAmplitude = 3.5f;
    private float zigzagFrequency = 7.5f;
    private Vector2 direction;
    //perpendicular a la direccion, sobre esta oscila
    private Vector2 perpendicular;
    private float speed;
    private int damage;
    private float lifetime;
    private float lifeTimer;
    //posicion sobre la linea recta, sin el zigzag
    private Vector2 basePosition;
    public void Initialize(Vector2 dir, float spd, float life, int dmg)
    {
        direction = dir.normalized;
        //la perpendicular es la direccion rotada 90 grados
        perpendicular = new Vector2(-direction.y, direction.x);
        speed = spd;
        lifetime = life;
        damage = dmg;
        lifeTimer = 0f;
        basePosition = transform.position;
    }
    private void Update()
    {
        lifeTimer += Time.deltaTime;
        //avanzamos la posicion base en linea recta
        basePosition += direction * speed * Time.deltaTime;
        //calculo de seno aplicado a una perpendi
        float offset = Mathf.Sin(lifeTimer * zigzagFrequency) * zigzagAmplitude;
        transform.position = basePosition + perpendicular * offset;
        if (lifeTimer >= lifetime)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth?.TakeDamage(damage);
        Destroy(gameObject);
    }
}