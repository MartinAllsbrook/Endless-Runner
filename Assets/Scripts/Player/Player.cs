using UnityEngine;
using UnityEngine.InputSystem;

class Player : MonoBehaviour
{
    public static Player Instance;

    [SerializeField] Projectile projectilePrefab;

    float moveInput = 0f;
    ObjectPool<Projectile> projectilePool;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize projectile pool
        projectilePool = new ObjectPool<Projectile>(projectilePrefab, 32);

    }

    void OnEnable()
    {
        InputReader.Move += HandleMove;
        InputReader.Shoot += HandleShoot;
    }

    void OnDisable()
    {
        InputReader.Move -= HandleMove;
        InputReader.Shoot -= HandleShoot;
    }

    void Update()
    {
        // Very simple movement logic
        Vector3 moveDirection = (Vector3.up + (Vector3.right * moveInput)) * Time.deltaTime * 5f;
        transform.Translate(moveDirection);
    }

    /// <summary>
    /// Handles the horizontal movement input
    /// </summary>
    void HandleMove(float value)
    {
        moveInput = value;
    }

    void HandleShoot()
    {
        // Get mouse position in world space (2D)
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Get direction from player to mouse
        Vector3 playerPos = transform.position;
        Vector3 direction = (mouseWorldPos - playerPos).normalized;

        // Calculate rotation towards cursor
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Get projectile from pool
        Projectile proj = projectilePool.Get(playerPos, rotation);
        if (proj != null)
        {
            proj.SetPool(projectilePool);
            proj.Initialize(rotation, 10f, 10f); // Example speed/range, adjust as needed
        }
    }

}