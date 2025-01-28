using UnityEngine;

public class Enemy : Living
{
    public bool hasBeenHitThisAttack = false;  // Flag para verificar si ya fue golpeado en este ciclo de ataque

    public override void TakeDamage(float damage)
    {
        if (!hasBeenHitThisAttack)
        {
            base.TakeDamage(damage);
            // Aplicar daño al enemigo
            Debug.Log($"{gameObject.name} ha recibido {damage} de daño");
            hasBeenHitThisAttack = true;  // Marcar como golpeado en este ciclo de ataque
        }
    }

    // Método para restablecer el estado de golpeado después de un ciclo de ataque
    public void ResetHitStatus()
    {
        Debug.Log("Restableciendo estado de golpeado para " + gameObject.name);
        hasBeenHitThisAttack = false;
    }
}
