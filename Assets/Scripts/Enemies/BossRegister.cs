using UnityEngine;

//script super necesario que me soluciono el sistema de guardado, ya que ahora los jefes permaneceran muertos y la habilidad tambien (agregue un boton en el menu para reiniciar mientras teste lo guardado)
public class BossRegister : MonoBehaviour
{
    //id del jefe, la forma mas facil de identificarlos, al final no termino usando este id en otro lado, pero ponerle un nombre representativo ayudara a futuro
    [SerializeField] private string bossId;
    //aca por si desbloquea alguna habilidad (creo q todos desbloquearan algo, y en el futuro agregare que te den cierta cantidad de monedas)
    [SerializeField] private BoolVariable unlocksAbility;

    private void Start()
    {
        //si detecta q ya esta "muerto", lo destruye antes q nada
        if (SaveLoadManagerJson.Instance != null && SaveLoadManagerJson.Instance.IsBossDefeated(bossId))
            Destroy(gameObject);
    }

    //esto es lo que dbeo llamar en la logica del boss cuando muere o esperar unos segundos con una corrutina para que sea mas natural o con cinematica
    public void RegisterDefeated()
    {
        //desbloquea la habilidad que pusimos como referencia
        if (unlocksAbility != null)
        {
            unlocksAbility.Value = true;
        }   
        //en el momento de ganarle se guarda todo, no es lo mas recomendable pero como son pocas cosas ni se sentira el guardado
        if (SaveLoadManagerJson.Instance != null)
        {
            SaveLoadManagerJson.Instance.RegisterBossDefeated(bossId);
        }
            
    }
}