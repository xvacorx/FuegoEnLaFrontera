using System.Collections;
using UnityEngine;

public abstract class Firearm : Weapon
{
    [SerializeField] protected Transform firepoint;
    public int currentAmmo;
    public bool unloaded;
    protected float nextFireTime = 0f;
    protected override void Start()
    {
        base.Start();
        unloaded = weaponData.unloaded;
        currentAmmo = weaponData.ammoCapacity;
    }

    public abstract void Shoot();

    protected void HandleShooting()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0)
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

        GameObject bullet = PoolManager.Instance.RequestObject("Bullet", weaponData.bulletPrefab.name);

        if (bullet != null)
        {
            bullet.transform.position = firepoint.position;
            bullet.transform.rotation = firepoint.rotation;
        }
        else
        {
            Debug.LogError("No se pudo obtener un proyectil del pool.");
        }
    }

    public override void Attack()
    {
        Shoot();
    }
}
