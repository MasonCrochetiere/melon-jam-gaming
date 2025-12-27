using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.buttonClick);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.buttonClick);
    }

    public void ButtonHover()
    {
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.buttonHover);
    }
}
