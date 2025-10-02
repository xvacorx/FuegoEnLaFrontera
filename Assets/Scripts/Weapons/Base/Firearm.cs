using System.Collections;
using UnityEngine;

public abstract class Firearm : Weapon
{
    [SerializeField] protected Transform firepoint;

    // --- CORRECCIÓN DE SERIALIZACIÓN ---
    // Usamos campos privados serializados ([SerializeField]) para que Unity y el Editor
    // siempre puedan encontrar y rastrear estos valores de la instancia.
    [SerializeField][HideInInspector] private int _currentAmmo;
    [SerializeField][HideInInspector] private bool _isUnloaded;

    // Propiedades de C# que encapsulan los campos privados serializados.
    // Todo el código del juego debe seguir usando estas propiedades.
    public int currentAmmo
    {
        get => _currentAmmo;
        protected set => _currentAmmo = value;
    }

    public bool isUnloaded
    {
        get => _isUnloaded;
        protected set => _isUnloaded = value;
    }
    // -----------------------------------

    // Controla si esta instancia específica puede ser recargada.
    public bool canBeReloaded { get; private set; } = true;

    protected float nextFireTime = 0f;

    protected override void InitializeWeapon()
    {
        base.InitializeWeapon();

        // Usamos la propiedad, que ahora escribe en el campo privado serializado.
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
                // CHANGE: Use the inverse (1f / RPS) to get the delay time.
                nextFireTime = Time.time + 1f / weaponData.shotsPerSecond;
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
        // Si tienes la estructura de PoolManager, puedes descomentar la siguiente línea:
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
            // Debug.LogError("No se pudo obtener un proyectil del pool."); 
            // Esto es solo un ejemplo, asumiendo que PoolManager.Instance está en el proyecto
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