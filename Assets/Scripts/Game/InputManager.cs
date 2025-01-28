using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputSystem_Actions inputActions;

    public InputAction MoveAction => inputActions.Player.Move;
    public InputAction LookAction => inputActions.Player.Look;
    public InputAction AttackAction => inputActions.Player.Attack;
    public InputAction ThrowAction => inputActions.Player.Throw;
    public InputAction InteractAction => inputActions.Player.Interact;
    public InputAction DropAction => inputActions.Player.Drop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    private void OnDestroy() => inputActions.Disable();
}
