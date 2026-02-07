using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(CarMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Inventory))]
class Player : MonoBehaviour
{
    public static Player Instance;

    float steerInput = 0f;
    float throttleInput = 0f;
    bool shootInput = false;

    SwivelGun swivelGun;
    Health health;
    CarMovement carMovement;
    Rigidbody2D rb;
    Collider2D playerCollider;

    bool dead = false;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Get components
        swivelGun = GetComponentInChildren<SwivelGun>();
        health = GetComponent<Health>();
        carMovement = GetComponent<CarMovement>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        health.OnDeath += Die;

        // Inputs
        InputReader.Move += HandleSteer;
        InputReader.Shoot += HandleShoot;
        InputReader.Throttle += HandleThrottle;
    }

    void OnDisable()
    {
        health.OnDeath -= Die;

        // Inputs
        InputReader.Move -= HandleSteer;
        InputReader.Shoot -= HandleShoot;
        InputReader.Throttle -= HandleThrottle;
    }

    void Update()
    {
        // Handle shooting
        if (shootInput)
        {
            swivelGun.TryFire();
        }
    }

    void FixedUpdate()
    {
        // Send input to CarMovement component
        Vector2 movementInput = new Vector2(steerInput, throttleInput);
        carMovement.SetMovementInput(movementInput);
    }

    async void Die()
    {
        health.OnDeath -= Die;
        
        if (dead) return;
        dead = true;

        Debug.Log("Player has died!");
        await GameManager.Instance.EndGame();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Scatter"))
        {
            Health objectHealth = collision.gameObject.GetComponent<Health>();
            float impactStrength = 0f;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float contactImpact = contact.normalImpulse / Time.fixedDeltaTime;
                if (contactImpact > impactStrength)
                {
                    impactStrength = contactImpact;
                }
            }

            Debug.Log($"Collision with {collision.gameObject.name}, impact strength: {impactStrength}");

            if (objectHealth != null)
            {
                objectHealth.DecreaseHealth(impactStrength); // Damage enemy on collision
                health.DecreaseHealth(impactStrength * 0.01f); // Damage player on collision
            }
        }
    }

    async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tunnel"))
        {
            Tunnel tunnel = other.GetComponent<Tunnel>();
            if (tunnel != null)
            {
                await tunnel.EnterTunnel();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Scatter"))
        {
            ScatterObject scatterObject = other.GetComponent<ScatterObject>();
            if (scatterObject != null)
            {
                
            }
            // Push player out of the trigger collider
            Vector2 closestPointOnOther = other.ClosestPoint(transform.position);
            Vector2 closestPointOnPlayer = playerCollider.ClosestPoint(other.transform.position);
            Vector2 pushDirection = (closestPointOnOther - closestPointOnPlayer).normalized;
            float pushForce = 10000; // Adjust for desired push strength
            rb.AddForceAtPosition(pushDirection * pushForce, closestPointOnPlayer, ForceMode2D.Force);

            Health objectHealth = other.GetComponent<Health>();
            if (objectHealth != null)
            {
                

                // Calculate damage based on player's current velocity
                float impactStrength = rb.linearVelocity.magnitude;
                objectHealth.DecreaseHealth(impactStrength * 4 * Time.deltaTime); // Damage object
                // health.DecreaseHealth(impactStrength * 0.1f); // Slight damage to player (reduced from 0.5)
                
                
            }
        }
    }

    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Setting Player Transform");
        transform.position = position;
        transform.rotation = rotation;
    
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    #region Input Handlers
    void HandleSteer(float value)
    {
        steerInput = value;
    }

    void HandleThrottle(float value)
    {
        Debug.Log("Throttle input: " + value);
        throttleInput = value;
    }

    void HandleShoot(bool shoot)
    {
        shootInput = shoot;
    }
    #endregion
}