using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private float healthEnemy = 30;
    public void TakeDamage(int amount)
    {
        //test
        healthEnemy = healthEnemy - amount;
        Debug.Log($"Health enemy: {healthEnemy}");
        if (healthEnemy <= 0)
        {
            Destroy(gameObject);
        }
    }
}