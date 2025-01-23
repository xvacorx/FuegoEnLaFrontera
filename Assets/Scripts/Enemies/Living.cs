using UnityEngine;

public abstract class Living : MonoBehaviour
{
    private Collider2D collisions;
    public float health;
    public bool isAlive = true;

    private void Awake()
    {
        if (collisions == null)
        {
            collisions = GetComponent<Collider2D>();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {

            Death();
        }
    }

    protected virtual void Death()
    {
        Debug.Log(gameObject.name + " is dead");
        isAlive = false;
        DisableColliders();
    }

    protected virtual void DisableColliders()
    {
        if (collisions != null)
        {
            collisions.enabled = false;
        }
        else { Destroy(gameObject); }
    }
}
