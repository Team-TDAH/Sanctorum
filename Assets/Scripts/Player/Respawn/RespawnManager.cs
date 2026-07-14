using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//no me agrada el tener que poner otro gameeoject con un script para manejar el respawn, pero es la forma mas correcta
//Queria hacer todo esto en el playerHealth.cs pero seria cargar el scripot con responsabilidades que no le corresponderian
public class RespawnManager : MonoBehaviour
{
    [SerializeField] private HealthChannel healthChannel;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;
    //panel con el texto "you die!", empieza desactivado, al morir aparecera por unos segundos y luego respawn
    [SerializeField] private GameObject deathPanel;
    //lugar donde respawneara, todavia no tenemos claro si sera en un unico lugar estilo sala central o en distintos respawns (no debe tener nada el gameobject del point)
    private Vector3 initialPosition;

    //con start deberia funciona bien, pero aveces daba error, asi que esperara un frame y me quito dramas
    private IEnumerator Start()
    {
        yield return null;
        initialPosition = playerController.transform.position;
    }

    private void OnEnable()
    {
        if (healthChannel != null)
            healthChannel.OnDeath += HandleDeath;
    }
    private void OnDisable()
    {
        if (healthChannel != null)
            healthChannel.OnDeath -= HandleDeath;
    }

    //corrutina para que espere los 5 segundos o los que coloque al final antes de respawnear (luego tendre que coordinar con la animacion de muerte y que los enemigos no sigan atacando)
    private void HandleDeath()
    {
        StartCoroutine(DeathSequence());
    }
    private IEnumerator DeathSequence()
    {
        //empieza la secuencia mostrando el panel del mensaje de muerte
        if (deathPanel != null)
            deathPanel.SetActive(true);

        //como ya murio, desactivamos el control del player
        playerController.InputEnabled = false;
        playerController.AbilityManager.InputEnabled = false;
        //------
        //aca deberia poner la animacion de muerte en un futuro cuando tenga los assets
        //------
        yield return new WaitForSeconds(5f);
        //cambie todo esto porque al morir contra el jefe prefiero que cargue la escena
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Para q busque el ultimo respawn point, pero en caos de que no tengo ningnuo guardado, puedo reutilizar el checkpoint que habia hecho al comienzo
    private Vector3 GetRespawnPosition()
    {
        if (SaveLoadManagerJson.Instance != null)
        {
            string savedId = SaveLoadManagerJson.Instance.GetCheckpointForActiveScene();
            if (!string.IsNullOrEmpty(savedId))
            {
                Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>();
                foreach (var checkpoint in checkpoints)
                {
                    if (checkpoint.CheckpointId == savedId)
                        return checkpoint.transform.position;
                }
            }
        }
        //sin checkpoint tocado todavia, usamos el punto fijo de siempre
        return initialPosition;
    }
}