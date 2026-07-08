using UnityEngine;

//para q respawnee dond e debe al inciar
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;


    private void Start()
    {
        if (SaveLoadManagerJson.Instance == null) return;

        string savedId = SaveLoadManagerJson.Instance.LastCheckpointId;
        if (string.IsNullOrEmpty(savedId)) return;

        //buscamos entre todos los checkpoints de la escena el que coincida con el guardado
        Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>();
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.CheckpointId == savedId)
            {
                playerController.transform.position = checkpoint.transform.position;
                return;
            }
        }
        //si no se encontro, el checkpoint es de otra escena y el player queda en su posicion inicial
    }
}