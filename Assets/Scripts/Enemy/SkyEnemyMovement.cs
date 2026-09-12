using UnityEngine;

public class SkyEnemyMovement : EnemyMovement
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float patrolDistance = 3f;

    // 巡回高度へ戻る速度
    [SerializeField] float returnHeightSpeed = 2f;

    // HPバーを反転させたくない場合
    [SerializeField] Transform hpBar;

    Rigidbody2D rb;

    Vector2 startPosition;

    int moveDirection = 1;

    Vector3 defaultScale;
    Vector3 defaultHpBarScale;

    void Awake()
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
        // 左右巡回
        if (transform.position.x >= startPosition.x + patrolDistance)
        {
            moveDirection = -1;
        }
        else if (transform.position.x <= startPosition.x - patrolDistance)
        {
            moveDirection = 1;
        }

        // Chaseで上下にずれた場合、元の高さへ戻る
        float heightDifference =
            startPosition.y - transform.position.y;

        float yVelocity = 0f;

        if (Mathf.Abs(heightDifference) > 0.1f)
        {
            yVelocity =
                Mathf.Sign(heightDifference) * returnHeightSpeed;
        }

        rb.linearVelocity =
            new Vector2(
                moveDirection * moveSpeed,
                yVelocity
            );

        UpdateFacing();
    }

    public override void MoveToward(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction =
            ((Vector2)target.position -
             (Vector2)transform.position).normalized;

        rb.linearVelocity =
            direction * moveSpeed;

        // X方向だけ見て左右を決定
        if (direction.x > 0)
        {
            moveDirection = 1;
        }
        else if (direction.x < 0)
        {
            moveDirection = -1;
        }

        UpdateFacing();
    }

    public override void Stop()
    {
        rb.linearVelocity = Vector2.zero;
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
        else if (target.position.x < transform.position.x)
        {
            moveDirection = -1;
        }

        UpdateFacing();
    }

    void UpdateFacing()
    {
        Vector3 scale = defaultScale;

        scale.x =
            Mathf.Abs(defaultScale.x) * moveDirection;

        transform.localScale = scale;

        // HPバーだけ左右反転を打ち消す
        if (hpBar != null)
        {
            Vector3 hpScale = defaultHpBarScale;

            hpScale.x =
                Mathf.Abs(defaultHpBarScale.x) * moveDirection;

            hpBar.localScale = hpScale;
        }
    }
}