using UnityEngine;
using System.Collections.Generic;

public class EnemyDictionary : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //シーンをまたげるようにする
    public static EnemyDictionary instance;
    //敵のIDとHPを管理する
    public Dictionary<string, int> enemyHealthDictionary = new Dictionary<string, int>();
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
