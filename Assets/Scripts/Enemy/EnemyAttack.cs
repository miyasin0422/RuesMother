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
        /*
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamage>().Damage(attackPower);
        }
        */
    }   
}
