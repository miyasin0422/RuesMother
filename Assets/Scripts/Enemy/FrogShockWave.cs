using UnityEngine;

public class FrogShockWave : EnemyAttack
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float lifeTime = 2f;

    Rigidbody2D rb;

    int damage;
    bool hasHit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Initialize(
        int attackPower,
        Vector2 targetPosition)
    {
        int direction =
            targetPosition.x >= transform.position.x ? 1 : -1;

        Initialize(attackPower, direction);
    }

    public void Initialize(
        int attackPower,
        int direction)
    {
        damage = attackPower;

        rb.linearVelocity =
            new Vector2(direction * moveSpeed, 0);

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
        {
            return;
        }

        PlayerDamage playerDamage =
            collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            hasHit = true;

            playerDamage.Damaged(damage);
        }
    }
}