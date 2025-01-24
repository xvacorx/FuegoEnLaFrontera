using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Firearm weaponOnGround;
    private Firearm weaponDropped;
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
        if (weaponOnGround == null) return;

        playerWeapon.EquipWeapon(weaponOnGround);
        playerWeapon.currentWeapon.unloaded = true;
        weaponOnGround = null;
    }
    private void TryDropWeapon()
    {
        if (playerWeapon.currentWeapon == null) return;
        weaponDropped = playerWeapon.currentWeapon;
        playerWeapon.DropWeapon();
    }
    private void TryReload()
    {
        if (playerWeapon.currentWeapon != null && weaponOnGround != null)
        {
            if (playerWeapon.currentWeapon.weaponData.ammoType == weaponOnGround.weaponData.ammoType)
            {
                if (weaponOnGround.currentAmmo > 0 && weaponOnGround.unloaded == false)
                {
                    playerWeapon.currentWeapon.currentAmmo += weaponOnGround.currentAmmo;
                    weaponOnGround.currentAmmo = 0;
                    weaponOnGround.unloaded = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Firearm firearm) && firearm != playerWeapon.currentWeapon)
        {
            TryReload();
            weaponOnGround = firearm;
            if (playerWeapon.currentWeapon == null && firearm != weaponDropped && firearm.currentAmmo > 0)
            {
                TryPickupWeapon();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out Firearm firearm) && firearm != playerWeapon.currentWeapon)
        {
            TryReload();
            weaponOnGround = firearm;
            if (playerWeapon.currentWeapon == null && firearm != weaponDropped && firearm.currentAmmo > 0)
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
            weaponDropped = null;
        }
    }
}