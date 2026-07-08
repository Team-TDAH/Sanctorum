using UnityEngine;

//al tocarlo el player a este trigger, guarda el checkpoint como ultimo
public class Checkpoint : MonoBehaviour
{
    //id unico, no repetir nunca (estot poniendo zone1_1, zone1_2,zone2_1,zone2_2....)
    [SerializeField] private string checkpointId;
    public string CheckpointId => checkpointId;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //solo le damos bola si lo toco el player
        if (other.GetComponent<PlayerController>() == null) return;

        if (SaveLoadManagerJson.Instance != null)
            SaveLoadManagerJson.Instance.SetCheckpoint(checkpointId);
    }
}