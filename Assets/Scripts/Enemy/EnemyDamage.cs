using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //IDとHP
    [SerializeField]
    private string enemyID;
    [SerializeField]
    private int enemyhealth;
    [SerializeField]
    private int enemyMaxHealth = 10;
    //攻撃間隔
    [SerializeField]
    private float attackInterval;
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
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyhealth <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void Damaged(int damage)
    {
        enemyhealth -= damage;
        enemyDic.enemyHealthDictionary[enemyID] = enemyhealth;
    }
    IEnumerator Attack()
    {
        while (true)
        {
            hitArea1.SetActive(true);
            yield return new WaitForSeconds(attackInterval);
            hitArea1.SetActive(false);
            yield return new WaitForSeconds(attackInterval);
        }
    }
}
