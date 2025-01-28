using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Min(0)] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not assigned in " + gameObject.name);
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateObject();
            ReturnObject(obj);
        }
    }

    public GameObject GetObject()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = CreateObject();
        }

        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj.activeSelf)
        {
            obj.SetActive(false);

            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            pool.Enqueue(obj);
        }
    }

    private GameObject CreateObject()
    {
        GameObject obj = Instantiate(prefab);
        RegisterObject(obj);
        return obj;
    }

    private void RegisterObject(GameObject obj)
    {
        if (!obj.TryGetComponent(out PooledObject pooledObject))
        {
            pooledObject = obj.AddComponent<PooledObject>();
        }
        pooledObject.Initialize(this);
    }
}
