using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize;
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            RegisterObject(obj); // Registramos el objeto en la pool
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        GameObject newObj = Instantiate(prefab, transform);
        newObj.SetActive(true);
        RegisterObject(newObj);
        return newObj;
    }

    private void RegisterObject(GameObject obj)
    {
        obj.AddComponent<PoolObject>().Initialize(this);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
