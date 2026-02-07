using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CollidableObject))]
class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [SerializeField] float speed = 2f;
    [SerializeField] float maxDistanceToPlayer = 200f;
    [SerializeField] float attackSpeed = 2f;
    [SerializeField] float damage = 20f;
    [SerializeField] Color attackColor = Color.red;
    Color originalColor;
    SpriteRenderer spriteRenderer;
    protected Health health;
    float attackCooldown;
    bool playerInRange = false;

    ObjectPool<Enemy> pool;

    void Awake()
    {
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
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
        TryAttack();
        Move();
    }

    void TryAttack()
    {
        if (playerInRange)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0f)
            {
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
        StartCoroutine(PlayAttackEffect());
        var playerHealth = Player.Instance.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.DecreaseHealth(damage);
        }
        attackCooldown = attackSpeed;
    }

    IEnumerator PlayAttackEffect()
    {
        spriteRenderer.color = attackColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    void Move()
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            attackCooldown = attackSpeed; // Reset cooldown when player leaves range
        }
    }

    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            attackCooldown = attackSpeed; // Reset cooldown when player leaves range
        }
    }
}