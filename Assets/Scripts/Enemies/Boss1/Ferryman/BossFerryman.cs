using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// El jefe derrotado convertido en barquero: al interactuar lleva a la siguiente escena.
/// GameObject con Collider2D en modo trigger, empieza desactivado.
/// BossRegister lo activa cuando el jefe muere o cuando ya estaba muerto al cargar.
/// </summary>
public class BossFerryman : MonoBehaviour
{
    //nombre exacto de la escena a cargar, tiene que estar en el build profiles
    [SerializeField] private string nextSceneName;
    //cartel de "presiona E" o similar, opcional, hijo de este gameobject
    [SerializeField] private GameObject interactPrompt;

    private InputAction interactAction;
    private bool playerInRange;


    private void Awake()
    {
        //mismo patron que el pausemanager, el player es de otro prefab
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
            interactAction = playerInput.actions["Interact"];
    }

    private void OnEnable()
    {
        if (interactAction != null) interactAction.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null) interactAction.Disable();
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            //aca ira la animacion de transicion del barco cuando la tengas
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerInRange = true;
        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() == null) return;

        playerInRange = false;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}