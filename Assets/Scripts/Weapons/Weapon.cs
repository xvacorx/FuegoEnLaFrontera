using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData weaponData; // Datos del arma (ScriptableObject)

    protected virtual void Start()
    {
        InitializeWeapon();
    }

    protected virtual void InitializeWeapon()
    {
        if (weaponData == null)
        {
            Debug.LogError("WeaponData no asignado en " + gameObject.name);
            return;
        }

        Debug.Log($"{weaponData.name} inicializado.");
    }

    // Método abstracto que todas las armas deben implementar
    public abstract void Attack();
}