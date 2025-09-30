using System.Collections;
using UnityEngine;

public abstract class Firearm : Weapon
{
    [SerializeField] protected Transform firepoint;

    // Estado de la instancia. Se mantiene 'protected set'.
    public int currentAmmo { get; protected set; }
    public bool isUnloaded { get; protected set; }

    // Controla si esta instancia específica puede ser recargada.
    public bool canBeReloaded { get; private set; } = true;

    protected float nextFireTime = 0f;

    protected override void InitializeWeapon()
    {
        base.InitializeWeapon();

        isUnloaded = weaponData.unloaded;

        if (isUnloaded)
        {
            currentAmmo = 0;
        }
        else
        {
            currentAmmo = weaponData.initialAmmo;
        }

        currentAmmo = Mathf.Min(currentAmmo, weaponData.ammoCapacity);
        canBeReloaded = true;
    }

    public abstract void Shoot();

    protected void HandleShooting()
    {
        if (Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + 1f / weaponData.fireRate;
                currentAmmo--;

                for (int i = 0; i < weaponData.pelletCount; i++)
                {
                    SpawnBullet();
                }

                // TODO: Llamar al PoolManager para instanciar el efecto de disparo (muzzle flash)
            }
            else
            {
                Debug.Log("¡Clic! Arma vacía.");
            }
        }
    }

    protected virtual void SpawnBullet()
    {
        if (firepoint == null)
        {
            Debug.LogError("FirePoint no asignado en " + gameObject.name);
            return;
        }

        float spreadAngle = Random.Range(-weaponData.spread / 2f, weaponData.spread / 2f);
        Quaternion bulletRotation = firepoint.rotation * Quaternion.Euler(0, 0, spreadAngle);

        // 2. Obtener objeto del Pool
        // NOTA: PoolManager.Instance debe existir en tu proyecto
        GameObject bulletObj = PoolManager.Instance.RequestObject(
            weaponData.projectileCategory,
            weaponData.projectilePoolName
        );

        if (bulletObj != null)
        {
            bulletObj.transform.position = firepoint.position;
            bulletObj.transform.rotation = bulletRotation;

            // 3. Pasar el daño del SO al script del proyectil
            if (bulletObj.TryGetComponent(out Bullet bulletScript))
            {
                bulletScript.InitializeBullet(weaponData.projectileDamage);
            }
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

    // --- LÓGICA DE MUNICIÓN Y ESTADO ---

    /// <summary>
    /// Añade munición al cargador, limitado por la capacidad máxima.
    /// </summary>
    public void CollectAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, weaponData.ammoCapacity);
    }

    /// <summary>
    /// Vacía completamente el arma y la marca como no recargable.
    /// Llamado por WeaponPickup cuando se extrae la munición.
    /// </summary>
    public void DischargeAndBlockReload()
    {
        currentAmmo = 0;
        canBeReloaded = false;
    }

    /// <summary>
    /// Setter público para que WeaponPickup pueda modificar el flag si es necesario.
    /// </summary>
    public void SetCanBeReloaded(bool state)
    {
        canBeReloaded = state;
    }

    // Método de recarga.
    public virtual void Reload(int playerTotalReserveAmmo)
    {
        if (!canBeReloaded)
        {
            Debug.Log("Esta arma ha sido descargada por completo y no se puede recargar.");
            return;
        }

        // TODO: Implementar lógica de recarga
        Debug.Log("Recargando...");
    }
}