using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
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
    [SerializeField] private BoolVariable DoubleJumpUnlocked;

    [SerializeField] private BoolVariable dashUnlocked;

    private float healthEnemy = 30;
    
    public void TakeDamage(int amount)
    {
        //test
        healthEnemy = healthEnemy - amount;
        Debug.Log($"Health enemy: {healthEnemy}");
        if (healthEnemy <= 0)
        {
            //cuando muere este enemigo, consigo el dash, y destruyo el enemigo
            dashUnlocked.Value = true;
            
            DoubleJumpUnlocked.Value = true; // desbloquea el dash

            Destroy(gameObject);
        }
    }
}