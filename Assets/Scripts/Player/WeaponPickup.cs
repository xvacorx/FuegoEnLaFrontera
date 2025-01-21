using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Firearm weaponOnGround;
    private bool isNearWeapon;

    private InputAction interactAction;
    private InputAction dropAction;

    private void Awake()
    {
        var inputActions = new InputSystem_Actions();
        interactAction = inputActions.Player.Interact;
        dropAction = inputActions.Player.Drop;
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

    private void Start() => playerWeapon = GetComponent<PlayerWeapon>();

    private void Update()
    {
        if (interactAction.triggered) { Debug.Log("Interaction"); }
        if (dropAction.triggered) { Debug.Log("Drop Action"); }
        if (interactAction.triggered && isNearWeapon && weaponOnGround != null)
        {
            Debug.Log("Weapon Pickup");
            playerWeapon.EquipWeapon(weaponOnGround);
            weaponOnGround = null;
            isNearWeapon = false;
        }

        if (dropAction.triggered && playerWeapon.currentWeapon != null)
        {
            playerWeapon.DropWeapon();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Firearm firearm) && firearm != playerWeapon.currentWeapon)
        {
            weaponOnGround = firearm;
            isNearWeapon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Firearm firearm) && firearm == weaponOnGround)
        {
            weaponOnGround = null;
            isNearWeapon = false;
        }
    }
}
