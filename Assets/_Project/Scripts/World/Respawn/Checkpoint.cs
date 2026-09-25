using UnityEngine;

//al tocarlo el player a este trigger, guarda el checkpoint como ultimo
public class Checkpoint : MonoBehaviour
{
    //id unico, no repetir nunca (estot poniendo zone1_1, zone1_2,zone2_1,zone2_2....)
    [SerializeField] private string checkpointId;
    public string CheckpointId => checkpointId;
    //al probar este no era el error, public static bool IgnoreTriggers;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        //para q cuando arranque, este totem arranque iluminado(luego preguntare si esto es correcto, porque capaz quiera que empiecen todos apagados)
        RefreshVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //solo le damos bola si lo toco el player
        if (other.GetComponent<PlayerController>() == null) return;

        //queria ver si era uclpa del trigger al tepearme pero no pareceif (IgnoreTriggers) return;

        if (SaveLoadManagerJson.Instance != null)
            SaveLoadManagerJson.Instance.SetCheckpoint(checkpointId);
    
        //para q todos los totems se "refresquen", luego vere si debo cambiarlo a que solo sean los ultimos que verifiquen
        NotifyAllCheckpoints();
    }
    //compara ids y ve si activar o no el bool animator
        public void RefreshVisual()
    {
        if (animator == null || SaveLoadManagerJson.Instance == null) return;
 
        string activeId = SaveLoadManagerJson.Instance.GetCheckpointForActiveScene();
        bool isActive = (activeId == checkpointId);
        animator.SetBool("isActive", isActive);
    }
    //lo que llamo antes, que hace q todos los totems verifiquen
    private void NotifyAllCheckpoints()
    {
        Checkpoint[] all = FindObjectsByType<Checkpoint>();
        foreach (var checkpoint in all)
            checkpoint.RefreshVisual();
    }
}