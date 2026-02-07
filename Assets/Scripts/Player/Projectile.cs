using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour, IPoolable<Projectile>
{
    TrailRenderer trailRenderer;

    float lifetime;
    Vector3 startPosition;
    ObjectPool<Projectile> pool;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    public void SetPool(ObjectPool<Projectile> _pool)
    {
        pool = _pool;
    }

    public void Initialize(Vector2 velocity, float _lifetime)
    {
        rb.linearVelocity = velocity;
        lifetime = _lifetime;

        trailRenderer.Clear();
        startPosition = transform.position;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            ReturnToPool();
            return;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Projectile collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Scatter"))
        {
            var objectHealth = collision.gameObject.GetComponent<Health>();
            if (objectHealth != null)
            {
                objectHealth.DecreaseHealth(25f);
            }

            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        pool.Return(this);
    }
}
