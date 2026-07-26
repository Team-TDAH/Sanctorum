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
    //panel con el texto "you die!", empieza desactivado, al morir aparecera por unos segundos y luego respawn(ahora luego de aparecer unos segundos, aparece un menu de muerte)
    [SerializeField] private GameObject deathPanel;
    //el menu q aparecera luego del text you die
    [SerializeField] private GameObject deathMenuPanel;
    //porque aparecia el menu de pausa si lo abriamos en el menu de muerte, y no deberia
    public bool IsDeathMenuActive => deathMenuPanel != null && deathMenuPanel.activeSelf;



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

        //fix de error que al abrir el menu de muerte seguia sin el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        var crosshair = FindAnyObjectByType<Crosshair>();
        if (crosshair != null)
            crosshair.SetVisible(false);

        //faltaban estas dos por eso el player seguia moviendose y disparando al morir
        playerController.InputEnabled = false;
        playerController.AbilityManager.InputEnabled = false;

        //congelamos la "mira" y quitamos su visibilidad tambien
        var playerAim = playerController.GetComponent<PlayerAim>();
        if (playerAim != null) playerAim.InputEnabled = false;
        //------
        //aca deberia poner la animacion de muerte en un futuro cuando tenga los assets
        //------
        yield return new WaitForSeconds(2f);
        //en vez de respawnear directamente, mostramos el menu de muerte
        if (deathPanel != null)
            deathPanel.SetActive(false);
 
        if (deathMenuPanel != null)
            deathMenuPanel.SetActive(true);
    }

    //-------------
    //para los botones del menu(deberia hacer otro script para el menu de muerte, pero no quiero otro script)
    public void OnRevivePressed()
    {
        //para que al respawnear nos devuelva a la ultima escena y no a la actual si el respawn no esta en la escena actual
        string targetScene = SceneManager.GetActiveScene().name;

        if (SaveLoadManagerJson.Instance != null && !string.IsNullOrEmpty(SaveLoadManagerJson.Instance.SavedScene))
            targetScene = SaveLoadManagerJson.Instance.SavedScene;

        SceneManager.LoadScene(targetScene);
    }
    public void OnMainMenuPressed()
    {
        SceneManager.LoadScene("MainMenu");//no es buena practica, pero no quiero mas variables en respawnmanager
    }
    //no creo usarlo, seria raro tener esta opcion en el menu de muerte
    public void OnQuitPressed()
    {
        Application.Quit();
    }

}