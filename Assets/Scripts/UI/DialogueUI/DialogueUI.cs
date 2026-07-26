using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
//escucha al canal de dialogos que hicimos, avanza al "interactuar" (ya vere q botones pongo), el panel de dialogo empieza SIEMPRE desactivado
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueChannel channel;
    //contenedor de todos los dialogos q se activa y desactiva
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text bodyText;
    //retrato de miharu siempre es el mismo
    [SerializeField] private Image miharuIMG;
    //retrato del "npc" o "boss"
    [SerializeField] private Image npcIMG;
    //solo se asigna una vez por el inspector, es le sprite de miharu
    [SerializeField] private Sprite miharuSprite;
    //cuanto se atenua el retrato del que NO habla
    [SerializeField] private float dimmedAlpha = 0.4f;
    //necesario para congelar al player mientras habla
    [SerializeField] private PlayerController playerController;
    //cualqueira sea el boton que ponga para pasar de dialogo
    private InputAction interactAction;
    private DialogueSO currentDialogue;
    private int currentLineIndex;
    //por las dudas, para ignorar el primer "click" del dialogo, ya que seria el que apretas para empezsar la charla
    private void Awake()
    {
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
            interactAction = playerInput.actions["Interact"];
    }
    private void OnEnable()
    {
        if (channel != null)
            channel.OnDialogueRequested += StartDialogue;
    }
    private void OnDisable()
    {
        if (channel != null)
            channel.OnDialogueRequested -= StartDialogue;
    }
    private void Update()
    {
        if (currentDialogue == null) return;
        //SOLUCION AL FIN, al problema de que salteara el primer dialogo
        if (channel != null && Time.frameCount == channel.LastOpenedFrame)
        {
            return;
        }
        //tengo miedo de agregar esto y q vuelva a haber problemas, pero tengo q probar para el dialogo entre medio de la pelea del boss
        if (interactAction != null && !interactAction.enabled)
            interactAction.Enable();

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            AdvanceLine();
        }
    }
    //para "comenzar" la conversacion
    private void StartDialogue(DialogueSO dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        channel.IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        //para asignas los sprites de cada uno, el de miharu ya esta en este script
        if (npcIMG != null) npcIMG.sprite = dialogue.npcPortrait;
        if (miharuIMG != null) miharuIMG.sprite = miharuSprite;
        //desactivamos todo menos el playerinput, sino no podremos avanzar en el dialogo
        if (playerController != null)
        {
            playerController.InputEnabled = false;
            playerController.AbilityManager.InputEnabled = false;
            //mismo que en DeathSequence del respawnmanager, para que al dialogar no siga moviendose la "mira"
            var playerAim = playerController.GetComponent<PlayerAim>();
            if (playerAim != null) playerAim.InputEnabled = false;
        }

        ShowLine(currentDialogue.lines[currentLineIndex]);
    }
    //para "avanzar" en la conversacion
    private void AdvanceLine()
    {
        currentLineIndex++;
        //si ya vimos toda la conversacion, se cierra 
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }
        ShowLine(currentDialogue.lines[currentLineIndex]);
    }
    private void ShowLine(DialogueLine line)
    {
        if (nameText != null) nameText.text = line.speakerName;
        if (bodyText != null) bodyText.text = line.text;
        bool miharuTalking = line.speaker == DialogueSpeaker.Miharu;
        SetPortraitHighlight(miharuIMG, miharuTalking);
        SetPortraitHighlight(npcIMG, !miharuTalking);
    }
    //para atenuar la imagen del q no este hablando
    private void SetPortraitHighlight(Image portrait, bool talking)
    {
        if (portrait == null) return;
        Color c = portrait.color;
        c.a = talking ? 1f : dimmedAlpha;
        portrait.color = c;
    }
    private void CloseDialogue()
    {
        currentDialogue = null;
        dialoguePanel.SetActive(false);
        channel.IsDialogueActive = false;
        channel.RaiseDialogueClosed();
        //vuelve a poder moverse
        if (playerController != null)
        {
            playerController.InputEnabled = true;
            //fix para que el ataque solo este disponible en pelea, y no luego del dialogo(y hubieron problemas con la charla en mitad de pelea)
            playerController.AbilityManager.InputEnabled = playerController.AbilityManager.InFight;
            var playerAim = playerController.GetComponent<PlayerAim>();
            if (playerAim != null) playerAim.InputEnabled = true;
        }
    }
}