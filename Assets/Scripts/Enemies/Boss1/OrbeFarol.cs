using UnityEngine;

//orbe de energia oscura que sale de los faroles de la sala (Faroles del Umbral, el basico del boss)
//hecho a imagen y semejanza de EnemyProjectile, pero con el danio amplificable por el debuff
public class OrbeFarol : MonoBehaviour
{
    [SerializeField] private float velocidad = 9f;
    [SerializeField] private int danio = 20; //gdd
    [SerializeField] private float vidaMaxima = 5f; //si no pega nada se destruye solo
    [SerializeField] private LayerMask playerMask; //igual que en EnemyProjectile

    private Vector2 dir;

    //para probarlo sin boss: instancia en escena, play, click derecho al componente
    [ContextMenu("Probar disparo hacia la izquierda")]
    private void Probar()
    {
        Lanzar(Vector2.left);
    }

    public void Lanzar(Vector2 direccion)
    {
        dir = direccion.normalized;
        Destroy(gameObject, vidaMaxima);
    }

    private void Update()
    {
        transform.Translate(dir * velocidad * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //solo reacciona con lo que este en la capa del player
        if (((1 << other.gameObject.layer) & playerMask) == 0) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            //el danio pasa por el debuff de fragilidad antes de llegar (si no hay debuff, va normal)
            ph.TakeDamage(DebuffFragilidad.Amplificar(ph.gameObject, danio));
        }
        Destroy(gameObject);
    }

}