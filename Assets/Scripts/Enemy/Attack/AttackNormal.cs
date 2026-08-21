using UnityEngine;

public class AttackNormal : EnemyAttack
{
    [SerializeField] float attackTime = 0.5f;
    private int damage;
    void Start()
    {
        Destroy(gameObject, attackTime);
    }
    public override void Initialize(int attackPower, Vector2 targetPosition)
    {
        damage = attackPower;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerDamage playerDamage = collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            playerDamage.Damaged(damage);
        }
    }
}
