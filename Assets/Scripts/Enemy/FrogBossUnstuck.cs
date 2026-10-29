using System.Collections.Generic;
using UnityEngine;

// 通常の移動処理の後に、退避用の横速度を設定する。
[DefaultExecutionOrder(200)]
[RequireComponent(typeof(Rigidbody2D))]
public class FrogBossUnstuck : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private FrogBossMovement movement;
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Transform roomCenter;

    [Header("乗り上げ判定")]
    [SerializeField] private float stuckDuration = 0.3f;
    [SerializeField] private float verticalSpeedThreshold = 0.5f;
    [SerializeField] private float footTolerance = 0.15f;

    [Header("退避")]
    [SerializeField] private float escapeSpeed = 5f;

    private Rigidbody2D rb;
    private EnemyHealth health;

    private float stuckTimer;
    private bool escaping;
    private float escapeDirection;

    private readonly List<ContactPoint2D> contacts =
        new List<ContactPoint2D>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();

        if (movement == null)
        {
            movement = GetComponent<FrogBossMovement>();
        }

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        if (movement == null ||
            bodyCollider == null ||
            roomCenter == null)
        {
            Debug.LogError(
                "FrogBossUnstuckのMovement・Body Collider・Room Centerを設定してください。",
                this
            );

            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (health != null && health.IsDead)
        {
            return;
        }

        // 地面へ着地したら退避終了。
        // その後の行動は既存のボスAIに任せる。
        if (movement.IsGrounded())
        {
            if (escaping)
            {
                rb.linearVelocity =
                    new Vector2(0f, rb.linearVelocity.y);
            }

            escaping = false;
            stuckTimer = 0f;
            return;
        }

        if (!escaping)
        {
            bool onPlayer = TryGetPlayerUnderFeet(out float playerX);

            bool verticallyStopped =
                Mathf.Abs(rb.linearVelocity.y) <= verticalSpeedThreshold;

            if (!onPlayer || !verticallyStopped)
            {
                stuckTimer = 0f;
                return;
            }

            stuckTimer += Time.fixedDeltaTime;

            if (stuckTimer < stuckDuration)
            {
                return;
            }

            // 壁へ押し込まないよう、部屋の中央へ退避する。
            float difference = roomCenter.position.x - rb.position.x;

            if (Mathf.Abs(difference) > 0.1f)
            {
                escapeDirection = Mathf.Sign(difference);
            }
            else
            {
                // 部屋の中央にいる場合はルーから離れる側へ。
                escapeDirection = rb.position.x >= playerX ? 1f : -1f;
            }

            escaping = true;
        }

        // 一度退避を始めたら、ルーとの接触が切れても着地まで続ける。
        // 縦速度は変えず、自然に落下させる。
        rb.linearVelocity = new Vector2(
            escapeDirection * escapeSpeed,
            rb.linearVelocity.y
        );
    }

    private bool TryGetPlayerUnderFeet(out float playerX)
    {
        playerX = 0f;

        contacts.Clear();
        rb.GetContacts(contacts);

        Bounds bossBounds = bodyCollider.bounds;

        foreach (ContactPoint2D contact in contacts)
        {
            // 接触している2つのColliderから、相手側を取得する。
            Collider2D other =
                contact.collider.attachedRigidbody == rb
                ? contact.otherCollider
                : contact.collider;

            if (other == null)
            {
                continue;
            }

            PlayerControll player =
                other.GetComponentInParent<PlayerControll>();

            if (player == null)
            {
                continue;
            }

            // 横から触れただけの場合を除外し、
            // カエルの底面付近でルーに触れているか確認する。
            bool contactAtFeet =
                contact.point.y <= bossBounds.min.y + footTolerance;

            bool playerBelow =
                other.bounds.center.y < bossBounds.center.y;

            if (contactAtFeet && playerBelow)
            {
                playerX = other.bounds.center.x;
                return true;
            }
        }

        return false;
    }

    private void OnDisable()
    {
        stuckTimer = 0f;
        escaping = false;
    }
}