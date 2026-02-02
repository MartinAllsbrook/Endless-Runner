using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    Health health;


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
        Vector2 moveDirection = (targetPosition - currentPosition).normalized;
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    void Die()
    {
        Destroy(gameObject);
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