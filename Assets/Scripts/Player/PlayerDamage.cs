using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] SpriteRenderer damageSprite;
    [SerializeField] Color damageColor = Color.red;
    [SerializeField] float damageFlashTime = 0.15f;
    Color originalColor;
    float flashEndTime;
    bool isFlashing;

    PlayerControll playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
    void Awake()
    {
        playerController = GetComponent<PlayerControll>();
        if (damageSprite == null)
        {
            damageSprite = GetComponentInChildren<SpriteRenderer>();
        }

        if (damageSprite != null)
        {
            originalColor = damageSprite.color;
        }
    }

    void FlashDamage()
    {
        if (damageSprite == null)
        {
            return;
        }

        Color flashColor = damageColor;
        flashColor.a = originalColor.a;

        damageSprite.color = flashColor;

        flashEndTime = Time.time + Mathf.Max(damageFlashTime, 0f);
        isFlashing = true;
    }

    void RestoreSpriteColor()
    {
        if (damageSprite != null)
        {
            damageSprite.color = originalColor;
        }

        isFlashing = false;
    }

    void OnDisable()
    {
        RestoreSpriteColor();
    }
    // Update is called once per frame
    void Update()
    {
        if (isFlashing && Time.time >= flashEndTime)
        {
            RestoreSpriteColor();
        }
    }
    public void Damaged(int damage, bool isEnemyAttack = false)
    {
        if (
            isEnemyAttack &&
            playerController != null &&
            playerController.IsDashInvulnerable)
        {
            return;
        }

        if (damage > 0)
        {
            FlashDamage();
        }

        PlayerStatus.playerHealth -= damage;
        uiManager.HPUpdate();

        if (PlayerStatus.playerHealth <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    // HP表示更新・死亡処理もそのまま残す
    public void Refresh(int hpRefresh)
    {
        if((PlayerStatus.playerHealth + hpRefresh) <= 100)
        {
            PlayerStatus.playerHealth += hpRefresh;
        }
        else
        {
            PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        }
        uiManager.HPUpdate();
        uiManager.RefreshItemUpdate();
    }
}