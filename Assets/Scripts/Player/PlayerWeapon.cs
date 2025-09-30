// (El script PlayerWeapon.cs se mantiene sin cambios)
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public Transform weaponHolder;
    public Weapon currentWeapon;
    // Asumo que tienes una clase generada para tus acciones de entrada
    private InputSystem_Actions inputActions;
    private InputAction attackAction;
    private InputAction throwAction;

    private void Awake()
    {
        // Asegúrate de que esta línea coincide con el nombre de tu clase de Input System
        inputActions = new InputSystem_Actions();
        attackAction = inputActions.Player.Attack;
        throwAction = inputActions.Player.Throw;
    }

    private void OnEnable()
    {
        attackAction.Enable();
        throwAction.Enable();

        // Asignar el método al evento
        attackAction.performed += OnAttackPerformed;
        throwAction.performed += OnThrowPerformed;
    }

    private void OnDisable()
    {
        // Desasignar el método del evento
        attackAction.performed -= OnAttackPerformed;
        throwAction.performed -= OnThrowPerformed;

        attackAction.Disable();
        throwAction.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // Usar .performed para ataques discretos (melee, revólver)
        // Pero usar IsPressed() en Update() para armas automáticas (Uzi)
        if (currentWeapon != null && !currentWeapon.weaponData.weaponType.Equals(WeaponData.WeaponType.Firearm))
        {
            currentWeapon.Attack();
        }
    }

    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        // El lanzamiento siempre ocurre al soltar el botón
        ThrowWeapon();
    }

    // --- Métodos de Recolección y Soltado (Llamados desde WeaponPickup) ---

    public void EquipWeapon(Weapon weaponOnGround)
    {
        currentWeapon = weaponOnGround;
        AttachWeapon(currentWeapon);
    }

    /// <summary>
    /// Suelta el arma en la posición del jugador para que pueda ser recogida.
    /// Esta es la versión llamada por WeaponPickup.cs cuando hay un intercambio.
    /// </summary>
    /// <param name="position">La posición donde el arma debe caer (normalmente la posición del jugador).</param>
    public void DropWeapon(Vector3 position)
    {
        if (currentWeapon == null) return;

        DetachWeapon(currentWeapon, position);
        currentWeapon = null;
    }

    // NOTA: Mantenemos la versión antigua para compatibilidad si otros scripts la usan.
    public void DropWeapon()
    {
        DropWeapon(transform.position);
    }

    // --- Lógica de Adjuntar/Separar ---

    private void AttachWeapon(Weapon weapon)
    {
        weapon.transform.SetParent(weaponHolder);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        // Resetear el Layer para evitar colisiones accidentales
        weapon.gameObject.layer = 0;

        // Desactivar la física
        if (weapon.TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;

        // Desactivar el collider de recolección
        if (weapon.TryGetComponent(out Collider2D collider))
        {
            collider.enabled = false;
            // El trigger es importante para que no interfiera con el movimiento
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Separa el arma y la coloca en la posición indicada.
    /// </summary>
    private void DetachWeapon(Weapon weapon, Vector3 position)
    {
        weapon.transform.SetParent(null);
        // Usar la posición del jugador para que caiga inmediatamente
        weapon.transform.position = position;

        // Reactivar la física para que el WeaponPickup pueda detectarla
        if (weapon.TryGetComponent(out Rigidbody2D rb)) rb.simulated = true;
        if (weapon.TryGetComponent(out Collider2D collider))
        {
            collider.enabled = true;
            // El collider debe ser un Trigger inicialmente para ser recogido por WeaponPickup
            collider.isTrigger = true;
        }

        // NOTA: El arma en el suelo (Weapon) es responsable de su propio estado de caída/trigger.
    }

    // --- Lógica de Input en Update (para armas automáticas) ---

    private void Update()
    {
        if (currentWeapon != null)
        {
            // Solo atacamos si el botón está presionado Y el arma es de fuego
            if (currentWeapon.weaponData.weaponType.Equals(WeaponData.WeaponType.Firearm) && attackAction.IsPressed())
            {
                currentWeapon.Attack(); // Esto manejará el Uzi (automático) o el Revólver (semiautomático)
            }
        }
    }

    // --- Lógica de Lanzamiento ---

    private void ThrowWeapon()
    {
        if (currentWeapon == null) return;

        // Calcular la dirección de lanzamiento hacia el puntero del mouse
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 throwDirection = (worldMousePosition - transform.position).normalized;

        // Usar la fuerza definida en el Scriptable Object
        float throwForce = currentWeapon.weaponData.throwForce;

        // 1. Lanzar el arma
        currentWeapon.Throw(throwDirection, throwForce);

        // 2. Limpiar la referencia del arma equipada
        currentWeapon = null;
    }
}