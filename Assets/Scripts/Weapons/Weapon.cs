using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData weaponData;
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
        //Debug.Log($"{weaponData.name} inicializado.");
    }

    public abstract void Attack();

    public virtual void Throw(Vector2 direction, float force)
    {
        if (rb == null || col == null) return;

        transform.SetParent(null);
        rb.simulated = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        col.enabled = true;
        col.isTrigger = false;

        gameObject.layer = 6;
        StartCoroutine(EnableTriggerWhenStopped());
    }

    private System.Collections.IEnumerator EnableTriggerWhenStopped()
    {
        yield return new WaitForSeconds(0.5f);

        while (rb.linearVelocity.sqrMagnitude > 1.5f)
        {
            yield return null;
        }

        col.isTrigger = true;
        gameObject.layer = 0;
    }
}
