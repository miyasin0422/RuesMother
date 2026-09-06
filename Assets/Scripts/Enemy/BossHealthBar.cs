using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;

    EnemyHealth bossHealth;

    public void Initialize(EnemyHealth health)
    {
        bossHealth = health;

        bossHealth.HealthChanged += UpdateHealthBar;

        UpdateHealthBar(
            bossHealth.CurrentHealth,
            bossHealth.MaxHealth
        );
    }

    void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.fillAmount =
            (float)currentHealth / maxHealth;
        Debug.Log("BossHPバー：" + healthBar.fillAmount);
    }

    void OnDestroy()
    {
        if (bossHealth != null)
        {
            bossHealth.HealthChanged -= UpdateHealthBar;
        }
    }
}