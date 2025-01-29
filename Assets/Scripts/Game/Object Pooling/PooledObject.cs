using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool objectPool;

    public void Initialize(ObjectPool pool)
    {
        objectPool = pool;
    }

    public void ReturnToPool()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no está vinculado a ningún pool.");
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }
}
