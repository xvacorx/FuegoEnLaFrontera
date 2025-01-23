using UnityEngine;

public class Uzi : Firearm
{
    public override void Shoot()
    {
        if (firepoint == null)
        {
            Debug.LogError($"FirePoint no asignado en {gameObject.name}");
            return;
        }

        if (Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + 1f / weaponData.fireRate;
            currentAmmo--;

            float spreadAngle = Random.Range(-weaponData.spread, weaponData.spread);
            GameObject bullet = PoolManager.Instance.RequestObject("Bullet", weaponData.bulletPrefab.name);

            if (bullet != null)
            {
                bullet.transform.position = firepoint.position;
                bullet.transform.rotation = firepoint.rotation * Quaternion.Euler(0, 0, spreadAngle);
            }
            else
            {
                Debug.LogError("No se pudo obtener un proyectil del pool.");
            }
        }
    }
}
