using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    // 敵ID
    [SerializeField] string enemyID;

    // HP
    [SerializeField] int maxHealth = 10;
    int currentHealth;

    // 通常敵用HPバー
    [SerializeField] Image healthBar;
    // ドロップアイテム
    [SerializeField] GameObject[] inventoryItems;
    [SerializeField] private float jumpForce = 5f;  // 上に打ち上げる力
    [SerializeField] private float sideForce = 2f;  // 左右へのランダムな散らばり
    // 被ダメージ時の色変化
    [SerializeField] SpriteRenderer damageSprite;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] float damageFlashTime = 0.15f;
    EnemyDictionary enemyDictionary;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    public bool IsDead { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Died;
    Color originalColor;
    float flashEndTime;
    bool isFlashing;

    // 酸状態
    [SerializeField] Color acidColor = Color.yellow;

    bool isAcid;
    float acidEndTime;
    float nextAcidDamageTime;
    float acidDamageInterval;
    int acidDamage;

    public bool IsAcid => isAcid;
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
    void Awake()
    {
        if (damageSprite == null)
        {
            damageSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (damageSprite != null)
        {
            originalColor = damageSprite.color;
        }
    }

    void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (isAcid)
        {
            while (
                !IsDead &&
                nextAcidDamageTime <= Time.time &&
                nextAcidDamageTime <= acidEndTime)
            {
                nextAcidDamageTime += acidDamageInterval;

                // 酸ダメージでは赤くしない
                Damaged(acidDamage, false);
            }

            if (IsDead)
            {
                return;
            }

            if (Time.time >= acidEndTime)
            {
                isAcid = false;
            }
        }

        if (isFlashing && Time.time >= flashEndTime)
        {
            isFlashing = false;
        }

        UpdateSpriteColor();
    }

    public void ApplyAcid(
    int tickDamage,
    float duration,
    float interval)
    {
        if (IsDead || tickDamage <= 0 || duration <= 0f)
        {
            return;
        }

        float safeInterval = Mathf.Max(interval, 0.05f);

        // 再付与で次のダメージ時刻をリセットしない
        if (!isAcid)
        {
            nextAcidDamageTime = Time.time + safeInterval;
        }

        acidDamage = tickDamage;
        acidDamageInterval = safeInterval;
        acidEndTime = Time.time + duration;
        isAcid = true;

        UpdateSpriteColor();
    }

    void FlashDamage()
    {
        flashEndTime = Time.time + Mathf.Max(damageFlashTime, 0f);
        isFlashing = true;

        UpdateSpriteColor();
    }


    void UpdateSpriteColor()
    {
        if (damageSprite == null)
        {
            return;
        }

        Color color = originalColor;

        if (isFlashing)
        {
            color = damageColor;
        }
        else if (isAcid)
        {
            color = acidColor;
        }

        color.a = originalColor.a;
        damageSprite.color = color;
    }

    void OnDisable()
    {
        isFlashing = false;
        isAcid = false;

        UpdateSpriteColor();
    }

    public void Damaged(int damage, bool showFlash = true)
    {
        if (IsDead)
        {
            return;
        }

        if (damage > 0 && showFlash)
        {
            FlashDamage();
        }
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
        DropItem();
    }
    void DropItem()
    {
        foreach (GameObject inventoryItem in inventoryItems)
        {
            GameObject dropItem = Instantiate(inventoryItem, transform.position, Quaternion.identity);
            Rigidbody2D rb = dropItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(UnityEngine.Random.Range(-sideForce, sideForce), jumpForce);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }
}