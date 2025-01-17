using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName; // Nombre del arma
    public GameObject bulletPrefab; // Prefab de la bala
    public float fireRate; // Disparos por segundo
    public int ammoCapacity; // Capacidad del cargador
    public float reloadTime; // Tiempo de recarga
    public int pelletCount; // Solo para armas como escopetas
    public float spread; // Dispersi�n del disparo (solo escopetas)
}
