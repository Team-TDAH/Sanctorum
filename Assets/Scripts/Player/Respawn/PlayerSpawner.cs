using UnityEngine;
using System.Collections;

//para q respawnee dond e debe al inciar
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;


    // *** FIX: Start ahora es coroutine para poder apagar la bandera despues de que la fisica proceso el teletransporte ***
    private IEnumerator Start()
    {
        if (SaveLoadManagerJson.Instance == null) yield break;

        string savedId = SaveLoadManagerJson.Instance.GetCheckpointForActiveScene();
        if (string.IsNullOrEmpty(savedId)) yield break;

        //buscamos entre todos los checkpoints de la escena el que coincida con el guardado
        Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>();
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.CheckpointId == savedId)
            {
                playerController.transform.position = checkpoint.transform.position;

                //esperamos a que la fisica procese el nuevo lugar antes de reactivar
                yield return new WaitForFixedUpdate();
                yield return null;
                yield break;
            }
        }
        //si no se encontro, el checkpoint es de otra escena y el player queda en su posicion inicial
    }
}