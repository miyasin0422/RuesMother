using System.Collections;
using UnityEngine;

public class SwordAttackBehavior : AttackBehavior
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] Transform attackPivot;

    [Header("Time")]
    [SerializeField] float chargeTime = 0.15f;
    [SerializeField] float swingTime = 0.2f;

    [Header("Rotation")]
    [SerializeField] float startAngle = 60f;
    [SerializeField] float endAngle = -60f;

    [Header("Position")]
    [SerializeField] Vector2 startOffset = new Vector2(-0.1f, 0.5f);
    [SerializeField] Vector2 endOffset = new Vector2(0.2f, -0.3f);
 
    public override void AttackPressed()
    {
        GameObject attack = Instantiate(
            attackPrefab,
            attackPivot.position,
            Quaternion.identity,
            attackPivot
        );

        attack.transform.localPosition = Vector3.zero;
        attack.transform.localScale = attackPrefab.transform.localScale;

        StartCoroutine(Slash());
    }

    IEnumerator Slash()
    {
        Vector3 basePosition = attackPivot.localPosition;

        Vector3 startPosition =
            basePosition + (Vector3)startOffset;

        Vector3 endPosition =
            basePosition + (Vector3)endOffset;

        // 最初に振りかぶった位置へ
        attackPivot.localPosition = startPosition;
        attackPivot.localRotation =
            Quaternion.Euler(0, 0, startAngle);

        // タメ
        yield return new WaitForSeconds(chargeTime);

        // 振り下ろし
        float elapsedTime = 0f;

        while (elapsedTime < swingTime)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / swingTime;

            attackPivot.localPosition =
                Vector3.Lerp(startPosition, endPosition, t);

            float angle =
                Mathf.Lerp(startAngle, endAngle, t);

            attackPivot.localRotation =
                Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        attackPivot.localPosition = endPosition;
        attackPivot.localRotation =
            Quaternion.Euler(0, 0, endAngle);

        GetComponent<GewenController>().EndAttack();
    }

}