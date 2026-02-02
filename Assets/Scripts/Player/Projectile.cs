using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed;
    float range;
    Vector3 direction;
    Vector3 startPosition;
    ObjectPool<Projectile> pool;

    public void SetPool(ObjectPool<Projectile> _pool)
    {
        pool = _pool;
    }

    public void Initialize(Quaternion rotation, float _speed, float _range)
    {
        speed = _speed;
        range = _range;
        
        direction = rotation * Vector3.up; // For 2D, use Vector3.up as the forward direction in XY plane
        direction.z = 0f; // Ensure no Z movement

        startPosition = transform.position;
        transform.rotation = rotation;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.position += direction * moveDistance;

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
