using UnityEngine;

public class Enemy2 : MonoBehaviour, IDamageable
{

    /*
    Estas lineas desbloquean el dash en cualquier script que quiera, sin necesidad de tener referencias del player
    [SerializeField] private BoolVariable dashUnlocked;
    dashUnlocked.Value = true;

    y esto para desbloquear el doble salto
    [SerializeField] private BoolVariable DoubleJumpUnlocked;
    DoubleJumpUnlocked.Value = true;
    Obvio luego hay que poner los respectivos SO en el inspector
    */
    [SerializeField] private BoolVariable ShieldUnlocked;

    private float healthEnemy = 50;

    //disparos basicos del enemigo para testear el daño al player y el parry
    [SerializeField] private GameObject projectilePrefab;
    private float fireTimer;



    
    public void TakeDamage(int amount)
    {
        //test
        healthEnemy = healthEnemy - amount;
        Debug.Log($"Health enemy: {healthEnemy}");
        if (healthEnemy <= 0)
        {
            //cuando muere este enemigo, consigue escudo en area
            ShieldUnlocked.Value = true;

            Destroy(gameObject);
        }
    }

    
    //todo hacia abajo es la logica del disparo basico, para testear
        private void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            Shoot();
            fireTimer = 2f;
        }
    }
    private void Shoot()
    {
        if (projectilePrefab == null) return;
 
        var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        proj.GetComponent<EnemyProjectile>()?.Initialize(Vector2.left, 8f, 4f, 20);
    }
}