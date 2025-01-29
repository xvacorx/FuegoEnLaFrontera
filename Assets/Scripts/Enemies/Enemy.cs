using TMPro;
using UnityEngine;

public class Enemy : Living
{
    public TextMeshProUGUI text;
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        // Aplicar daño al enemigo
        Debug.Log($"{gameObject.name} ha recibido {damage} de daño");
    }
    private void Update()
    {
        text.text = health.ToString();
    }
}