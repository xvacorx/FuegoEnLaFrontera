using UnityEngine;

public abstract class EnvironmentWeapon : Weapon
{
    // Armas fijas o del entorno (e.g., cañones).

    public override void Attack()
    {
        // Lógica para disparar un arma estática (e.g., disparar un cañón fijo).
        Debug.Log($"[{weaponData.weaponName}] atacando el entorno.");
    }
}