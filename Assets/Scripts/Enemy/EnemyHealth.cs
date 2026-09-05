using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    // 敵ID
    [SerializeField] string enemyID;

    // HP
    [SerializeField] int maxHealth = 10;
    int currentHealth;

    // 通常敵用HPバー
    [SerializeField] Image healthBar;

    EnemyDictionary enemyDictionary;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public bool IsDead { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Died;

    void Start()
    {
        enemyDictionary = EnemyDictionary.instance;

        // EnemyDictionaryがない場合は最大HPで開始
        if (enemyDictionary == null)
        {
            currentHealth = maxHealth;
            UpdateHealthBar();
            return;
        }

        // 過去のHPデータが存在する場合
        if (enemyDictionary.enemyHealthDictionary.ContainsKey(enemyID))
        {
            currentHealth =
                enemyDictionary.enemyHealthDictionary[enemyID];
        }
        else
        {
            currentHealth = maxHealth;

            enemyDictionary.enemyHealthDictionary[enemyID] =
                currentHealth;
        }

        // すでに倒されている敵
        if (currentHealth <= 0)
        {
            IsDead = true;
            Destroy(gameObject);
            return;
        }

        UpdateHealthBar();
    }

    public void Damaged(int damage)
    {
        currentHealth -= damage;

        if (enemyDictionary != null)
        {
            enemyDictionary.enemyHealthDictionary[enemyID] =
                currentHealth;
        }

        Debug.Log("Enemy HP：" + currentHealth);

        UpdateHealthBar();

        // ボスHPバーなどへ通知
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }

        healthBar.fillAmount =
            (float)currentHealth / maxHealth;
    }

    void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        Died?.Invoke();

        Destroy(gameObject);
    }
}