using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public float lifetime = 2f;

    private Coroutine disableCoroutine;

    private void OnEnable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }
        disableCoroutine = StartCoroutine(DisableAfterDelay());
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(damage);
            DeactivateBullet();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            DeactivateBullet();
        }
    }

    private IEnumerator DisableAfterDelay()
    {
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
        gameObject.SetActive(false);
    }
}
