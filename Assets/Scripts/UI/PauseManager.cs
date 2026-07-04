using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private PlayerInput playerInput;
    private InputAction pauseAction;
    private bool isPaused;

    //necesario porque al pausar me estaba creando orbes de luz al apretar las opciones del menu de pausa
    //no me gusto para nada la solucion, pero es la unica forma de que el dash no se consuma mientras este en la pausa y aprete dash
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private PlayerController playerController;
 
 
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
            TogglePause();
    }
    private void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
        abilityManager.InputEnabled = !isPaused;
        playerController.InputEnabled = !isPaused; 
    }

    
    public void Resume()
    {
        TogglePause();
    }
        public void BackToMainMenu()
    {
        TogglePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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