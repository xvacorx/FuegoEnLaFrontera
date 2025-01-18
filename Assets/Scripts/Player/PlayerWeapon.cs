using UnityEngine;
public class PlayerWeapon : MonoBehaviour
{
    public Transform weaponHolder; // Lugar donde se equipará el arma
    public Firearm currentWeapon; // Referencia al arma actualmente equipada

    /// <summary>
    /// Equipa directamente el arma que se encuentra en el suelo.
    /// </summary>
    /// <param name="weaponOnGround">El arma que el jugador toma del suelo.</param>
    public void EquipWeapon(Firearm weaponOnGround)
    {
        // Si ya hay un arma equipada, dejarla en el suelo
        if (currentWeapon != null)
        {
            DropWeapon(currentWeapon);
        }

        // Asignar el arma del suelo como la actual
        currentWeapon = weaponOnGround;

        // Configurar el arma para que sea hija del jugador
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Desactivar físicas y colisiones del arma equipada
        currentWeapon.GetComponent<Rigidbody2D>().simulated = false;
        currentWeapon.GetComponent<Collider2D>().enabled = false;

        // Cambiar el estado de activación del arma

        //currentWeapon.SetEquippedState(true);
    }

    /// <summary>
    /// Suelta el arma equipada y la coloca en el suelo.
    /// </summary>
    /// <param name="weapon">El arma equipada.</param>
    public void DropWeapon(Firearm weapon)
    {
        weapon.transform.SetParent(null); // Eliminar como hijo del jugador
        weapon.transform.position = transform.position + transform.forward * 1f; // Dejar frente al jugador

        // Reactivar físicas y colisiones
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        Collider2D collider = weapon.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;

        currentWeapon = null;
    }

    private void Update()
    {
        if (currentWeapon == null) return;

        // Disparo al presionar "Fire1"
        if (Input.GetButtonDown("Fire1"))
        {
            currentWeapon.Shoot();
        }
    }
}
