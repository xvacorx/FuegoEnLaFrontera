using UnityEngine;

public class Facon : MeleeWeapon
{
    // Propiedad para la fuerza de empuje. Puede ser pública para ajustes finos.
    [SerializeField] private float pushForce = 5f;

    // Aunque la clase padre llama a InitializeWeapon, es bueno usar Start para 
    // inicializar lógica específica del Facón (como el chequeo de recuperación).
    protected override void Start()
    {
        base.Start();

        // TODO: Si el Facón está en el suelo, verifica weaponData.autoRecovery.
        // Si es true, el script de control del jugador debería ser notificado 
        // para recuperarlo automáticamente al pasar cerca.
    }

    protected override void PerformMeleeAttack()
    {
        // 1. Ejecutar el ataque base (detección de enemigos y aplicación de daño/OHK).
        base.PerformMeleeAttack();

        // 2. Lógica específica del Facón (Empuje). 
        // Nota: Por eficiencia, podrías fusionar el OverlapCircleAll del padre aquí,
        // pero por simplicidad de herencia, dejaremos la llamada base.

        // Si tu PerformMeleeAttack del padre no retorna los enemigos golpeados,
        // necesitas volver a hacer la detección aquí o modificar la clase MeleeWeapon.

        // **Opción 1: Re-detectar enemigos para el empuje (menos óptimo)**
        float range = weaponData.attackRange;
        Vector2 attackOrigin = transform.position + transform.right * range * 0.5f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin, range / 2f, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            // Solo empujamos a los que tienen Rigidbody
            if (enemy.TryGetComponent(out Rigidbody2D enemyRb))
            {
                PushEnemy(enemyRb);
            }
        }
    }

    // Recibe el Rigidbody, ya no el Collider2D.
    private void PushEnemy(Rigidbody2D enemyRb)
    {
        // Usa la posición del Rigidbody para calcular la dirección del empuje
        Vector2 pushDirection = (enemyRb.transform.position - transform.position).normalized;
        enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
    }
}