using UnityEngine;

//tambien utiliza IDamageable para poder recibir daño de cualquier fuente que implemente esa interfaz(debo estudiar bien como funciona la herencia)

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 100;

    [Header("I-frames")]
    [SerializeField] private float invulnerabilityDuration = 1f;
    //cada cuanto parpadea el sprite durante los i-frames (puramente visual)
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Canal de eventos")]
    [SerializeField] private HealthChannel healthChannel;

    //Variable del escudo en area, para que todos los sistemas esten al tanto de si se esta protegiendo o no
    public bool IsShielded { get; set; }

    //estado runtime
    private int currentHealth;
    private bool isInvulnerable;
    private float invulnerabilityTimer;
    private float blinkTimer;

    //referencia al sprite para el efecto de parpadeo, puede ser null si no hay sprite
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        //avisamos al canal para que la UI arranque con los valores correctos
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (!isInvulnerable) return;

        //contamos el tiempo de invulnerabilidad
        invulnerabilityTimer -= Time.deltaTime;
        if (invulnerabilityTimer <= 0f)
        {
            EndInvulnerability();
            return;
        }

        //efecto de parpadeo mientras duren los i-frames
        if (spriteRenderer != null)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0f)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                blinkTimer = blinkInterval;
            }
        }
    }


    //----------------------------------------------------------------------------------------------
    //IDamageable

    public void TakeDamage(int amount)
    {
        //culpa de esto, el player se quedaba inmovil al respawnear por otros 5 segundos, esto solucionaria
        if (currentHealth <= 0) return;
        //el escudo y los i-frames bloquean cualquier fuente de dano
        if (isInvulnerable) return;
        if (IsShielded) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            healthChannel?.RaiseDeath();
            //la logica de muerte (reiniciar escena, animacion, etc.) va en otro lado
            //suscribirse a healthChannel.OnDeath desde donde corresponda
            return;
        }

        //si sobrevivio al golpe, activamos los i-frames
        StartInvulnerability();
    }


    //para curarme, todavia no cree nada que utilice esto, pero bueno
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }
    //termine eligiendo que reviva, porque la idea es que respawnee luego de morir en vez de reiniciar la escena o poner un bonton de volver a comenzar, asi que tecnicamente nunca necesitaria la funcion "death"
    //la idea es que cuando muera muestre el panel de muerte con un mensaje y luego de ciertos segundos aparezca en cierto punto de respawn
    public void Respawn()
    {
        currentHealth = maxHealth;
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }

    //----------------------------------------------------------------------------------------------
    //I-frames luego de recibir daño, asi no parece injusto cuando ungolpe te pega, luego ajustar el tiempo de invulnerabiliadd

    private void StartInvulnerability()
    {
        isInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;
        blinkTimer = blinkInterval;
    }

    private void EndInvulnerability()
    {
        isInvulnerable = false;
        //nos aseguramos de que el sprite quede visible al terminar
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
    }


    //----------------------------------------------------------------------------------------------
    //Propiedades de solo lectura para debug o UI sin canal
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvulnerable => isInvulnerable;
}