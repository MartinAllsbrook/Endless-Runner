using UnityEngine;

public class WorldObject : MonoBehaviour, IPoolable<WorldObject>
{
    ObjectPool<WorldObject> pool;

    public void SetPool(ObjectPool<WorldObject> pool)
    {
        this.pool = pool;
    }

    public void ReturnToPool()
    {
        if (pool != null)
        {
            pool.Return(this);
        }
        else
        {
            // Fallback: just deactivate if no pool is set
            gameObject.SetActive(false);
        }
    }
}
