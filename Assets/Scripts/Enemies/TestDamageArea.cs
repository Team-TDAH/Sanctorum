using UnityEngine;

public class TestDamageArea : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            //verifica si tiene puesto el escudo o no
            if (player.IsShielded)
            {
                Debug.Log("<color=cyan><b>[ESCUDO]</b> ¡Ataque bloqueado por el escudo en área!</color>");
                return;
            }

            //aplica el daño en caso de no tener escudo, este es un test nada mas
            player.TakeDamage(damageAmount);
        }
    }
}