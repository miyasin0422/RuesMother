using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerEnemyContact : MonoBehaviour
{
    [SerializeField] PlayerControll playerController;
    [SerializeField] Collider2D standCollider;
    [SerializeField] Collider2D crouchCollider;

    [Header("接触ダメージ")]
    [SerializeField] int contactDamage = 5;
    [SerializeField] float damageInterval = 0.8f;

    [Header("横へのノックバック")]
    [SerializeField] float knockbackSpeed = 10f;
    [SerializeField] float knockbackDuration = 0.4f;

    Rigidbody2D rb;
    PlayerDamage playerDamage;
    Rigidbody2D contactedEnemy;

    float knockbackEndTime;
    float nextDamageTime;
    float pushDirection;
    float pushSpeed;

    public bool IsKnockedBack =>
        isActiveAndEnabled &&
        Time.time < knockbackEndTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDamage = GetComponent<PlayerDamage>();

        if (playerController == null)
        {
            playerController = GetComponent<PlayerControll>();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContact(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        HandleContact(collision);
    }

    void HandleContact(Collision2D collision)
    {
        if (
            playerController == null ||
            playerDamage == null ||
            playerController.IsDashing ||
            IsKnockedBack ||
            PlayerStatus.playerHealth <= 0)
        {
            return;
        }

        Collider2D enemy = collision.collider;

        EnemyHealth enemyHealth =
            enemy.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null || enemyHealth.IsDead)
        {
            return;
        }

        Collider2D body =
            standCollider != null && standCollider.isActiveAndEnabled
            ? standCollider
            : crouchCollider;

        if (body == null || !body.isActiveAndEnabled)
        {
            return;
        }

        Bounds playerBounds = body.bounds;
        Bounds enemyBounds = enemy.bounds;

        // 敵の中心より左なら左。それ以外は右。
        pushDirection =
            playerBounds.center.x < enemyBounds.center.x
            ? -1f
            : 1f;

        float duration = Mathf.Max(knockbackDuration, 0.05f);

        // 大きな敵でも、横の端まで届く速度を確保する
        float distanceToOutside = pushDirection < 0f
            ? playerBounds.max.x - enemyBounds.min.x
            : enemyBounds.max.x - playerBounds.min.x;

        pushSpeed = Mathf.Max(
            knockbackSpeed,
            (Mathf.Max(distanceToOutside, 0f) + 0.1f) / duration
        );

        contactedEnemy = enemy.attachedRigidbody;
        knockbackEndTime = Time.time + duration;

        ApplyPush();

        if (Time.time >= nextDamageTime)
        {
            nextDamageTime =
                Time.time + Mathf.Max(damageInterval, 0.05f);

            playerDamage.Damaged(contactDamage, true);
        }
    }

    void FixedUpdate()
    {
        if (playerController == null || playerController.IsDashing)
        {
            knockbackEndTime = 0f;
            return;
        }

        if (IsKnockedBack)
        {
            ApplyPush();
        }
    }

    void ApplyPush()
    {
        float enemySpeedX = contactedEnemy != null
            ? contactedEnemy.linearVelocity.x
            : 0f;

        rb.linearVelocity = new Vector2(
            enemySpeedX + pushDirection * pushSpeed,
            Mathf.Min(rb.linearVelocity.y, 0f)
        );
    }

    void OnDisable()
    {
        knockbackEndTime = 0f;
        nextDamageTime = 0f;
        contactedEnemy = null;
    }
}