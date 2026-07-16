using UnityEngine;
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityDuration = 1f;//funciona bien con este tiempo de invulnerabilidad
    //parpadeo(no esta funcionando muy bien con el skin)
    private float blinkInterval = 0.1f;
    [SerializeField] private HealthChannel healthChannel;
    //parece que no irá, pero lo dejare por ahora
    public bool IsShielded { get; set; }
    //estado runtime para guardado
    private int currentHealth;
    private bool isInvulnerable;
    private float invulnerabilityTimer;
    private float blinkTimer;
    //debuff para el spectral charge de angevin pero se podra usar tambien a futuro para otros bosses
    private float damageMultiplier = 1f;
    private float debuffTimer;
    //referencia al sprite para el efecto de parpadeo, puede ser null si no hay sprite
    private SpriteRenderer spriteRenderer;
    //para agregar un iconito o algo en la ui que diga que el efecto todavia esta activo(y capaz agregar un fill como en las habilidades)
    public bool HasDamageDebuff => debuffTimer > 0f;
    //para q la ui muestre el contador de cuanto falta
    public float DebuffTimeRemaining => debuffTimer;

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
        //contador basico para el debuff, q dure lo q tenga q durar -----------Aca hice cambio, luego comprobar si el tiempo sale bien
        if (debuffTimer > 0f)
        {
            debuffTimer -= Time.deltaTime;
            if (debuffTimer <= 0f)
                damageMultiplier = 1f;
        }
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
        //el escudo y iframes bloquean ataques
        if (isInvulnerable) return;
        if (IsShielded) return;
        //aplico el debuf al daño recibido
        int finalDamage = Mathf.RoundToInt(amount*damageMultiplier);
        //para separar, luego veo si funciona(puse amount en vez de finalDamage que ACABO de calcular)
        currentHealth = Mathf.Max(0, currentHealth - finalDamage);
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            healthChannel?.RaiseDeath();
            return;
        }
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
        damageMultiplier = 1f;
        debuffTimer = 0f;
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
    //-------------------------------
    //para q lo llame la habilidad spectralchargue y cualquiera que aplique este tipo de debuff
    public void ApplyDamageDebuff(float multiplier, float duration)
    {
        damageMultiplier = multiplier;
        debuffTimer = duration;
    }
}