using UnityEngine;

public class PooledObject : MonoBehaviour
{
    private ObjectPool objectPool;
    private bool isReturningToPool = false; // Flag para asegurarnos de que solo se llame a ReturnToPool una vez por ciclo de desactivaci�n

    public void Initialize(ObjectPool pool)
    {
        objectPool = pool;
    }

    public void ReturnToPool()
    {
        if (isReturningToPool) return; // Si ya est� retornado, no hace nada

        isReturningToPool = true; // Marcar que el objeto est� siendo retornado al pool

        if (objectPool != null)
        {
            objectPool.ReturnObject(gameObject);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no est� vinculado a ning�n pool.");
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        // Solo devuelve a la pool si no ha sido ya retornado
        // Agregar un peque�o retraso para asegurarse de que todo el proceso de desactivaci�n termine correctamente
        if (!isReturningToPool)
        {
            Invoke(nameof(ReturnToPoolDelayed), 0.1f); // Llamada retrasada para evitar conflictos
        }
    }

    private void ReturnToPoolDelayed()
    {
        if (!isReturningToPool)
        {
            ReturnToPool();
        }
    }

    // Resetear la bandera cuando el objeto es activado de nuevo
    private void OnEnable()
    {
        isReturningToPool = false; // Permitir que se retorne al pool nuevamente cuando se vuelva a activar
    }
}
