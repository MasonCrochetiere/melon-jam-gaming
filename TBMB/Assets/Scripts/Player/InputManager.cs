using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    public static InputManager instance;
    public InputSystem_Actions actions { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Enable();

        playerController.initializeInput(actions.Player);

        instance = this;
    }

    private void OnDestroy()
    {
        actions.Disable();
    }
}
