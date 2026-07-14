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
    //para el dialogo antes del tp, estbaa pensando en hacer otro script para las puertas o transportes pero al final me quedo con este para casi todo
    [SerializeField] private DialogueChannel dialogueChannel;
    [SerializeField] private DialogueSO dialogue;
    //al cerrarse hace el viaje
    private bool waitingForDialogueEnd;

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
        if (dialogueChannel != null)
            dialogueChannel.OnDialogueClosed += HandleDialogueClosed;
    }
    private void OnDisable()
    {
        if (dialogueChannel != null)
            dialogueChannel.OnDialogueClosed -= HandleDialogueClosed;
    }
    private void Update()
    {
        if (!playerInRange) return;
        if (interactAction == null) return;
        
        // AHORA SI, 03:58 pude terminar este bug que no me dejaba tepearme porque npcdialogue del boss parece que desabilitaba el esta accion
        if (!interactAction.enabled) interactAction.Enable();

        //obviamente, no abrir si ya hay dialogo
        if (dialogueChannel != null && dialogueChannel.IsDialogueActive) return;

        //ignorar input si el frame coincide
        if (dialogueChannel != null && Time.frameCount == dialogueChannel.LastClosedFrame) return;

        if (interactAction.WasPressedThisFrame())
        {
            //si hay dialogo se reproduce
            if (dialogueChannel != null && dialogue != null)
            {
                waitingForDialogueEnd = true;
                dialogueChannel.RequestDialogue(dialogue);
            }
            else
            {
                //sino viaja directo
                Travel();
            }
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
    private void HandleDialogueClosed()
    {
        if (!waitingForDialogueEnd) return;
 
        waitingForDialogueEnd = false;
        Travel();
    }
    private void Travel()
    {
        //!!!!!!!! aca va la animacion de transicion
        SceneManager.LoadScene(nextSceneName);
    }
}