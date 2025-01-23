using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private InputAction moveAction;

    private void Awake()
    {
        var inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
    }

    private void OnEnable() => moveAction.Enable();

    private void OnDisable() => moveAction.Disable();

    private void Start() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        // Leer la entrada de movimiento
        movement = moveAction.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        // Aplicar movimiento
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
