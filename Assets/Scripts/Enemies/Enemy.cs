using TMPro;
using UnityEngine;

public class Enemy : Living
{
    public TextMeshProUGUI text;
    public bool isStunned = false;
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        Debug.Log($"{gameObject.name} ha recibido {damage} de daño");
    }
    private void Update()
    {
        text.text = health.ToString();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {

    }
}