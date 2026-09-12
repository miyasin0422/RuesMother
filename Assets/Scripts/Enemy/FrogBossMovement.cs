using UnityEngine;

public class FrogBossMovement : MonoBehaviour
{
    [SerializeField] float hopSpeed = 3f;
    [SerializeField] float hopPower = 6f;

    [SerializeField] float stompSpeed = 5f;
    [SerializeField] float stompJumpPower = 8f;

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 通常の跳ね移動
    public void HopToward(Transform target)
    {
        if (target == null)
        {
            return;
        }

        float direction =
            target.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity =
            new Vector2(
                direction * hopSpeed,
                hopPower
            );
    }

    // 踏みつけ攻撃用ジャンプ
    public void JumpToPlayer(Transform target)
    {
        if (target == null)
        {
            return;
        }

        float direction =
            target.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity =
            new Vector2(
                direction * stompSpeed,
                stompJumpPower
            );
    }

    public void Stop()
    {
        rb.linearVelocity =
            new Vector2(0, rb.linearVelocity.y);
    }

    public bool IsGrounded()
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                groundCheck.position,
                groundCheckRadius
            );

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }
}