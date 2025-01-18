using UnityEngine;

public class Shotgun : Firearm
{
    public override void Shoot()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            currentAmmo--;

            for (int i = 0; i < weaponData.pelletCount; i++)
            {
                SpawnBulletWithSpread();
            }
        }
    }

    private void SpawnBulletWithSpread()
    {
        if (firepoint == null)
        {
            Debug.LogError("FirePoint no asignado en " + gameObject.name);
            return;
        }

        // Calcular el ángulo de dispersión
        float spreadAngle = Random.Range(-weaponData.spread, weaponData.spread);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);

        // Solicitar un objeto del pool
        GameObject bullet = PoolManager.Instance.RequestObject("Bullet", weaponData.bulletPrefab.name);

        if (bullet != null)
        {
            // Configurar posición y rotación del proyectil
            bullet.transform.position = firepoint.position;
            bullet.transform.rotation = firepoint.rotation * spreadRotation;
        }
        else
        {
            Debug.LogError("No se pudo obtener un proyectil del pool.");
        }
    }
}
