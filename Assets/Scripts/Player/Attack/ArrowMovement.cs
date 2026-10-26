using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 startPosition;
    private float maxDistance;
    private bool hasBeenShot;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 direction, float speed, float distance)
    {
        startPosition = rb.position;
        maxDistance = Mathf.Max(0f, distance);
        hasBeenShot = true;

        rb.linearVelocity = direction.normalized * speed;
    }

    void FixedUpdate()
    {
        if (!hasBeenShot)
        {
            return;
        }

        // 発射位置から最大射程まで離れたら消える
        if (Vector2.Distance(startPosition, rb.position) >= maxDistance)
        {
            Destroy(gameObject);
            return;
        }

        if (rb.linearVelocity == Vector2.zero)
        {
            return;
        }

        float angle = Mathf.Atan2(
            rb.linearVelocity.y,
            rb.linearVelocity.x
        ) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 地面・壁への接触だけで消す
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}