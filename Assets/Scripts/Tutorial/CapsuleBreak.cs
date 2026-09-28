using System.Collections;
using UnityEngine;

public class CapsuleBreak : MonoBehaviour
{
    [SerializeField] GameObject breakEffectPrefab;
    [SerializeField] float destroyDelay = 0.3f;

    public IEnumerator PlayBreak()
    {
        // 破壊エフェクト
        if (breakEffectPrefab != null)
        {
            Instantiate(
                breakEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // とりあえず本体を非表示
        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // Colliderも無効化
        Collider2D collider =
            GetComponent<Collider2D>();

        if (collider != null)
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}