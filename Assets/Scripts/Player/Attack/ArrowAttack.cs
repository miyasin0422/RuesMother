using UnityEngine;

public class ArrowAttack : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float ArrowSpeed = 10f;
    [SerializeField] float ArrowUpSpeed = 10f;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //方向補正
        int direction = transform.localScale.x > 0 ? 1 : -1;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;
        rb.linearVelocity = new Vector2(ArrowSpeed * direction, ArrowUpSpeed);
    }
    void FixedUpdate()
    {
        //先端が先頭になるように
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyDamage enemy = other.GetComponentInParent<EnemyDamage>();
        if (enemy != null)
        {
            enemy.Damaged(damage);
        }
        Destroy(gameObject);
    }
}
