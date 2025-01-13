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
        // Si ya hay un arma equipada, dejarla o eliminarla
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // Asignar el arma del suelo como la actual
        currentWeapon = weaponOnGround;

        // Configurar el arma para que sea hija del jugador
        currentWeapon.transform.SetParent(weaponHolder);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        currentWeapon.GetComponent<Rigidbody2D>().simulated = false; // Desactivar físicas si las usa
    }

    private void Update()
    {
        if (currentWeapon == null) return;

        // Disparo al presionar "Fire1"
        if (Input.GetButtonDown("Fire1"))
        {
            currentWeapon.Shoot();
        }

        // Recargar al presionar "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
    }
}
