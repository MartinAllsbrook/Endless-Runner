using UnityEngine;

class Player : MonoBehaviour
{
    float moveInput = 0f;

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
        Vector3 moveDirection = (Vector3.up + (Vector3.right * moveInput)) * Time.deltaTime * 5f;
        transform.Translate(moveDirection);
    }

    void HandleMove(float value)
    {
        moveInput = value;
    }
}