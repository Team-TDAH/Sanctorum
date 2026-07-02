using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIScript : MonoBehaviour
{

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    public void OnNewGamePressed()
    {
        // TODO: limpiar/crear save data nuevo si aplica
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnContinuePressed()
    {
        Debug.Log("todavia no hice el sistema de guardado");
    }
    public void OnSettingsPressed()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
        public void OnSettingsBackPressed()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
