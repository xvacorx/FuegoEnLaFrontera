using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Velocidad de la bala
    public int damage = 1;    // Daño que causa
    public float lifetime = 2f; // Tiempo antes de destruirse automáticamente

    private void Start()
    {
        // Destruir la bala después de un tiempo
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Mover la bala hacia adelante
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar colisiones con enemigos, paredes, etc.
        if (other.CompareTag("Enemy"))
        {
            // Aplicar daño al enemigo (puedes usar un script en el enemigo)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Destruir la bala al impactar con algo
        Destroy(gameObject);
    }
}
