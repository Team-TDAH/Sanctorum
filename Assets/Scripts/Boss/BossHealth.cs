using UnityEngine;
public class BossHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    //nombre x defecto si no se asigna un nombre al boss
    [SerializeField] private string bossName = "Boss";
    [SerializeField] private BossHealthChannel bossChannel;
    //lo del estado runtime
    private int currentHealth;
    private bool isDead;
    //para que comience "neutro", luego de la charla apareceria la barra de vida y la pelea
    private bool fightStarted;
    //para q se registre que fue derrotado y no vuelva a aparecer como enemigo(asi con cada jefe)
    private BossRegister bossRegister;
    public float HealthPercent => (float)currentHealth / maxHealth;
    public bool IsDead => isDead;
    private void Awake()
    {
        bossRegister = GetComponent<BossRegister>();
        currentHealth = maxHealth;
    }
    private void Start()
    {
        //para lo obvio, que no muestre la barra si ya fue derrotado
        if (bossRegister != null && bossRegister.IsAlreadyDefeated()) return;

        //saque la barra y demas para que aparezca cuando empieza la pelea, no en start
    }
    //esto deberia llamar el boss cuando terminen de hablar, para iniciar la pelea(antes estaba esot en start)
    public void StartFight()
    {
        if (fightStarted || isDead) return;

        fightStarted = true;
        bossChannel?.RaiseFightStarted(bossName, maxHealth);
        bossChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }
    //----------------------------------------------------------------------------------------------




    //IDamageable
    public void TakeDamage(int amount)
    {
        //para q no pued recibir daño mientras este "neutro"
        if (!fightStarted || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        bossChannel?.RaiseHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        isDead = true;
        bossChannel?.RaiseDefeated();
    }
    //lo llama el boss cuando temrina la secuencia d emuerte
    public void CompleteDeath()
    {
        bossRegister?.RegisterDefeated();
        Destroy(gameObject);
    }
}