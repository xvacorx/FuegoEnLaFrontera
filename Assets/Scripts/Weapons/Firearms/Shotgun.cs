using UnityEngine;

public class Shotgun : Firearm
{
    public override void Shoot()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0 && !IsReloading)
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

        // Calcular el �ngulo de dispersi�n
        float spreadAngle = Random.Range(-weaponData.spread, weaponData.spread);
        Quaternion spreadRotation = Quaternion.Euler(0, 0, spreadAngle);
        Instantiate(weaponData.bulletPrefab, firepoint.position, firepoint.rotation * spreadRotation);

    }
}
