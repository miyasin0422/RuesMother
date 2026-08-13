using UnityEngine;

public class MovingGear : MonoBehaviour
{
    [SerializeField] private int attackPower;
    public Transform[] points;
    public float moveSpeed = 3f;
    public float rotateSpeed = 180f;
    Rigidbody2D rb;

    int currentPointIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //移動
        Vector2 targetPosition = points[currentPointIndex].position;
        Vector2 nextPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);
        if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
        }
        //回転
        rb.MoveRotation(rb.rotation + rotateSpeed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerDamage playerDamage = collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            playerDamage.Damaged(attackPower);
        }
    }
}