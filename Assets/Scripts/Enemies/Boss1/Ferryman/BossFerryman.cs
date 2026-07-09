using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//bossRegister hace desaparecer al boss luego de derrotarlo y activar al ferryman para que nos pueda llevar a la siguiente escena o luego volver
public class BossFerryman : MonoBehaviour
{
    //para que luego no me olvide donde esta el conetenedor de la escena a la que quiero ir (tambien habra 2 ferryman, uno que te lleve a la siguiente y otro que te traiga a la anterior)
    [SerializeField] private string nextSceneName;
    //el gameobject que debe tener algun mensaje como "E" o "Press E" avisando que se puede interactuar con el mismo, ya luego agregare los dialogos y demas
    [SerializeField] private GameObject interactPrompt;
    private InputAction interactAction;
    //para saber cuando mostrar el mensaje y cuando no, el de la E
    private bool playerInRange;
    private void Awake()
    {
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            //en esete caso no tengo que crear un nuevo input, existe uno llamado interact
            interactAction = playerInput.actions["Interact"];
        }
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
            //ANIMACION DE TRANSICION ACA!!!!!!!!!!!!!!!!!
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