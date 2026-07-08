using UnityEngine;

//IA del boss del rio de espiritus (Caronto / el Navegante) - VERSION 1
//por ahora: vida conectada al canal, mira al player, y el ataque basico (Faroles del Umbral)
//el resto de las habilidades del gdd se van sumando de a una
public class BossNavegante : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private HealthChannel healthChannel; //el asset BossHealthChannel, NO el del player

    [Header("Referencias")]
    [SerializeField] private PlayerHealth player;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Faroles del Umbral (basico)")]
    [SerializeField] private float cdBasico = 0.3f; //gdd
    [SerializeField] private Transform[] faroles;
    [SerializeField] private GameObject orbeFarolPrefab;

    //estado runtime
    private int currentHealth;
    private bool peleaTerminada;
    private float tBasico;

    private void Start()
    {
        currentHealth = maxHealth;
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (peleaTerminada || player == null) return;

        MirarAlJugador();

        //timer del basico: cuenta para arriba y dispara al llegar al cd
        tBasico += Time.deltaTime;
        if (tBasico >= cdBasico)
        {
            tBasico = 0f;
            FarolesDelUmbral();
        }
    }

    //----------------------------------------------------------------------------------------------
    //IDamageable: por aca entra el danio de los ataques del player

    public void TakeDamage(int amount)
    {
        if (peleaTerminada) return;

        //el 1% del gdd: la vida nunca baja de ahi, el boss no muere
        int umbralFinal = Mathf.CeilToInt(maxHealth * 0.01f);
        currentHealth = Mathf.Max(currentHealth - amount, umbralFinal);
        healthChannel?.RaiseHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= umbralFinal)
        {
            //aca despues va la secuencia de victoria completa (frase, moneda, mascota)
            peleaTerminada = true;
            Debug.Log("[Navegante] Has pagado el precio.");
        }
    }

    //el boss no se cura, pero lo dejo por si la interfaz crece.
    public void Heal(int amount) { }

    //----------------------------------------------------------------------------------------------

    //basico: un orbe dirigido al player que sale de un farol random de la sala.
    private void FarolesDelUmbral()
    {
        if (faroles == null || faroles.Length == 0 || orbeFarolPrefab == null) return;

        Transform farol = faroles[Random.Range(0, faroles.Length)];
        Vector2 dir = (player.transform.position - farol.position).normalized;
        GameObject orbe = Instantiate(orbeFarolPrefab, farol.position, Quaternion.identity);
        orbe.GetComponent<OrbeFarol>().Lanzar(dir);
    }

    private void MirarAlJugador()
    {
        //ojo: si el sprite mira para la izquierda por defecto, hay que dar vuelta la comparacion
        sprite.flipX = player.transform.position.x < transform.position.x;
    }
}