using UnityEngine;

//ahora es algo mas confuso, porque es x indice, donde el 0 es el primer hablante y 1 el segundo, para miharu hay que marcar la casilla de miharu
//aca se elije de q lado aparece, distinto a dead as disco pero mejor croe yo
public enum PortraitSide
{
    Left,
    Right
}
//los literalmente participantes de la conversacion
[System.Serializable]
public class DialogueParticipant
{
    public string speakerName;
    public Sprite portrait;
    public PortraitSide side = PortraitSide.Left;
}
//la linea individual de dialogo, que puede ser de miharu o de un participante
[System.Serializable]
public class DialogueLine
{
    public bool isMiharu;
    public int participantIndex;
    [TextArea] public string text;
}
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Conversation")]
public class DialogueSO : ScriptableObject
{
    public DialogueParticipant[] participants;
    public DialogueLine[] lines;
}