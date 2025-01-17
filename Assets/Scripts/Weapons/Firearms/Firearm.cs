using System.Collections;
using UnityEngine;

public abstract class Firearm : Weapon
{
    [SerializeField] protected Transform firepoint;
    [SerializeField] protected int currentAmmo; // Munición actual
    private bool isReloading = false; // Control de recarga
    protected float nextFireTime = 0f; // Tiempo hasta el siguiente disparo

    public bool IsReloading => isReloading;

    protected override void Start()
    {
        base.Start();
        currentAmmo = weaponData.ammoCapacity; // Inicializar munición específica
    }

    public abstract void Shoot();

    protected void HandleShooting()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0 && !isReloading)
        {
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            currentAmmo--;
            SpawnBullet();
        }
    }

    protected virtual void SpawnBullet()
    {
        if (firepoint == null)
        {
            Debug.LogError("FirePoint no asignado en " + gameObject.name);
            return;
        }

        Instantiate(weaponData.bulletPrefab, firepoint.position, firepoint.rotation);
    }

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

    public override void Attack()
    {
        Shoot();
    }
}
