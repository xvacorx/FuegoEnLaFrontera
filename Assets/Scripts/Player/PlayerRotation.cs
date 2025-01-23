using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotation : MonoBehaviour
{
    private InputAction lookAction;

    private void Awake()
    {
        var inputActions = new InputSystem_Actions();
        lookAction = inputActions.Player.Look;
    }

    private void OnEnable() => lookAction.Enable();

    private void OnDisable() => lookAction.Disable();

    private void Update()
    {
        // Obtener la posición del mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(lookAction.ReadValue<Vector2>());
        mousePosition.z = 0; // Asegurar que la posición Z sea 0 en 2D

        // Calcular dirección y rotación
        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar rotación
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}