using UnityEngine;

//script que detecta los proyectiles en el area del parry, y lo gira hacia donde este mirando el player
public class ParryHitbox : MonoBehaviour
{
    private AbilityManager abilityManager;

    private void Awake()
    {
        abilityManager = GetComponentInParent<AbilityManager>();
        //tuve que sacar la obtencion de la localposition, porque empieza desactivado xd, me tarde en resolverlo pero funciona con normalidad ahor
    }

    //funcion muy completa que utilizare capaz con los assets en un futuro para flipear correctamente 
    public void Activate(float facingDirection)
    {
        float sign = Mathf.Sign(facingDirection);
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Abs(pos.x) * sign;
        transform.localPosition = pos;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * sign;
        transform.localScale = scale;


        //activa la hitbox
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        //desactiva la hitbox
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //para que solo reaccione con proyectiles enemigos, sino problemas
        var projectile = other.GetComponent<EnemyProjectile>();
        if (projectile == null) return;
        //señal para avisar de parry bien hecho, luego agregare alguna pasiva como recompensa por un buen parry
        abilityManager?.NotifyParry(projectile.Direction);
        Destroy(projectile.gameObject);
    }
}