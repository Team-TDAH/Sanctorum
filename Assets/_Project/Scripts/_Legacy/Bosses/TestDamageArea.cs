using UnityEngine;

public class TestDamageArea : MonoBehaviour
{


    [SerializeField] private int damageAmount = 7;
    void OnTriggerStay2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            //verifica si tiene puesto el escudo o no
            if (playerHealth.IsShielded)
            {
                //podria luego agregar un efecto visual o de sonido para indiciar que el escudo esta protegiendo
                return;
            }

            //aplica el daño en caso de no tener escudo, este es un test nada mas
            playerHealth.TakeDamage(damageAmount);
        }
    }
}