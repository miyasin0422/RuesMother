using UnityEngine;

public class GroundEnemyMovement : EnemyMovement
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float patrolDistance = 3f;

    [SerializeField] Transform hpBar;

    Rigidbody2D rb;

    Vector2 startPosition;

    int moveDirection = 1;

    Vector3 defaultScale;
    Vector3 defaultHpBarScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;

        defaultScale = transform.localScale;

        if (hpBar != null)
        {
            defaultHpBarScale = hpBar.localScale;
        }
    }

    public override void Patrol()
    {
        if (transform.position.x >= startPosition.x + patrolDistance)
        {
            moveDirection = -1;
        }
        else if (transform.position.x <= startPosition.x - patrolDistance)
        {
            moveDirection = 1;
        }

        rb.linearVelocity =
            new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        UpdateFacing();
    }

    public override void MoveToward(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (target.position.x > transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }

        rb.linearVelocity =
            new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        UpdateFacing();
    }

    public override void Stop()
    {
        rb.linearVelocity =
            new Vector2(0, rb.linearVelocity.y);
    }

    public override void FaceTarget(Transform target)
    {
        if (target == null)
        {
            return;
        }

        if (target.position.x > transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }

        UpdateFacing();
    }

    void UpdateFacing()
    {
        Vector3 scale = defaultScale;

        scale.x = Mathf.Abs(defaultScale.x) * moveDirection;

        transform.localScale = scale;

        if (hpBar != null)
        {
            Vector3 hpScale = defaultHpBarScale;

            hpScale.x =
                Mathf.Abs(defaultHpBarScale.x) * moveDirection;

            hpBar.localScale = hpScale;
        }
    }
}