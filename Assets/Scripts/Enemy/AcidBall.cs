using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AcidBall : EnemyAttack
{
    [SerializeField] GameObject acidPoolPrefab;

    [SerializeField] float flightTime = 0.8f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float poolHeightOffset = 0.02f;

    Rigidbody2D rb;

    int damage;
    bool hasHit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public override void Initialize(
        int attackPower,
        Vector2 targetPosition)
    {
        damage = attackPower;

        float time = Mathf.Max(flightTime, 0.05f);

        Vector2 gravity = Physics2D.gravity * rb.gravityScale;
        Vector2 distance = targetPosition - rb.position;

        rb.linearVelocity =
            distance / time - gravity * (time * 0.5f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        PlayerDamage player =
            other.GetComponentInParent<PlayerDamage>();

        if (player != null)
        {
            hasHit = true;

            player.Damaged(damage);

            Destroy(gameObject);
            return;
        }

        // 敵や他の攻撃判定などのTriggerは無視する
        if (other.isTrigger)
        {
            return;
        }

        hasHit = true;

        if (other.CompareTag("Ground") && acidPoolPrefab != null)
        {
            // 水平な床の上面に生成する
            Vector2 poolPosition = new Vector2(
                Mathf.Clamp(
                    transform.position.x,
                    other.bounds.min.x,
                    other.bounds.max.x
                ),
                other.bounds.max.y + poolHeightOffset
            );

            Instantiate(
                acidPoolPrefab,
                poolPosition,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}