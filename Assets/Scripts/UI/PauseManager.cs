using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private PlayerInput playerInput;
    private InputAction pauseAction;
    private bool isPaused;
    //no queria tener que tener estas referencias, pero no queda otra, tenia un bug donde al apretar escape teniendo el menu de settings este no se cerraba pero el game seguia andando
    [SerializeField] private GameObject mainButtons;
    [SerializeField] private GameObject settingsContent;

    //necesario porque al pausar me estaba creando orbes de luz al apretar las opciones del menu de pausa
    //no me gusto para nada la solucion, pero es la unica forma de que el dash no se consuma mientras este en la pausa y aprete dash
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private RespawnManager respawnManager;
    
 
    private void Awake()
    {
        if (playerInput != null)
            //no es ideal ponerlo asi, pero no quiero mas referencias para un "simple" script de pausa
            pauseAction = playerInput.actions["Pause"];
    }
    private void OnEnable()
    {
        if (pauseAction != null) pauseAction.Enable();
    }
    private void OnDisable()
    {
        if (pauseAction != null) pauseAction.Disable();
    }
    private void Update()
    {
        if (pauseAction != null && pauseAction.WasPressedThisFrame())
        {
            //no abrira el menu de pausa si el menu de muerte esta activo
            if (respawnManager != null && respawnManager.IsDeathMenuActive) return;

            TogglePause();
        }
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        //para que se vea el cursor de nuevo, lo agregue luego de agregar el apuntado con mouse
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        abilityManager.InputEnabled = !isPaused;
        playerController.InputEnabled = !isPaused; 
        //mismo que en DeathSequence del respawnmanager, para que al pausar no siga moviendose la "mira"
        var playerAim = playerController.GetComponent<PlayerAim>();
        if (playerAim != null) playerAim.InputEnabled = !isPaused;
        //ahora se cierra tmabien si apretamos escape
        if (!isPaused)
        {
            if (settingsContent != null) settingsContent.SetActive(false);
            if (mainButtons != null) mainButtons.SetActive(true);
        }
    }

    
    public void Resume()
    {
        TogglePause();
    }
        public void BackToMainMenu()
    {
        //tenia un grave bug aca, porque llamaba a la pausa de nuevo
        Time.timeScale = 1f;
        //fix culpa de si vuelvo al menu no recuperaba el cursor en el menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }
}


//esta era la idea pero recorde que no tengo el sistema de inputs viejo, no pense que el input actual seria tan molesto
/*
    [SerializeField] private GameObject pausePanel;
    //me di cuent aahora, que no estaba siguiendo la arquitectura camelCase en las variables,lpm
    [SerializeField] private GameObject settingsPanel;
    private bool isPaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void Resume()
    {
        TogglePause();
    }
    */