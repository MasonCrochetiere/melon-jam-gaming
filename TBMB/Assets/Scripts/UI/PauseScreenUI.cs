using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseScreenUI : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    [Header("Input Actions")]
    public InputActionAsset InputActions;
    private InputAction toggleMenu;

    // Initialize Input Actions
    private void OnEnable()
    {
        InputActions.FindActionMap("UI").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("UI").Disable();
    }

    private void Awake()
    {
        toggleMenu = InputSystem.actions.FindAction("ToggleMenu");
    }

    // Handles Player Input
    void Update()
    {
        if (toggleMenu.WasPressedThisFrame())
        {

            if (GameIsPaused)
            {
                Resume();
            }

            else
            {
                Pause();
            }
        }
    }

    // Pauses the Game and Shows UI
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    // Resumes the Game and Hides UI
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
