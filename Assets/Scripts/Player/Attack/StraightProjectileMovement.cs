using UnityEngine;

public class StraightProjectileMovement : MonoBehaviour
{
    Rigidbody2D rb;

    Vector2 startPosition;
    float maxDistance;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(int direction, float speed, float distance)
    {
        startPosition = transform.position;
        maxDistance = distance;
        // ナイフの向きを進行方向に合わせる
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
        rb.linearVelocity =
            new Vector2(speed * direction, 0f);
    }

    void FixedUpdate()
    {
        float distance =
            Vector2.Distance(startPosition, transform.position);

        if (distance >= maxDistance)
        {
            Destroy(gameObject);
        }
    }
}