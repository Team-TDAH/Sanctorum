using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIScript : MonoBehaviour
{
    public void OnNewGamePressed()
    {
        // TODO: limpiar/crear save data nuevo si aplica
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
