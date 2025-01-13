using UnityEngine;

public class Enemy : MonoBehaviour
{
    public void TakeDamage(float damage)
    {
        Debug.Log("Damage Dealt: " + damage);
    }
}
