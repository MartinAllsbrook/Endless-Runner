using UnityEngine;

class Player : MonoBehaviour
{
    public static Player Instance;
    float moveInput = 0f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        InputReader.Move += HandleMove;
    }

    void OnDisable()
    {
        InputReader.Move -= HandleMove;
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
}