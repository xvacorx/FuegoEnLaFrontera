using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance => instance;

    private Dictionary<string, Dictionary<string, ObjectPool>> poolCategories = new Dictionary<string, Dictionary<string, ObjectPool>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (Transform categoryTransform in transform)
        {
            string categoryName = categoryTransform.name;

            if (!poolCategories.ContainsKey(categoryName))
            {
                poolCategories[categoryName] = new Dictionary<string, ObjectPool>();
            }

            foreach (Transform poolTransform in categoryTransform)
            {
                ObjectPool pool = poolTransform.GetComponent<ObjectPool>();
                if (pool != null)
                {
                    string poolName = poolTransform.name;
                    poolCategories[categoryName][poolName] = pool;

                    Debug.Log($"Registered Pool: Category = {categoryName}, Pool = {poolName}");
                }
                else
                {
                    Debug.LogWarning($"GameObject {poolTransform.name} under category {categoryName} does not have an ObjectPool component.");
                }
            }
        }
    }

    public GameObject RequestObject(string category, string poolName)
    {
        if (poolCategories.TryGetValue(category, out var pools))
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                return pool.GetObject();
            }
        }
        Debug.LogError($"Pool {poolName} in category {category} not found.");
        return null;
    }

    public void ReturnObject(string category, string poolName, GameObject obj)
    {
        if (poolCategories.TryGetValue(category, out var pools))
        {
            if (pools.TryGetValue(poolName, out var pool))
            {
                pool.ReturnObject(obj);
                return;
            }
        }
        Debug.LogError($"Pool {poolName} in category {category} not found.");
    }
}
