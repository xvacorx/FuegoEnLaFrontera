using UnityEngine;

public class Enemy : Living
{
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        // Aplicar daño al enemigo
        Debug.Log($"{gameObject.name} ha recibido {damage} de daño");
    }
}