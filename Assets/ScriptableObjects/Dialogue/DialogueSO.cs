using UnityEngine;

//para ver quien resaltar, dependiendo de quien hable
public enum DialogueSpeaker
{
    Miharu, 
    NPC     //cualquier npc o boss, o luego vere como seria con monologos
}
//una linea individual del dialogo
[System.Serializable]
public class DialogueLine
{
    public DialogueSpeaker speaker;
    [TextArea] public string text;
    //el nombre de quein sea con quien estemos hablamos
    public string speakerName;
}

//igual q con abilitySo, click derecho en assets y vmaos a dialogue y luego conversacion
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Conversation")]
public class DialogueSO : ScriptableObject
{
    //la iamgen del que sea con quien hablemos, el de miharu no hace falta aca, luego lo deberia meter en algun lado asi siempre es el mismo
    public Sprite npcPortrait;
    //las lines en "total" orden para q funque
    public DialogueLine[] lines;
}