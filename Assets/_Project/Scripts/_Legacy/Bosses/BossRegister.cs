using UnityEngine;

//script super necesario que me soluciono el sistema de guardado, ya que ahora los jefes permaneceran muertos y la habilidad tambien (agregue un boton en el menu para reiniciar mientras teste lo guardado)
public class BossRegister : MonoBehaviour
{
    //id del jefe, la forma mas facil de identificarlos, al final no termino usando este id en otro lado, pero ponerle un nombre representativo ayudara a futuro
    [SerializeField] private string bossId;
    //aca por si desbloquea alguna habilidad (creo q todos desbloquearan algo, y en el futuro agregare que te den cierta cantidad de monedas)
    [SerializeField] private BoolVariable unlocksAbility;
    //barquero que aparece luego de matar al jefe, es el mismo jefe pero con la opcion de interactuar con el y poder viajar a la siguiente escena
    [SerializeField] private GameObject ferryman;

    private void Start()
    {
        //si detecta q ya esta "muerto", lo destruye antes q nada
        //en caso de ya estar muerto, no aparecera el boss de ataque sino directamente el barquero
        if (SaveLoadManagerJson.Instance != null && SaveLoadManagerJson.Instance.IsBossDefeated(bossId))
        {
            //aca apareceria el barquero en vez del boss
            if (ferryman != null) ferryman.SetActive(true);
            Destroy(gameObject);
        }
    }

    //esto es lo que dbeo llamar en la logica del boss cuando muere o esperar unos segundos con una corrutina para que sea mas natural o con cinematica
    public void RegisterDefeated()
    {
        if (unlocksAbility != null)
            unlocksAbility.Value = true;
    
        if (SaveLoadManagerJson.Instance != null)
            SaveLoadManagerJson.Instance.RegisterBossDefeated(bossId);
    
        //logica barq
        if (ferryman != null) ferryman.SetActive(true);
    }
    //para q sepa la ui si debe mostrar o no la barra del jefe
    public bool IsAlreadyDefeated()
    {
        return SaveLoadManagerJson.Instance != null
            && SaveLoadManagerJson.Instance.IsBossDefeated(bossId);
    }
}