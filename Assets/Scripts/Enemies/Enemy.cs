using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [SerializeField] float speed = 2f;
    [SerializeField] float maxDistanceToPlayer = 200f;

    Health health;

    ObjectPool<Enemy> pool;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        health.OnDeath += Die;   
    }

    void OnDisable()
    {
        health.OnDeath -= Die;
    }

    void Update()
    {
        Vector2 targetPosition = Player.Instance.transform.position;
        Vector2 currentPosition = transform.position;
        
        // Despawn if too far from player
        if (Vector2.Distance(currentPosition, targetPosition) > maxDistanceToPlayer)
        {
            Despawn();
            return;
        }
        
        // Move towards player
        Vector2 moveDirection = (targetPosition - currentPosition).normalized;
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

    }

    public void SetPool(ObjectPool<Enemy> _pool)
    {
        pool = _pool;
    }

    void Die()
    {
        Player.Instance.GetComponent<Inventory>().AddScrap(3);
        Despawn();
    }

    void Despawn()
    {
        pool.Return(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.DecreaseHealth(20f);
            }
        }
    }
}