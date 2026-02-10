using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CollidableObject))]
public class ScatterObject : MonoBehaviour, IPoolable<ScatterObject>
{
    [SerializeField] ScatterTag scatterTag;
    float movementResistance = 10000f;

    ObjectPool<ScatterObject> pool;
    Vector2 initialPosition;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(ObjectPool<ScatterObject> pool)
    {
        this.pool = pool;
    }

    public void SetPosition(Vector2 position)
    {
        transform.position = position;
        initialPosition = position;
    }

    void OnEnable()
    {
        GetComponent<Health>().OnDeath += ReturnToPool;
    }

    void OnDisable()
    {
        GetComponent<Health>().OnDeath -= ReturnToPool;
    }

    void FixedUpdate()
    {
        if (initialPosition == null)
            return;

        float offset = Vector2.Distance(initialPosition, transform.position);

        if (offset > 0.1f)
        {
            Vector2 directionToInitial = (initialPosition - (Vector2)transform.position).normalized;
            Vector2 resistanceForce = directionToInitial * (offset * movementResistance);
            rb.AddForce(resistanceForce * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = initialPosition;
        }
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Road") || collision.CompareTag("POI Area"))
        {
            ReturnToPool();
        }
    }
}
