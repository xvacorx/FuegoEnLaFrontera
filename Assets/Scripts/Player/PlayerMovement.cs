using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private void Start() => rb = GetComponent<Rigidbody2D>();

    private void Update()
    {
        movement = InputManager.Instance.MoveAction.ReadValue<Vector2>().normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
