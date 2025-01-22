using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Velocidad de la bala
    public int damage = 1;    // Daño que causa
    public float lifetime = 2f; // Tiempo antes de desactivarse automáticamente

    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        // Reinicia el estado de la bala cada vez que se activa
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisableAfterDelay());
    }

    private void Update()
    {
        // Mover la bala hacia adelante
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detectar colisiones con enemigos, paredes, etc.
        if (other.CompareTag("Enemy"))
        {
            // Aplicar daño al enemigo
            if (other.TryGetComponent(out Enemy enemy))
            {
                {
                    enemy.TakeDamage(damage);
                }
            }

            DeactivateBullet();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            // Desactivar la bala al impactar con un obstáculo
            DeactivateBullet();
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        // Desactivar la bala después del tiempo de vida
        yield return new WaitForSeconds(lifetime);
        DeactivateBullet();
    }

    private void DeactivateBullet()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }
        gameObject.SetActive(false); // Desactiva el objeto para el pooling
    }
}
