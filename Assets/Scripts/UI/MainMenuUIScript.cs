using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIScript : MonoBehaviour
{
    //escena de gameplay a cargar si es partida nueva o no hay guardado
    [SerializeField] private string firstSceneName = "SceneTest2";
    //lo llama el boton playgame
    public void PlayGame()
    {
        string targetScene = firstSceneName;
 
        //si hay guardado, leemos la escena donde quedo la partida
        if (PlayerPrefs.HasKey("savefile"))
        {
            string json = PlayerPrefs.GetString("savefile", "");
            SaveData data = JsonUtility.FromJson<SaveData>(json);
 
            //si el guardado tiene una escena valida, vamos ahi en vez de la inicial
            if (data != null && !string.IsNullOrEmpty(data.currentScene))
                targetScene = data.currentScene;
        }
 
        SceneManager.LoadScene(targetScene);
    }
    public void OnContinuePressed()
    {
        Debug.Log("todavia no hice el sistema de guardado");
    }
    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
