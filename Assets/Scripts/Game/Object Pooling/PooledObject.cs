using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool objectPool;

    public void Initialize(ObjectPool pool)
    {
        objectPool = pool;
    }

    private void OnDisable()
    {
        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
