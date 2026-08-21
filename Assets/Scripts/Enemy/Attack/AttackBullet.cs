using UnityEngine;

public class AttackBullet : EnemyAttack
{
    [SerializeField] float bulletSpeed = 5f;
    private Rigidbody2D rb;
    private int damage;
    private Vector2 direction;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public override void Initialize(int attackPower, Vector2 targetPosition)
    {
        damage = attackPower;

        direction =
            (targetPosition - (Vector2)transform.position).normalized;
    }
    void FixedUpdate()
    {
        rb.linearVelocity = direction * bulletSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerDamage playerDamage = collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            playerDamage.Damaged(damage);
        }
        Destroy(gameObject);
    }
}
