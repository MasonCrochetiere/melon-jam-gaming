using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private EventReference buttonClick;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        AudioManager.instance.PlayeOneShot2D(buttonClick);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        AudioManager.instance.PlayeOneShot2D(buttonClick);
    }
}
