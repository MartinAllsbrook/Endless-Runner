using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    float range;
    Vector3 startPosition;
    ObjectPool<Projectile> pool;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(ObjectPool<Projectile> _pool)
    {
        pool = _pool;
    }

    public void Initialize(Vector2 velocity, float _range)
    {
        rb.linearVelocity = velocity;
        range = _range;

        startPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            ReturnToPool();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyHealth = collision.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.DecreaseHealth(25f);
            }

            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        pool.Return(this);
    }
}
