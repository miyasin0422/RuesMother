using System.Collections;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [SerializeField] EnemyAttack attackPrefab;
    [SerializeField] Transform attackPoint;

    [SerializeField] int attackPower;

    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] float attackCoolTime = 1f;
    [SerializeField] float attackSpriteDuration = 0.2f;

    EnemyVisualController enemyVisual;

    void Awake()
    {
        enemyVisual =
            GetComponentInChildren<EnemyVisualController>(true);

        if (enemyVisual == null)
        {
            Debug.LogError(
                "敵本体または子にEnemyVisualControllerがありません。",
                this
            );
        }
    }

    bool isAttacking = false;

    public void Attack(Transform target)
    {
        if (isAttacking)
        {
            return;
        }

        StartCoroutine(AttackCoroutine(target));
    }

    IEnumerator AttackCoroutine(Transform target)
    {
        isAttacking = true;

        Debug.Log("予備動作");

        yield return new WaitForSeconds(attackDelay);

        if (target != null)
        {
            Debug.Log("攻撃判定");

            if (enemyVisual != null)
            {
                enemyVisual.ShowAttack(attackSpriteDuration);
            }

            EnemyAttack attack = Instantiate(
                attackPrefab,
                attackPoint.position,
                attackPoint.rotation
            );

            attack.Initialize(
                attackPower,
                target.position
            );
        }

        Debug.Log("硬直");

        yield return new WaitForSeconds(attackCoolTime);

        isAttacking = false;
    }
}