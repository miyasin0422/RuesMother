using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //攻撃力
    [SerializeField]
    private int attackPower;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerDamage playerDamage = collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            playerDamage.Damaged(attackPower);
        }
    }
}
