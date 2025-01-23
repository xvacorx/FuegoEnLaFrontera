using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Firearm weaponOnGround;

    private InputAction interactAction;
    private InputAction dropAction;

    private void Awake()
    {
        var inputActions = new InputSystem_Actions();
        interactAction = inputActions.Player.Interact;
        dropAction = inputActions.Player.Drop;

        interactAction.performed += _ => TryPickupWeapon();
        dropAction.performed += _ => TryDropWeapon();
    }

    private void OnEnable()
    {
        interactAction.Enable();
        dropAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
        dropAction.Disable();
    }

    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        if (playerWeapon == null)
        {
            Debug.LogError("PlayerWeapon component is missing on the player.");
        }
    }

    private void TryPickupWeapon()
    {
        if (weaponOnGround == null || weaponOnGround == playerWeapon.currentWeapon) return;

        if (playerWeapon.currentWeapon == null)
        {
            playerWeapon.EquipWeapon(weaponOnGround);
            weaponOnGround = null;
        }
        else
        {
            playerWeapon.EquipWeapon(weaponOnGround);
            weaponOnGround = null;
        }
    }

    private void TryDropWeapon()
    {
        if (playerWeapon.currentWeapon == null) return;

        playerWeapon.DropWeapon();
        TryPickupWeapon();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Firearm firearm) && firearm != playerWeapon.currentWeapon)
        {
            weaponOnGround = firearm;
            if (playerWeapon.currentWeapon == null)
            {
                TryPickupWeapon();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out Firearm firearm) && firearm != playerWeapon.currentWeapon)
        {
            weaponOnGround = firearm;
            if (playerWeapon.currentWeapon == null)
            {
                TryPickupWeapon();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Firearm firearm) && firearm == weaponOnGround)
        {
            weaponOnGround = null;
        }
    }
}
