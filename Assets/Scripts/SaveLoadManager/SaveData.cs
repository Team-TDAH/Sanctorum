using System.Collections.Generic;
using UnityEngine;

//guardado por json, me parecio muy facil de aplicar, diria mas que playerprefs, en un futuro vere de hacerlo mas "seguro"
[System.Serializable]
public class SaveData
{ 
    /*
    //para testear, nunca use el sistema de guardado con json
    public float moveSpeed;
    */



    //para ver que habilidades ya desbloqueamos(ir agregando a medida que creamos habilidaes)
    public bool dashUnlocked;
    public bool doubleJumpUnlocked;
    public bool shieldUnlocked;
    //lista de bosses derrotados, para el prototipo la idea es que sean solo 2
    public List<string> defeatedBosses = new();

    //para que spawnee en el ultimo checkpoint y no en la posicion exacta donde guardo, sino podria dar errores y demas
    public string currentScene;
    public string lastCheckpointId;
    
}
