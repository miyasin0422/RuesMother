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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
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
    public void Damaged(int damage)
    {
        if (damage > 0)
        {
            FlashDamage();
        }
        PlayerStatus.playerHealth -= damage;
        uiManager.HPUpdate();
        //Debug.Log("playerHP：" + PlayerStatus.playerHealth);
        if (PlayerStatus.playerHealth <= 0)
        {
            StartCoroutine(ReStart());
        }
    }
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
    IEnumerator ReStart()
    {
        yield return new WaitForSeconds(1);
        PlayerStatus.playerHealth = PlayerStatus.MaxplayerHealth;
        if(SaveManager.savedSceneName == "Stage1-1")
        {
            SceneMoveData.NextSpawnId = "Start";
        }
        else
        {
            SceneMoveData.NextSpawnId = "Left";
        }
        SceneManager.LoadScene(SaveManager.savedSceneName);
    }
}