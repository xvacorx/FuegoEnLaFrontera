using UnityEngine;
public class WeaponPickup : MonoBehaviour
{
    private PlayerWeapon playerWeapon; // Referencia al script de manejo de armas del jugador
    private Firearm weaponOnGround;   // Referencia al arma en el suelo
    private bool isNearWeapon;        // Indica si el jugador está cerca de un arma
    private void Start()
    {
        playerWeapon = GetComponent<PlayerWeapon>();
    }
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

        // Limpiar la referencia al arma en el suelo
        weaponOnGround = null;
        isNearWeapon = false;
    }

    private void DropCurrentWeapon()
    {
        Debug.Log("Dropping weapon");

        // Soltar el arma equipada
        playerWeapon.DropWeapon(playerWeapon.currentWeapon);
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