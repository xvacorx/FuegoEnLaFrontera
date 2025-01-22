using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData weaponData; // Datos del arma (ScriptableObject)
    private Rigidbody2D rb;
    private Collider2D col;

    protected virtual void Start()
    {
        InitializeWeapon();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (rb == null) Debug.LogError("Rigidbody2D no encontrado en " + gameObject.name);
        if (col == null) Debug.LogError("Collider2D no encontrado en " + gameObject.name);
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

    // Lógica para lanzar el arma
    public virtual void Throw(Vector2 direction, float force)
    {
        if (rb == null || col == null) return;

        transform.SetParent(null); // Desparentar el arma
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        col.enabled = true; // Habilitar colisiones
        col.isTrigger = false;

        gameObject.layer = 6; // Cambiar a Layer Mask 6
        StartCoroutine(EnableTriggerWhenStopped());
    }

    private System.Collections.IEnumerator EnableTriggerWhenStopped()
    {
        yield return new WaitForSeconds(0.5f); // Esperar un tiempo para estabilizar el movimiento

        // Esperar hasta que el arma esté quieta
        while (rb.linearVelocity.sqrMagnitude > 0.2f)
        {
            yield return null;
        }

        col.isTrigger = true; // Activar como trigger
        gameObject.layer = 0; // Cambiar de nuevo a Layer Mask 0
    }
}
