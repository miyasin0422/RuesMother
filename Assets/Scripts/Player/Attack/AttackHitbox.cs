using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] int damage = 10;

    public Gene.WeaponParam Parameters { get; private set; }

    public void Initialize(Gene.WeaponParam parameters)
    {
        Parameters = parameters;

        // 敵のHPが整数なので、攻撃力を最も近い整数に丸める
        damage = Mathf.Max(
            0,
            Mathf.RoundToInt(parameters.attackPower)
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null || enemy.IsDead)
        {
            return;
        }

        // 通常の攻撃ダメージ
        enemy.Damaged(damage);

        // 通常攻撃で倒した場合は酸を付与しない
        if (enemy.IsDead || !Parameters.isAcid)
        {
            return;
        }

        int tickDamage = Mathf.Max(
            0,
            Mathf.RoundToInt(Parameters.acidDamage)
        );

        if (tickDamage <= 0)
        {
            return;
        }

        float chance = Mathf.Clamp01(Parameters.acidChance);

        bool success =
            chance >= 1f ||
            (chance > 0f && Random.value < chance);

        if (success)
        {
            enemy.ApplyAcid(
                tickDamage,
                5f, // 持続時間
                1f  // ダメージ間隔
            );
        }
    }
}