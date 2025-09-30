using UnityEngine;
using System.Collections;

public abstract class MeleeWeapon : Weapon
{
    // LayerMask ya NO se serializa aquí, debe ser configurado en el Script de Personaje/Controlador
    [Tooltip("LayerMask de los enemigos, debe ser asignado por el controlador que usa el arma.")]
    public LayerMask enemyLayer;

    private bool canAttack = true;

    protected override void InitializeWeapon()
    {
        base.InitializeWeapon();
        // No es necesario inicializar attackRange aquí, ya que se usará el valor del SO en PerformMeleeAttack
    }

    public override void Attack()
    {
        // Usar attackSpeed del SO para el cooldown
        if (canAttack)
        {
            PerformMeleeAttack();
            // TODO: Llamar al PoolManager para el efecto de impacto/swing (si aplica)
            StartCoroutine(AttackCooldown());
        }
    }

    protected virtual void PerformMeleeAttack()
    {
        // Usar attackRange del SO
        float range = weaponData.attackRange;

        // Determinar el centro del ataque (asumiendo que está en la punta del arma)
        Vector2 attackOrigin = transform.position + transform.right * range * 0.5f;

        // Usar OverlapCircleAll para encontrar enemigos en el área de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin, range / 2f, enemyLayer);

        int enemiesHit = 0;
        foreach (var enemyCollider in hitEnemies)
        {
            if (enemyCollider.transform.root.TryGetComponent(out Enemy enemyScript))
            {
                enemyScript.TakeDamage(weaponData.attackDamage);
                enemiesHit++;

                // Lógica de Penetración
                if (!weaponData.hasPenetration && enemiesHit >= 1)
                {
                    // Si no tiene penetración (ej: Facón), detenemos el bucle después del primer golpe
                    break;
                }
            }
        }

        // TODO: Mover esta parte al script de personaje que usa el arma
        // Debug.Log("Ataque cuerpo a cuerpo realizado."); 
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        // Usar attackSpeed del SO
        float cooldownTime = 1f / weaponData.attackSpeed;
        yield return new WaitForSeconds(cooldownTime);
        canAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Usar attackRange del SO para el Gizmo
        Gizmos.color = Color.red;
        float range = weaponData != null ? weaponData.attackRange : 1f;
        Vector2 attackOrigin = transform.position + transform.right * range * 0.5f;
        Gizmos.DrawWireSphere(attackOrigin, range / 2f);
    }
}