using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Weapon weaponOnGround;  // Cambiado a Weapon para que funcione con cualquier arma
    private Weapon weaponDropped;

    private void OnEnable()
    {
        var input = InputManager.Instance;
        input.InteractAction.Enable();
        input.DropAction.Enable();
        input.InteractAction.performed += _ => TryPickupWeapon();
        input.DropAction.performed += _ => TryDropWeapon();
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        input.InteractAction.Disable();
        input.DropAction.Disable();
    }

    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        if (!playerWeapon) Debug.LogError("PlayerWeapon component missing.");
    }

    private void TryPickupWeapon()
    {
        if (weaponOnGround == null) return;
        playerWeapon.EquipWeapon(weaponOnGround);  // Equipa el arma
        weaponOnGround = null;  // Elimina el arma del suelo una vez recogida
    }

    private void TryDropWeapon()
    {
        if (playerWeapon.currentWeapon == null) return;
        weaponDropped = playerWeapon.currentWeapon;
        playerWeapon.DropWeapon();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleWeaponTrigger(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleWeaponTrigger(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon) && weapon == weaponOnGround)
        {
            weaponOnGround = null;
            weaponDropped = null;
        }
    }

    private void HandleWeaponTrigger(Collider2D collision)
    {
        // Se detecta cualquier tipo de Weapon (no solo Firearm)
        if (collision.TryGetComponent(out Weapon weapon) && weapon != playerWeapon.currentWeapon)
        {
            weaponOnGround = weapon;  // Asigna el arma en el suelo
            if (playerWeapon.currentWeapon == null && weapon != weaponDropped)
            {
                TryPickupWeapon();  // Permite recoger el arma si no hay ninguna equipada
            }
        }
    }
}
