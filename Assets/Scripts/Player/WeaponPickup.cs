using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public PlayerWeapon playerWeapon; // Referencia al script de manejo de armas del jugador

    private Firearm weaponOnGround;   // Referencia al arma en el suelo
    private bool isNearWeapon;        // Indica si el jugador está cerca de un arma

    private void Update()
    {
        // Recoger el arma al presionar "E"
        if (isNearWeapon && Input.GetKeyDown(KeyCode.E))
        {
            if (weaponOnGround != null)
            {
                PickupWeapon();
            }
        }

        // Desechar el arma equipada al presionar "Q"
        if (Input.GetKeyDown(KeyCode.Q) && playerWeapon.currentWeapon != null)
        {
            DropCurrentWeapon();
        }
    }

    private void PickupWeapon()
    {
        Debug.Log("Picking up weapon");

        // Equipar el arma
        playerWeapon.EquipWeapon(weaponOnGround);

        // Desactivar el Rigidbody2D y colisiones del arma equipada
        Rigidbody2D rb = weaponOnGround.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        Collider2D collider = weaponOnGround.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Limpiar la referencia al arma en el suelo
        weaponOnGround = null;
        isNearWeapon = false;
    }

    private void DropCurrentWeapon()
    {
        Debug.Log("Dropping weapon");

        // Obtener el arma equipada
        Firearm currentWeapon = playerWeapon.currentWeapon;

        // Soltar el arma equipada
        currentWeapon.transform.SetParent(null);
        currentWeapon.transform.position = transform.position + transform.forward * 1f; // Colocarla frente al jugador

        // Reactivar físicas y colisiones
        Rigidbody2D rb = currentWeapon.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        Collider2D collider = currentWeapon.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;

        // Limpiar el arma equipada del jugador
        playerWeapon.currentWeapon = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar si el objeto es un arma
        Firearm firearm = collision.GetComponent<Firearm>();
        if (firearm != null && firearm != playerWeapon.currentWeapon)
        {
            weaponOnGround = firearm;
            isNearWeapon = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Salir del rango del arma
        Firearm firearm = collision.GetComponent<Firearm>();
        if (firearm == weaponOnGround)
        {
            weaponOnGround = null;
            isNearWeapon = false;
        }
    }
}
