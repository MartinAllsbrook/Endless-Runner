using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class WorldObject : MonoBehaviour, IPoolable<WorldObject>
{
    ObjectPool<WorldObject> pool;

    public void SetPool(ObjectPool<WorldObject> pool)
    {
        this.pool = pool;
    }

    void OnEnable()
    {
        GetComponent<Health>().OnDeath += ReturnToPool;
    }

    void OnDisable()
    {
        GetComponent<Health>().OnDeath -= ReturnToPool;
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
