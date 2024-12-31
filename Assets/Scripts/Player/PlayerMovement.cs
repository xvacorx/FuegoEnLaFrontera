using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private InputSystem_Actions playerInputActions;
    private InputAction moveAction;
    private InputAction lookAction;

    void Awake()
    {
        playerInputActions = new InputSystem_Actions();

        moveAction = playerInputActions.Player.Move;
        lookAction = playerInputActions.Player.Look;
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleLooking();
        HandleMovement();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleMovement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        movement = moveInput.normalized;
    }

    void HandleLooking()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(lookAction.ReadValue<Vector2>());
        Vector3 direction = mousePosition - transform.position;

        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
}