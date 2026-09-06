using UnityEngine;

public class AttackBullet : EnemyAttack
{
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] float lifeTime = 5f;

    Rigidbody2D rb;

    int damage;

    Vector2 direction;

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

        direction =
            (targetPosition - (Vector2)transform.position).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity =
            direction * bulletSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerDamage playerDamage =
            collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            playerDamage.Damaged(damage);
        }

        Destroy(gameObject);
    }
}