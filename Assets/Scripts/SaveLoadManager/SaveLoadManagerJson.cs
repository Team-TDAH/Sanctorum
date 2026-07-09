using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManagerJson : MonoBehaviour
{
    //referencias a las habilidades desbloqueables, para verificar cuales ya tenes y cuales no
    [SerializeField] private BoolVariable dashUnlocked;
    [SerializeField] private BoolVariable doubleJumpUnlocked;
    [SerializeField] private BoolVariable shieldUnlocked;
    //datos de la memoria, donde guardar y cargar la info
    private SaveData currentData;
    //No me gusta el sistema singleton, pero aca me agiliza mucho si los jefes pueden consultarlo sin referencia
    public static SaveLoadManagerJson Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //lo que me faltaba, es que directamente cargue la partida al comenzar, antes tenia un boton para cargar, que carece de sentido en este tipo de juegos
        LoadGame();
    }

    //cada que quiera, puedo guardar la informacio, debo mejorar esto para que guarde mas que solo las habilidades por ahora
    public void SaveGame()
    {
        currentData.dashUnlocked = dashUnlocked != null && dashUnlocked.Value;
        currentData.doubleJumpUnlocked = doubleJumpUnlocked != null && doubleJumpUnlocked.Value;
        currentData.shieldUnlocked = shieldUnlocked != null && shieldUnlocked.Value;
        string json = JsonUtility.ToJson(currentData, true);
        //  Esto tenia antes, pero no guardaba las partidas en itchio WEB, solo en .exe, asi que cambie a playerprefs al final, una combinacion rara, File.WriteAllText(filePath, json);
        PlayerPrefs.SetString("savefile", json);
        PlayerPrefs.Save();


        //no hace falta pero para testear por las duads
        Debug.Log("Save Game (PlayerPrefs)");
    }
    //no es necesaria su referencia nunca aparte de start, asi que lo dejo tal cual por ahora
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("savefile"))
        {
            string json = PlayerPrefs.GetString("savefile", "");

            currentData = JsonUtility.FromJson<SaveData>(json);

            if (dashUnlocked != null) dashUnlocked.Value = currentData.dashUnlocked;
            if (doubleJumpUnlocked != null) doubleJumpUnlocked.Value = currentData.doubleJumpUnlocked;
            if (shieldUnlocked != null) shieldUnlocked.Value = currentData.shieldUnlocked;

            Debug.Log("Game Loaded");
        }
        else
        {
            //la primera vez tendras que crear el currentdata
            currentData = new SaveData();
            Debug.Log("No savegame found, starting fresh");
        }
    }

    //     ------------------------------------ 

    //para que el jefe pueda llamar a esto al morir
    public void RegisterBossDefeated(string bossId)
    {
        if (currentData.defeatedBosses.Contains(bossId)) return;
        
        currentData.defeatedBosses.Add(bossId);
        SaveGame();
    }
    //consultar si estan muertos al iniciar la partida
    public bool IsBossDefeated(string bossId)
    {
        return currentData.defeatedBosses.Contains(bossId);
    }
    //checkpoints y escena
    //complique mas este script culpta de agregar que guarde el ultimo checkpoint de la escena actual
    public void SetCheckpoint(string checkpointId)
    {
        string sceneName = SceneManager.GetActiveScene().name;
    
        //buscamos si esta escena ya tiene un checkpoint registrado y lo actualizamos
        foreach (var entry in currentData.sceneCheckpoints)
        {
            if (entry.sceneName == sceneName)
            {
                //si ya es el checkpoint activo no hace falta reguardar
                if (entry.checkpointId == checkpointId) return;
    
                entry.checkpointId = checkpointId;
                currentData.currentScene = sceneName;
                SaveGame();
                return;
            }
        }
        //primera vez que esta escena registra un checkpoint (lo mas important)
        currentData.sceneCheckpoints.Add(new SceneCheckpoint
        {
            sceneName = sceneName,
            checkpointId = checkpointId
        });
        currentData.currentScene = sceneName;
        SaveGame();
    }
    //para q al iniciar verifiquen que hay guardado
    public string SavedScene => currentData.currentScene;

        public string GetCheckpointForActiveScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
    
        foreach (var entry in currentData.sceneCheckpoints)
        {
            if (entry.sceneName == sceneName)
                return entry.checkpointId;
        }
        return "";
    }
    







    //IMPORTANTE BORRAR LUEGO, ES PARA BORRAR COSAS GUARDADAS Y EMPEZAR DE 0 PARA TESTEAR COSAS (tambien borrar el boton en el menu de pausa)
    [ContextMenu("Delete Save")]
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("savefile");

        currentData = new SaveData();

        if (dashUnlocked != null) dashUnlocked.Value = false;
        if (doubleJumpUnlocked != null) doubleJumpUnlocked.Value = false;
        if (shieldUnlocked != null) shieldUnlocked.Value = false;

        Debug.Log("Save deleted");
    }
}









/*
using System.IO;
using UnityEngine;
    ---------------------
        SaveLoadManagerJson saveLoad;
        void start()
        {
            saveLoad = FindAnyObjectByType<SaveLoadManagerJson>;
        }
        
        public void SaveGame()
        {
            saveLoad.SaveGame(speed);
        }
        public void LoadGame()
        {
            SaveData loadedData = saveLoad.LoadGame();
            if(loadedData!=null)
            {
                //asignas a la variable el valor guardado de esa variables
                moveSpeed = loadedData.moveSpeed;
            }
        }
        -----------------
public class SaveLoadManagerJson : MonoBehaviour
{

    private string filePath;
    void Start()
    {
        filePath = Application.persistentDataPath + "/savefile.json";
    }

    //metodo para guardar la info
    public void SaveGame(float moveSpeed)
    {
        SaveData data = new SaveData();
        data.moveSpeed = moveSpeed;
        //convierte en json y lo guarda en el path que asignamos en start
        string json = JsonUtility.ToJson(data,true);
        File.WriteAllText(filePath,json);

        Debug.Log("Save Game In: "+filePath);
    }
    //metodo para cargar la informacion
    public SaveData LoadGame()
    {
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game Loaded");
            return data;
        }
        else
        {
            Debug.LogWarning("Could not find savegame");
            return null;
        }
    }
}
*/

