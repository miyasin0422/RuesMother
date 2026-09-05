using System.Collections;
using UnityEngine;

public class FrogTongueAttack : EnemyAttack
{
    [SerializeField] float extendTime = 0.15f;
    [SerializeField] float holdTime = 0.1f;
    [SerializeField] float retractTime = 0.15f;

    // 舌Spriteの元の横幅
    [SerializeField] float baseLength = 1f;

    int damage;

    bool hasHit;

    Vector3 originalScale;

    public override void Initialize(
        int attackPower,
        Vector2 targetPosition)
    {
        damage = attackPower;

        Vector2 origin = transform.position;

        Vector2 difference =
            targetPosition - origin;

        float distance = difference.magnitude;

        Vector2 direction =
            difference.normalized;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0, 0, angle);

        originalScale = transform.localScale;

        StartCoroutine(
            TongueCoroutine(
                origin,
                direction,
                distance
            )
        );
    }

    IEnumerator TongueCoroutine(
        Vector2 origin,
        Vector2 direction,
        float distance)
    {
        float timer = 0;

        // 伸びる
        while (timer < extendTime)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / extendTime);

            SetLength(origin, direction, distance, t);

            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        timer = 0;

        // 戻る
        while (timer < retractTime)
        {
            timer += Time.deltaTime;

            float t =
                1f - Mathf.Clamp01(timer / retractTime);

            SetLength(origin, direction, distance, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    void SetLength(
        Vector2 origin,
        Vector2 direction,
        float distance,
        float rate)
    {
        float length = distance * rate;

        transform.position =
            origin + direction * (length / 2f);

        Vector3 scale = originalScale;

        scale.x =
            originalScale.x
            * (distance / baseLength)
            * rate;

        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit)
        {
            return;
        }

        PlayerDamage playerDamage =
            collision.GetComponentInParent<PlayerDamage>();

        if (playerDamage != null)
        {
            hasHit = true;

            playerDamage.Damaged(damage);
        }
    }
}