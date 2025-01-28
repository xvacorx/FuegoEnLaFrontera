using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Weapon currentWeapon;  // Cambiado de Firearm a Weapon
    private InputAction attackAction;
    private InputAction throwAction;

    private void Awake()
    {
        var inputActions = new InputSystem_Actions();
        attackAction = inputActions.Player.Attack;
        throwAction = inputActions.Player.Throw;
    }

    private void OnEnable()
    {
        attackAction.Enable();
        throwAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.Disable();
        throwAction.Disable();
    }

    // Cambiado para aceptar cualquier tipo de Weapon
    public void EquipWeapon(Weapon weaponOnGround)
    {
        if (currentWeapon != null) DropWeapon();

        currentWeapon = weaponOnGround;
        AttachWeapon(currentWeapon);
    }

    public void DropWeapon()
    {
        if (currentWeapon == null) return;

        DetachWeapon(currentWeapon);
        currentWeapon = null;
    }

    // Cambiado para aceptar cualquier tipo de Weapon
    private void AttachWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(weaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        if (weapon.TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;
        if (weapon.TryGetComponent(out Collider2D collider))
        {
            collider.enabled = false;
            collider.isTrigger = true;
        }
    }

    // Cambiado para aceptar cualquier tipo de Weapon
    private void DetachWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(null);
        weapon.transform.position = transform.position + transform.forward;

        if (weapon.TryGetComponent(out Rigidbody2D rb)) rb.simulated = true;
        if (weapon.TryGetComponent(out Collider2D collider)) collider.enabled = true;
    }

    private void Update()
    {
        if (currentWeapon != null)
        {
            if (attackAction.IsPressed())
            {
                currentWeapon.Attack();  // Ahora llama a Attack() en lugar de Shoot()
            }

            if (throwAction.triggered)
            {
                ThrowWeapon();
            }
        }
    }

    private void ThrowWeapon()
    {
        if (currentWeapon == null) return;

        Vector2 throwDirection = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position).normalized;
        float throwForce = 25f;

        currentWeapon.Throw(throwDirection, throwForce);  // Lanza cualquier tipo de Weapon
        currentWeapon = null; // Ya no tienes el arma equipada
    }
}
