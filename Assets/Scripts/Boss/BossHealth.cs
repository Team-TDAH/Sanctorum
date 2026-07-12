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
        // *** FIX: si este jefe ya fue derrotado, no mostramos la barra ***
        // *** el BossRegister va a destruir este gameobject, pero su Start puede correr despues del nuestro ***
        if (bossRegister != null && bossRegister.IsAlreadyDefeated()) return;

        //avisamos que empieza la pelea para que aparezca la barra
        bossChannel?.RaiseFightStarted(bossName, maxHealth);
        bossChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }


    //----------------------------------------------------------------------------------------------
    //IDamageable

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        bossChannel?.RaiseHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        bossChannel?.RaiseDefeated();

        //el BossRegister se encarga de desbloquear la habilidad, guardar y activar el ferryman
        bossRegister?.RegisterDefeated();

        //el script de comportamiento del jefe deberia reaccionar a IsDead para su animacion de muerte
        //por ahora lo destruimos directo
        Destroy(gameObject);
    }
}