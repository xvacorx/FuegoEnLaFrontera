using UnityEngine;
using System.Collections;

public abstract class MeleeWeapon : Weapon
{
    [SerializeField] protected float attackRange = 1f; // Rango del ataque
    [SerializeField] protected LayerMask enemyLayer;  // Capa para detectar enemigos

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        PerformMeleeAttack();
    }

    // Realiza el ataque cuerpo a cuerpo
    // Realiza el ataque cuerpo a cuerpo
    protected virtual void PerformMeleeAttack()
    {
        // Detectar enemigos dentro del rango de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            // Comprobamos si el enemigo tiene el componente Enemy
            if (enemy.TryGetComponent(out Enemy enemyScript))
            {
                // Aplicamos daño solo si no ha sido golpeado en este ciclo de ataque
                if (!enemyScript.hasBeenHitThisAttack)
                {
                    enemyScript.TakeDamage(weaponData.attackDamage);
                    Debug.Log($"{enemyScript.gameObject.name} ha recibido {weaponData.attackDamage} de daño");
                }
            }
        }

        // Depuración para comprobar si se está entrando en la lógica del ciclo de ataque
        Debug.Log($"attackSpeed: {weaponData.attackSpeed}");

        // Restablecer el estado de "golpeado" después de completar el ataque


        StartCoroutine(EndAttackCycle(hitEnemies, 0.1f));


        Debug.Log("Ataque cuerpo a cuerpo realizado.");
    }

    // Corrutina que restablece el estado de golpeado después de un cierto tiempo
    private IEnumerator EndAttackCycle(Collider2D[] hitEnemies, float waitTime)
    {
        Debug.Log($"Esperando {waitTime} segundos antes de restablecer estado de golpeado.");
        yield return new WaitForSeconds(waitTime);  // Esperar el tiempo definido

        // Restablecer el estado de "golpeado" de todos los enemigos
        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyScript))
            {
                enemyScript.ResetHitStatus();
                Debug.Log($"Restablecido estado de golpeado para: {enemyScript.gameObject.name}");
            }
        }
    }

    // Visualización del rango de ataque en el editor (opcional)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
