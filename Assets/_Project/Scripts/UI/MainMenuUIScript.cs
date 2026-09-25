using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIScript : MonoBehaviour
{
    //escena de gameplay a cargar si es partida nueva o no hay guardado
    [SerializeField] private string firstSceneName = "SceneTest2";

    //para poder "atenuarlo" en caso de no tener partida guardada
    [SerializeField] private Button continueButton;
    //para q tambien atenue el text, luego vere como qued(quedo muuuy bien, me convencio completamente, seguire utilizando canvasgroup en un futuro)
    [SerializeField] private CanvasGroup continueCanvasGroup;
    private float disabledAlpha = 0.4f;


    private void Start()
    {
        //para q el boton continue solo funque si hay partida guardada
        bool hasSave = HasSavedGame();
        if (continueButton != null)
            continueButton.interactable = hasSave;

        if (continueCanvasGroup != null)
            continueCanvasGroup.alpha = hasSave ? 1f : disabledAlpha;
    }
    //borra el guardado en caso de dar si, el saveload se encarga de borrar habilidades
    public void OnConfirmNewGame()
    {
        PlayerPrefs.DeleteKey("savefile");
        SceneManager.LoadScene(firstSceneName);
    }
    //funciona igual q como lo tenia antes
    public void OnContinuePressed()
    {
        string targetScene = firstSceneName;
        if (PlayerPrefs.HasKey("savefile"))
        {
            string json = PlayerPrefs.GetString("savefile", "");
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            //para ir directamnte a la escena donde quedo, si es que se quedo en alguna distinta
            if (data != null && !string.IsNullOrEmpty(data.currentScene))
                targetScene = data.currentScene;
        }
        SceneManager.LoadScene(targetScene);
    }
    public void OnQuitPressed()
    {
        Application.Quit();
    }
    //si existe key, existe partida guardada, asi elije como mostrar continue
    private bool HasSavedGame()
    {
        if (!PlayerPrefs.HasKey("savefile")) return false;

        string json = PlayerPrefs.GetString("savefile", "");
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data != null && !string.IsNullOrEmpty(data.currentScene);
    }
}