using UnityEngine;

public class BaseballBat : MeleeWeapon
{
    [SerializeField] private float pushForce = 5f; // Fuerza con la que empuja al enemigo

    protected override void Start()
    {
        base.Start();
    }

    // Override del ataque específico para el Bate de Baseball
    protected override void PerformMeleeAttack()
    {
        // Detectar enemigos dentro del rango del bate
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            // Comprobamos si el enemigo tiene el componente Enemy
            if (enemy.TryGetComponent(out Enemy enemyScript))
            {
                enemyScript.TakeDamage(weaponData.attackDamage);
                PushEnemy(enemy); // Empujar al enemigo hacia atrás
            }
        }

        // Opcional: Efectos visuales o sonoros para el bate
        Debug.Log("Bate de Baseball golpeó a los enemigos.");
    }

    // Empujar al enemigo hacia atrás
    private void PushEnemy(Collider2D enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

        if (enemyRb != null)
        {
            // Empujamos al enemigo en la dirección opuesta al bate
            Vector2 pushDirection = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }

    // Visualización del rango de ataque en el editor (opcional)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
