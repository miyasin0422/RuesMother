using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static int MaxplayerHealth = 100;
    public static int playerHealth = 100;
    public static int refreshItemStock = 2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Damaged(int damage)
    {
        playerHealth -= damage;

        if (playerHealth < 0)
        {
            playerHealth = 0;
        }

        if (playerHealth == 0)
        {
            // 死亡処理は後で追加
        }
    }
}
