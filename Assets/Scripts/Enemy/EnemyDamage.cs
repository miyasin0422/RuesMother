using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //IDとHP
    [SerializeField] private string enemyID;
    [SerializeField] private int enemyhealth;
    [SerializeField] private int enemyMaxHealth = 10;
    //攻撃間隔
    [SerializeField] private float attackInterval;
    //HPバー
    [SerializeField] private Image healthBar;
    //攻撃判定オブジェクト
    public GameObject hitArea1;
    //HP仮の辞書
    EnemyDictionary enemyDic;
    void Start()
    { 
        enemyDic = GameObject.Find("EnemyHealthDictionary").GetComponent<EnemyDictionary>();
        if (enemyDic.enemyHealthDictionary.ContainsKey(enemyID))
        {
            enemyhealth = enemyDic.enemyHealthDictionary[enemyID];
        }
        else
        {
            enemyhealth = enemyMaxHealth;
            enemyDic.enemyHealthDictionary[enemyID] = enemyhealth;
        }
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Damaged(int damage)
    {
        enemyhealth -= damage;
        enemyDic.enemyHealthDictionary[enemyID] = enemyhealth;
        Debug.Log("enemyHP：" + enemyhealth);
        UpdateHealthBar();

        if (enemyhealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)enemyhealth / enemyMaxHealth;
    }
}
