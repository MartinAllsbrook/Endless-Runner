using UnityEngine;

class Enemy : MonoBehaviour
{
    public float speed = 2f;

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
}