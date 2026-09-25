using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
//el panel empieza SIEMPRE desactivado
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private DialogueChannel channel;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text bodyText;

    //CAMBIOO ahora es un unicaa imagen del que este hablando, ya no hace falta "oscurecer" al q no este hablando
    [SerializeField] private Image speakerPortrait;
    //para q tenga mas espacio de los laterales si hace falta, como todaiva no tengo imagen oficial para las conversaciones, hara falta
    [SerializeField] private Vector2 portraitEdgeOffset = Vector2.zero;

    [Header("Miharu siempre global)")]
    //No hace falta q en el SO tenga q poner la imagne de miharu q siempre aparecera igualmente, quizas luego si cambia expresiones y demas vere
    [SerializeField] private Sprite miharuSprite;
    //x si cambiamos el nombre a algun titulo o algo
    [SerializeField] private string miharuName = "Miharu";
    [SerializeField] private PortraitSide miharuSide = PortraitSide.Left;

    //necesario para congelar al player mientras habla
    [SerializeField] private PlayerController playerController;
    //cualquier boton que ponga para pasar de dialogoa
    private InputAction interactAction;
    private DialogueSO currentDialogue;
    private int currentLineIndex;

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
        //x bug que tenia donde me salteaba el primer dialogo
        if (channel != null && Time.frameCount == channel.LastOpenedFrame)
        {
            return;
        }
        if (interactAction != null && !interactAction.enabled)
            interactAction.Enable();

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            AdvanceLine();
        }
    }
    private void StartDialogue(DialogueSO dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        channel.IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        //ya no hace falta setear ninguna imagen q lo hace en showline()
        // desactivamos todo menos el playerinput, sino no podremos avanzar en el dialogo
        if (playerController != null)
        {
            playerController.InputEnabled = false;
            playerController.AbilityManager.InputEnabled = false;
            // para que al dialogar no siga moviendose la "mira"
            var playerAim = playerController.GetComponent<PlayerAim>();
            if (playerAim != null) playerAim.InputEnabled = false;
        }

        ShowLine(currentDialogue.lines[currentLineIndex]);
    }
    private void AdvanceLine()
    {
        currentLineIndex++;
        // si ya vimos toda la conversacion, se cierra
        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }
        ShowLine(currentDialogue.lines[currentLineIndex]);
    }
    private void ShowLine(DialogueLine line)
    {
        string speakerName;
        Sprite portrait;
        PortraitSide side;
        if (line.isMiharu)
        {
            speakerName = miharuName;
            portrait = miharuSprite;
            side = miharuSide;
        }
        else
        {
            //para q no se rompa todo si me olvide de asignar algo en el SO
            var participants = currentDialogue.participants;
            if (participants == null || line.participantIndex < 0 || line.participantIndex >= participants.Length)
            {
                Debug.LogWarning($"[DialogueUI] ARREGLA ACA '{currentDialogue.name}'");
                return;
            }
            var p = participants[line.participantIndex];
            speakerName = p.speakerName;
            portrait = p.portrait;
            side = p.side;
        }  

        if (nameText != null) nameText.text = speakerName;
        if (bodyText != null) bodyText.text = line.text;
        if (speakerPortrait != null) speakerPortrait.sprite = portrait;
        ApplySide(side);
    }
    //posiciona SOLO donde va el retrato, y le hace flip segun de q lado este
    private void ApplySide(PortraitSide side)
    {
        if (speakerPortrait == null) return;
        var rt = speakerPortrait.rectTransform;
        bool left = side == PortraitSide.Left;
        rt.anchorMin = new Vector2(left ? 0f : 1f, 0f);
        rt.anchorMax = rt.anchorMin;
        rt.pivot = new Vector2(0.5f, 0f);
        float halfW = rt.rect.width * 0.5f;
        float x = left ? (halfW + portraitEdgeOffset.x) : -(halfW + portraitEdgeOffset.x);
        rt.anchoredPosition = new Vector2(x, portraitEdgeOffset.y);
        Vector3 s = rt.localScale;
        s.x = Mathf.Abs(s.x) * (left ? 1f : -1f);
        rt.localScale = s;
    }
    private void CloseDialogue()
    {
        currentDialogue = null;
        dialoguePanel.SetActive(false);
        channel.IsDialogueActive = false;
        channel.RaiseDialogueClosed();
        //da el movimiento al palyer de vuelta
        if (playerController != null)
        {
            playerController.InputEnabled = true;
            //para q el ataque solo este disponible en pelea, no luego de charlar y ya
            playerController.AbilityManager.InputEnabled = playerController.AbilityManager.InFight;
            var playerAim = playerController.GetComponent<PlayerAim>();
            if (playerAim != null) playerAim.InputEnabled = true;
        }
    }
}