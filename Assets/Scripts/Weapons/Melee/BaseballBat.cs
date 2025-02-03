using UnityEngine;

public class BaseballBat : MeleeWeapon
{
    [SerializeField] private float pushForce = 5f;
    protected override void Start()
    {
        base.Start();
    }

    protected override void PerformMeleeAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (var enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out Enemy enemyScript))
            {
                enemyScript.TakeDamage(weaponData.attackDamage);
                PushEnemy(enemy);
            }
        }

        Debug.Log("Bate de Baseball golpeó a los enemigos.");
    }

    private void PushEnemy(Collider2D enemy)
    {
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

        if (enemyRb != null)
        {
            Vector2 pushDirection = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }
}
