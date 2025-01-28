using UnityEngine;
using System.Collections;

public abstract class MeleeWeapon : Weapon
{
    [SerializeField] protected float attackRange = 1f; // Rango del ataque
    [SerializeField] protected LayerMask enemyLayer;  // Capa para detectar enemigos

    private bool canAttack = true;

    protected override void Start()
    {
        base.Start();
    }

    public override void Attack()
    {
        if (canAttack)
        {
            PerformMeleeAttack();
            StartCoroutine(AttackCooldown());
        }
    }

    protected virtual void PerformMeleeAttack()
    {
        // Detectar enemigos dentro del rango de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var enemyCollider in hitEnemies)
        {
            // Intentar obtener el componente Enemy
            if (enemyCollider.transform.root.TryGetComponent(out Enemy enemyScript))
            {
                // Aplicar daño al enemigo
                enemyScript.TakeDamage(weaponData.attackDamage);
                Debug.Log($"{enemyScript.gameObject.name} ha recibido {weaponData.attackDamage} de daño");
            }
        }

        Debug.Log("Ataque cuerpo a cuerpo realizado.");
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1f / weaponData.attackSpeed);
        canAttack = true;
    }

    // Visualización del rango de ataque en el editor (opcional)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
