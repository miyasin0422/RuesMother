using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Damaged(int damage)
    {
        PlayerStatus.playerHealth -= damage;
        Debug.Log("playerHP：" + PlayerStatus.playerHealth);
        if (PlayerStatus.playerHealth < 0)
        {
            PlayerStatus.playerHealth = 0;
            Debug.Log("ゲームオーバー");
        }
    }
}