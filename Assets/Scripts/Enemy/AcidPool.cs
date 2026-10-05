using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour
{
    [SerializeField] int initialDamage = 5;
    [SerializeField] int continuousDamage = 2;
    [SerializeField] float damageInterval = 1f;
    [SerializeField] float lifeTime = 5f; 
    readonly List<PlayerDamage> damageTargets = new();

    // プレイヤーの複数Colliderによる重複を防ぐ
    readonly Dictionary<Collider2D, PlayerDamage> contacts = new();
    readonly Dictionary<PlayerDamage, float> nextDamageTimes = new();

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    void Update()
    {
        // 更新中に辞書を変更できるよう、対象を一度リストにコピーする
        damageTargets.Clear();
        damageTargets.AddRange(nextDamageTimes.Keys);

        foreach (PlayerDamage player in damageTargets)
        {
            if (player == null)
            {
                nextDamageTimes.Remove(player);
                continue;
            }

            if (PlayerStatus.playerHealth <= 0)
            {
                continue;
            }

            if (Time.time < nextDamageTimes[player])
            {
                continue;
            }

            nextDamageTimes[player] =
                Time.time + Mathf.Max(damageInterval, 0.05f);

            player.Damaged(continuousDamage);

            Debug.Log($"酸の継続ダメージ: {continuousDamage}");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        RegisterContact(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        RegisterContact(other);
    }

    PlayerDamage RegisterContact(Collider2D other)
    {
        if (contacts.TryGetValue(other, out PlayerDamage registered))
        {
            return registered;
        }

        PlayerDamage player =
            other.GetComponentInParent<PlayerDamage>();

        if (player == null)
        {
            return null;
        }

        contacts.Add(other, player);

        if (!nextDamageTimes.ContainsKey(player))
        {
            nextDamageTimes.Add(
                player,
                Time.time + Mathf.Max(damageInterval, 0.05f)
            );

            player.Damaged(initialDamage);
        }

        return player;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!contacts.TryGetValue(other, out PlayerDamage player))
        {
            return;
        }

        contacts.Remove(other);

        // 他のColliderがまだ触れていれば接触を継続する
        if (!contacts.ContainsValue(player))
        {
            nextDamageTimes.Remove(player);
        }
    }

    void OnDisable()
    {
        contacts.Clear();
        nextDamageTimes.Clear();
    }
}