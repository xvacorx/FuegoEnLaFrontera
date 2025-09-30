using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Weapon weaponOnGround;
    private Weapon weaponDropped;

    private void OnEnable()
    {
        // NOTA: Se asume que InputManager.Instance y las acciones existen.
        var input = InputManager.Instance;
        input.InteractAction.Enable();
        input.DropAction.Enable();

        input.InteractAction.performed += _ => TryManualPickup();
        input.DropAction.performed += _ => TryDropWeapon();
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        input.InteractAction.performed -= _ => TryManualPickup();
        input.DropAction.performed -= _ => TryDropWeapon();
        input.InteractAction.Disable();
        input.DropAction.Disable();
    }

    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        if (!playerWeapon) Debug.LogError("PlayerWeapon component missing on " + gameObject.name);
    }

    private void Update()
    {
        CheckForAutoPickup();
    }

    // --- Lógica de Recolección ---

    private void TryManualPickup()
    {
        if (weaponOnGround == null) return;

        // 1. Intenta recoger munición. Si tiene éxito, termina aquí (recolección manual de munición).
        if (CheckAndCollectAmmo(weaponOnGround))
        {
            return;
        }

        // 2. Si no pudo recoger munición, procede con el intercambio/recolección manual.
        PerformWeaponSwap(weaponOnGround);
    }

    private void CheckForAutoPickup()
    {
        if (weaponOnGround == null) return;
        if (weaponOnGround == weaponDropped) return;

        int ammoOnGround = (weaponOnGround is Firearm firearm) ? firearm.currentAmmo : 0;

        if (ShouldAutoPickup(weaponOnGround.weaponData, ammoOnGround))
        {
            if (playerWeapon.currentWeapon == null)
            {
                // Recoger Facón o Pistola cargada si no lleva arma.
                PerformWeaponSwap(weaponOnGround);
            }
            else
            {
                // Recolección automática de munición (si tiene un arma en mano)
                CheckAndCollectAmmo(weaponOnGround);
            }
        }
    }

    private bool ShouldAutoPickup(WeaponData dataOnGround, int currentAmmoOnGround)
    {
        // A. REGLA: Armas Cuerpo a Cuerpo (Melee) siempre se recogen automáticamente.
        if (dataOnGround.weaponType == WeaponData.WeaponType.Melee)
        {
            return true;
        }

        // B. REGLA: Armas de fuego (automático solo si tiene al menos 1 bala)
        if (dataOnGround.weaponType == WeaponData.WeaponType.Firearm)
        {
            return currentAmmoOnGround > 0;
        }

        // C. Otros (Throwables, Environment): No se recogen automáticamente.
        return false;
    }

    private bool CheckAndCollectAmmo(Weapon weaponToCollect)
    {
        if (playerWeapon.currentWeapon is Firearm heldFirearm && weaponToCollect is Firearm groundFirearm)
        {
            // Chequea compatibilidad de munición y si el arma del suelo tiene balas
            if (heldFirearm.weaponData.ammoType == groundFirearm.weaponData.ammoType && groundFirearm.currentAmmo > 0)
            {
                int ammoToTransfer = groundFirearm.currentAmmo;

                // 2. Transferir la munición al arma en mano
                heldFirearm.CollectAmmo(ammoToTransfer);

                // 3. Vaciar y bloquear la recarga del arma del suelo.
                groundFirearm.DischargeAndBlockReload();

                Debug.Log($"Recogida munición ({ammoToTransfer}) de tipo {heldFirearm.weaponData.ammoType}.");
                return true;
            }
        }
        return false;
    }

    private void PerformWeaponSwap(Weapon weaponToEquip)
    {
        if (playerWeapon.currentWeapon != null)
        {
            weaponDropped = playerWeapon.currentWeapon;
            // Soltar el arma actual antes de equipar la nueva
            playerWeapon.DropWeapon(weaponDropped.transform.position);
        }

        playerWeapon.EquipWeapon(weaponToEquip);
        weaponOnGround = null;
    }

    private void TryDropWeapon()
    {
        if (playerWeapon.currentWeapon == null) return;

        weaponDropped = playerWeapon.currentWeapon;
        playerWeapon.DropWeapon(transform.position);
    }

    // --- Detección de Colisión ---

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
        }
    }

    private void HandleWeaponTrigger(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon) && weapon != playerWeapon.currentWeapon)
        {
            weaponOnGround = weapon;
        }
    }
}