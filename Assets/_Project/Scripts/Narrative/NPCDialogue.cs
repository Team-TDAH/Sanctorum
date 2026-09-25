using UnityEngine;
using UnityEngine.InputSystem;
//va en el npc o boss, parecido al farryman pero dispara dialogo en vez de cambiar de escena
public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private DialogueChannel channel;
    //el dialogueSO con la conversacion del nppc o boss
    [SerializeField] private DialogueSO dialogue;
    //cartel que diga "E" o "presiona E"
    [SerializeField] private GameObject interactPrompt;
    private InputAction interactAction;
    private bool playerInRange;
    private void Awake()
    {
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
        if (!playerInRange)        
        {
            return;
        }
        //no abrimos si ya hay un dialogo en curso
        if (channel != null && channel.IsDialogueActive) return;
        if (channel != null && Time.frameCount == channel.LastClosedFrame) return;
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (channel != null && dialogue != null)
                channel.RequestDialogue(dialogue);
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