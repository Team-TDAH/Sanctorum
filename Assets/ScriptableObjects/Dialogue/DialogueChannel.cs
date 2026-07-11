using System;
using UnityEngine;

//npc publica la conversacion, la ui y demas solo "escuchaN", iguales uqe los canales de habilidades
[CreateAssetMenu(fileName = "DialogueChannel", menuName = "Dialogue/Dialogue Channel")]
public class DialogueChannel : ScriptableObject
{
    //esto dispara al interactuar, es deber de las otras partes "escuchar"
    public event Action<DialogueSO> OnDialogueRequested;
    //esto para avisar que terminaron de hablar, asi activamos de vuelta el movimiento y lo demas
    public event Action OnDialogueClosed;
    //No creo que haga falta, pero para prevenir que haga mas dialogos
    [System.NonSerialized] private bool isDialogueActive;
    [System.NonSerialized] public int LastClosedFrame = -1;
    public bool IsDialogueActive
    {
        get => isDialogueActive;
        set => isDialogueActive = value;
    }
        private void OnEnable()
    {
        isDialogueActive = false;
    }
    public void RequestDialogue(DialogueSO dialogue)
    {
        OnDialogueRequested?.Invoke(dialogue);
    }
    public void RaiseDialogueClosed()
    {
        LastClosedFrame = Time.frameCount;
        OnDialogueClosed?.Invoke();
    }
}