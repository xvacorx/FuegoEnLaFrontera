using UnityEngine;
using System.Collections; // Necesario para IEnumerator

public abstract class Weapon : MonoBehaviour
{
    // Campo público para asignar el Scriptable Object en el Inspector
    public WeaponData weaponData;

    // Componentes que toda arma tendrá al ser lanzada o manipulada
    private Rigidbody2D rb;
    private Collider2D col;

    // Constante para el LayerMask de objetos en el suelo (ej: Layer 6)
    private const int THROWABLE_LAYER = 6;
    // Constante para el LayerMask de un arma equipada o en el suelo (ej: Layer 0)
    private const int DEFAULT_LAYER = 0;

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
    }

    public abstract void Attack();

    // Lógica de Lanzamiento Universal
    public virtual void Throw(Vector2 direction, float forceMultiplier = 1f)
    {
        if (rb == null || col == null) return;

        // Desvincular de cualquier padre (jugador)
        transform.SetParent(null);

        // Configurar componentes para la física del lanzamiento
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;

        // Usar la fuerza definida en el SO multiplicada por el factor de control
        float actualForce = weaponData.throwForce * forceMultiplier;
        rb.AddForce(direction.normalized * actualForce, ForceMode2D.Impulse);

        // Configurar colisionador para detectar impacto
        col.enabled = true;
        col.isTrigger = false;

        // Asignar layer para colisión mientras está en el aire (Layer 6: Ignora jugador/enemigos, solo golpea)
        gameObject.layer = THROWABLE_LAYER;

        // Iniciar la corrutina para detectar el fin del lanzamiento
        StartCoroutine(EnableTriggerWhenStopped());
    }

    // TODO: Implementar lógica de impacto de lanzamiento aquí (OnCollisionEnter2D)
    // El impacto debe hacer daño 'weaponData.thrownImpactDamage' y aplicar un stun.

    private IEnumerator EnableTriggerWhenStopped()
    {
        // Esperar un breve momento para que la fuerza se aplique
        yield return new WaitForSeconds(0.1f);

        // Esperar hasta que el objeto esté casi parado (para simular que cae al suelo)
        while (rb.linearVelocity.sqrMagnitude > 0.5f) // Umbral más bajo para más precisión
        {
            yield return null;
        }

        // Una vez en el suelo, se convierte en un trigger para ser recogido
        col.isTrigger = true;

        // Volver al layer por defecto (para evitar que se comporte como un obstáculo para siempre)
        gameObject.layer = DEFAULT_LAYER;

        // Desactivar la simulación de Rigidbody para que no se mueva más
        rb.simulated = false;
    }
}