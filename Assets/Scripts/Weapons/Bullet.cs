using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // Velocidad de la bala
    public int damage = 1;    // Da�o que causa
    public float lifetime = 2f; // Tiempo antes de destruirse autom�ticamente

    private void Start()
    {

    }

    private void Update()
    {
        // Mover la bala hacia adelante
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detectar colisiones con enemigos, paredes, etc.
        if (other.CompareTag("Enemy"))
        {
            // Aplicar da�o al enemigo (puedes usar un script en el enemigo)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
