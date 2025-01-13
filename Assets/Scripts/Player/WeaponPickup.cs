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
                playerWeapon.EquipWeapon(weaponOnGround); // Equipar el arma directamente
                weaponOnGround = null;
                isNearWeapon = false;
            }
        }

        // Desechar el arma equipada al presionar "Q"
        if (Input.GetKeyDown(KeyCode.Q) && playerWeapon.currentWeapon != null)
        {
            DropCurrentWeapon();
        }
    }

    private void DropCurrentWeapon()
    {
        // Obtener el arma actualmente equipada
        Firearm currentWeapon = playerWeapon.currentWeapon;

        // Liberar el arma del jugador
        currentWeapon.transform.SetParent(null);
        currentWeapon.transform.position = transform.position + transform.forward; // Dejarla frente al jugador
        currentWeapon.GetComponent<Rigidbody2D>().simulated = true; // Reactivar físicas si las tiene

        // Limpiar la referencia del arma equipada
        playerWeapon.currentWeapon = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar si el objeto es un arma
        Firearm firearm = collision.GetComponent<Firearm>();
        if (firearm != null)
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
