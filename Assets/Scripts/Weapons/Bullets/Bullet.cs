using System.Collections;
using UnityEngine;

// Asegurar que este script se añade a los GameObjects que forman parte del Pool
// y que tienen el script PooledObject adjunto.
public class Bullet : MonoBehaviour
{
    // Ya no son serializados, el daño y velocidad vendrán del WeaponData a través de Firearm
    public float speed = 20f;
    private float damage = 1f; // Ahora es float para usar projectileDamage del SO

    // La vida útil puede ser una propiedad de la bala o una constante
    public float maxLifetime = 2f;

    private Coroutine disableCoroutine;

    // Método llamado por la clase Firearm para establecer los parámetros antes de volar
    public void InitializeBullet(float damageValue, float speedValue = 20f)
    {
        this.damage = damageValue;
        this.speed = speedValue;
    }

    private void OnEnable()
    {
        // Detener corrutinas previas si el objeto se reutiliza
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        // Iniciar el temporizador de autodestrucción/retorno al pool
        disableCoroutine = StartCoroutine(DisableAfterDelay());
    }

    private void Update()
    {
        // Mover la bala hacia adelante (asumiendo que el 'up' local es la dirección de disparo)
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Simplificar la lógica de impacto
        if (other.CompareTag("Enemy") && other.transform.root.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
            ReturnToPool();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            // TODO: Llamar al PoolManager para instanciar el efecto de impacto (ej: chispas)
            ReturnToPool();
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(maxLifetime);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        // NO LLAMAR SETACTIVE(FALSE) DIRECTAMENTE.
        // En su lugar, llama al método de retorno que maneja el PooledObject.
        // Si el script Bullet está en el mismo GameObject que PooledObject:
        if (gameObject.TryGetComponent(out PooledObject pooledObject))
        {
            pooledObject.ReturnToPool();
        }
        else
        {
            // Fallback si no está en el pool (destrucción clásica o error)
            gameObject.SetActive(false);
        }
    }
}