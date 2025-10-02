using UnityEngine;

public abstract class ThrowableWeapon : Weapon
{
    // El tipo 'Throwable' ya implica un solo uso.

    public override void Attack()
    {
        // La lógica de ataque de un arrojadizo es simplemente la lógica de lanzamiento.
        // Se asume que el input de "Attack" en el PlayerWeapon llamará a Throw.
        // Si hay una animación de ataque/lanzamiento, se dispararía aquí.
        // Por ahora, solo ponemos un log para diferenciarlo.
        Debug.Log($"[{weaponData.weaponName}] listo para ser lanzado (Throw).");
    }
}