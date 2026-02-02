using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
class CarMovement : MonoBehaviour
{
    [Header("Car Movement")]
    [SerializeField] float acceleration = 10f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] float reverseMaxSpeed = 8f;
    [SerializeField] float turnSpeed = 150f;
    [SerializeField] float brakeForce = 20f;
    [SerializeField] float drag = 2f;

    // Input vector: x = steering (-1 to 1), y = throttle/reverse (-1 to 1)
    Vector2 movementInput = Vector2.zero;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    public void SetMovementInput(Vector2 input)
    {
        movementInput = input;
    }

    void ApplyMovement()
    {
        // Get current velocity
        Vector2 velocity = rb.linearVelocity;
        float currentSpeed = velocity.magnitude;

        // Calculate forward direction
        Vector2 forward = transform.up;

        // Determine if we're moving forward or backward
        float forwardDot = Vector2.Dot(velocity.normalized, forward);
        bool movingForward = forwardDot > 0.1f;
        bool movingBackward = forwardDot < -0.1f;

        float steerInput = movementInput.x;
        float throttleInput = movementInput.y;

        // Apply throttle/reverse
        if (Mathf.Abs(throttleInput) > 0.1f)
        {
            if (throttleInput > 0) // Forward throttle
            {
                if (currentSpeed < maxSpeed)
                {
                    rb.AddForce(forward * acceleration * throttleInput, ForceMode2D.Force);
                }
            }
            else // Reverse
            {
                float reverseInput = -throttleInput;
                if (currentSpeed < reverseMaxSpeed || movingBackward)
                {
                    rb.AddForce(-forward * acceleration * reverseInput * 0.7f, ForceMode2D.Force);
                }
                else if (movingForward)
                {
                    // Apply brake force when trying to reverse while moving forward
                    rb.AddForce(-velocity.normalized * brakeForce * reverseInput, ForceMode2D.Force);
                }
            }
        }

        // Apply steering (only when moving)
        if (Mathf.Abs(steerInput) > 0.1f && currentSpeed > 0.5f)
        {
            float turnAmount = steerInput * turnSpeed * currentSpeed * Time.fixedDeltaTime;
            // Reduce turn speed at higher velocities for more realistic handling
            float speedFactor = Mathf.Clamp01(currentSpeed / maxSpeed);
            
            // Reverse steering direction when moving backward
            float steeringDirection = movingBackward ? -1f : 1f;
            rb.angularVelocity = -turnAmount * speedFactor * 10f * steeringDirection;
        }
        else
        {
            rb.angularVelocity = 0f;
        }

        // Apply drag
        rb.linearVelocity = Vector2.Lerp(velocity, Vector2.zero, drag * Time.fixedDeltaTime);
    }
}