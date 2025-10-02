using UnityEngine;
using UnityEngine.InputSystem;
using System; // Necesario para System.Action

public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon;
    private Weapon weaponOnGround;
    private Weapon weaponDropped; // Almacena el arma que el jugador acaba de soltar

    // --- Referencias para Subscripción/Desuscripción ---
    private Action<InputAction.CallbackContext> onInteract;
    private Action<InputAction.CallbackContext> onDrop;

    private void Awake()
    {
        // Se inicializan los delegados para referenciar las funciones
        onInteract += TryManualPickup;
        onDrop += TryDropWeapon;
    }

    private void OnEnable()
    {
        var input = InputManager.Instance;
        if (input == null)
        {
            // Si esto sucede, verificar el Script Execution Order en Unity.
            Debug.LogError("InputManager.Instance es NULL. Verifique el orden de ejecución de scripts.");
            return;
        }

        input.InteractAction.Enable();
        input.DropAction.Enable();

        input.InteractAction.performed += onInteract;
        input.DropAction.performed += onDrop;
    }

    private void OnDisable()
    {
        var input = InputManager.Instance;
        if (input != null)
        {
            input.InteractAction.performed -= onInteract;
            input.DropAction.performed -= onDrop;

            input.InteractAction.Disable();
            input.DropAction.Disable();
        }
    }

    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
        if (!playerWeapon) Debug.LogError("PlayerWeapon component missing on " + gameObject.name);
    }

    private void Update()
    {
        // Llama a la comprobación de auto-recogida en cada frame
        CheckForAutoPickup();
    }

    // --- Lógica de Recolección ---

    private void TryManualPickup(InputAction.CallbackContext context)
    {
        // 1. PRIORIDAD AL ARMA NUEVA (weaponOnGround)
        if (weaponOnGround != null && weaponOnGround != weaponDropped)
        {
            // Intenta recoger munición
            if (CheckAndCollectAmmo(weaponOnGround))
            {
                return;
            }

            // Si no recoge munición, realiza el swap con la nueva arma.
            PerformWeaponSwap(weaponOnGround);
            return;
        }

        // 2. PRIORIDAD AL ARMA RECIÉN SOLTADA (weaponDropped) - Recogida Manual Inmediata
        // Solo recogemos si el jugador acaba de soltar el arma Y el arma todavía existe.
        if (weaponDropped != null && weaponOnGround == weaponDropped)
        {
            Weapon targetWeapon = weaponDropped;

            // Eliminamos el bloqueo y la detección antes de recoger
            weaponDropped = null;
            weaponOnGround = null;

            PerformWeaponSwap(targetWeapon);
            return;
        }

        // 3. Caso de recogida simple si no tenía arma equipada
        if (weaponOnGround != null && playerWeapon.currentWeapon == null)
        {
            PerformWeaponSwap(weaponOnGround);
            return;
        }

        Debug.Log("No hay arma nueva o recién soltada en rango para recoger.");
    }

    private void CheckForAutoPickup()
    {
        if (weaponOnGround == null) return;

        // Bloqueo Anti-Recogida: Evita la auto-recogida si es el arma que el jugador acaba de soltar.
        if (weaponOnGround == weaponDropped) return;

        int ammoOnGround = (weaponOnGround is Firearm firearm) ? firearm.currentAmmo : 0;

        if (ShouldAutoPickup(weaponOnGround.weaponData, ammoOnGround))
        {
            if (playerWeapon.currentWeapon == null)
            {
                // Recoger si no lleva arma
                PerformWeaponSwap(weaponOnGround);
            }
            else
            {
                // Auto-recolección de munición
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

        // C. Otros: No se recogen automáticamente.
        return false;
    }

    private bool CheckAndCollectAmmo(Weapon weaponToCollect)
    {
        if (playerWeapon.currentWeapon is Firearm heldFirearm && weaponToCollect is Firearm groundFirearm)
        {
            if (heldFirearm.weaponData.ammoType == groundFirearm.weaponData.ammoType && groundFirearm.currentAmmo > 0)
            {
                int ammoToTransfer = groundFirearm.currentAmmo;
                heldFirearm.CollectAmmo(ammoToTransfer);
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
            // Guardamos el arma actual antes de soltarla (en caso de que sea un swap)
            if (weaponDropped != playerWeapon.currentWeapon)
            {
                weaponDropped = playerWeapon.currentWeapon;
            }

            playerWeapon.DropWeapon(weaponDropped.transform.position);
        }

        playerWeapon.EquipWeapon(weaponToEquip);
        weaponOnGround = null; // Limpiamos la referencia del arma en el suelo después de equiparla
    }

    private void TryDropWeapon(InputAction.CallbackContext context)
    {
        if (playerWeapon.currentWeapon == null)
        {
            Debug.Log("No Weapon");
            return;
        }

        // 1. Guardar la referencia del arma que se va a soltar
        weaponDropped = playerWeapon.currentWeapon;
        Debug.Log($"WeaponDropped: Soltando {weaponDropped.weaponData.weaponName}");

        // 2. Llamar al método de soltar en PlayerWeapon
        playerWeapon.DropWeapon(transform.position);

        // ¡CORRECCIÓN CLAVE!: Se ELIMINÓ 'weaponOnGround = null;' de aquí. 
        // Si hay otro arma en el suelo, 'HandleWeaponTrigger' lo detectará y lo mantendrá.
    }

    // --- Detección de Colisión ---

    // Al entrar en el Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleWeaponTrigger(collision);
    }

    // Mientras se permanece en el Trigger (necesario para que Update funcione)
    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleWeaponTrigger(collision);
    }

    // Al salir del Trigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon))
        {
            // Olvidamos el arma si sale
            if (weapon == weaponOnGround)
            {
                weaponOnGround = null;
            }

            // Permitimos que se pueda recoger de nuevo (resetea el bloqueo de 'weaponDropped')
            if (weapon == weaponDropped)
            {
                weaponDropped = null;
            }
        }
    }

    // Función unificada para manejar la detección de armas
    private void HandleWeaponTrigger(Collider2D collision)
    {
        if (collision.TryGetComponent(out Weapon weapon))
        {
            // Solo registra el arma si no es la equipada
            if (weapon != playerWeapon.currentWeapon)
            {
                weaponOnGround = weapon;
            }
        }
    }
}