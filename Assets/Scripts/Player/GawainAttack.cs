using UnityEngine;

public class GawainAttack : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float attackTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, attackTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyDamage enemy = other.GetComponentInParent<EnemyDamage>();
        if (enemy != null)
        {
            enemy.Damaged(damage);
        }
    }
}
