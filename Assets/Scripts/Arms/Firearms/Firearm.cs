using System.Collections;
using UnityEngine;

public abstract class Firearm : MonoBehaviour
{
    public WeaponData weaponData; // Datos del arma (ScriptableObject)
    protected int currentAmmo; // Munición actual
    private bool isReloading = false; // Control de recarga
    protected float nextFireTime = 0f; // Tiempo hasta el siguiente disparo

    // Propiedad para verificar si está recargando
    public bool IsReloading => isReloading;

    private void Start()
    {
        InitializeWeapon();
    }

    // Inicializa el arma
    private void InitializeWeapon()
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponData no asignado en " + gameObject.name);
            return;
        }

        currentAmmo = weaponData.ammoCapacity; // Munición inicial
    }

    // Método abstracto para disparar
    public abstract void Shoot();

    // Maneja la lógica básica de disparo
    protected void HandleShooting()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0 && !isReloading)
        {
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            currentAmmo--;
            SpawnBullet();
        }
    }

    // Instancia la bala
    protected virtual void SpawnBullet()
    {
        if (weaponData.firePoint == null)
        {
            Debug.LogError("FirePoint no asignado en " + gameObject.name);
            return;
        }

        Instantiate(weaponData.bulletPrefab, weaponData.firePoint.position, weaponData.firePoint.rotation);
    }

    // Recarga el arma
    public void Reload()
    {
        if (!isReloading && currentAmmo < weaponData.ammoCapacity)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponData.reloadTime);
        currentAmmo = weaponData.ammoCapacity;
        isReloading = false;
    }
}
