using UnityEngine;
using UnityEngine.Pool;

public class PoolableObject : MonoBehaviour
{
    private ObjectPool<GameObject> parentPool;

    // Set the pool when the object is retrieved
    public void SetParentPool(ObjectPool<GameObject> pool)
    {
        parentPool = pool;
    }

    public void ReturnToPool()
    {
        if (parentPool != null)
        {
            parentPool.Release(gameObject);
        }
    }
}