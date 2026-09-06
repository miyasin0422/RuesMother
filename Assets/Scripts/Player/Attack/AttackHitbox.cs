using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] int damage = 10;

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.Damaged(damage);
        }
    }
}