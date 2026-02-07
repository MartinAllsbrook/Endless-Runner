using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Health))]
public class ScatterObject : MonoBehaviour, IPoolable<ScatterObject>
{
    [SerializeField] ScatterTag scatterTag;
    [SerializeField] float resistance = 1f;

    public float Resistance => resistance;

    ObjectPool<ScatterObject> pool;

    public void SetPool(ObjectPool<ScatterObject> pool)
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
