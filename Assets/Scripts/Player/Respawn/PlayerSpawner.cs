using UnityEngine;
using System.Collections;

//para q respawnee dond e debe al inciar
public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    //corrutina necesaria para el tp
    private IEnumerator Start()
    {
        //ahora estan conectados x ID, pense q era mala idea pero es exageradamente util, asi tampoco hay confusiones y puedo usarlo tambien para puertas
        if (!string.IsNullOrEmpty(BossFerryman.PendingConnectionId))
        {
            string targetId = BossFerryman.PendingConnectionId;
            //prevenciones
            BossFerryman.PendingConnectionId = null;
            BossFerryman[] ferrymen = FindObjectsByType<BossFerryman>(FindObjectsInactive.Include);
            foreach (var ferryman in ferrymen)
            {
                if (ferryman.ArrivalPoint == null) continue;
                if (ferryman.ConnectionId != targetId) continue;
                playerController.transform.position = ferryman.ArrivalPoint.position;
                yield return new WaitForFixedUpdate();
                yield return null;
                yield break;
            }
        }
        if (SaveLoadManagerJson.Instance == null) yield break;
        string savedId = SaveLoadManagerJson.Instance.GetCheckpointForActiveScene();
        if (string.IsNullOrEmpty(savedId)) yield break;
        //buscamos entre todos los checkpoints a el q coincida
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
        //si no se encontro el player aparece en su posicion inicial del editor
    }
}