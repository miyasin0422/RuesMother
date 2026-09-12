using System.Collections;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [SerializeField] EnemyAttack attackPrefab;
    [SerializeField] Transform attackPoint;

    [SerializeField] int attackPower;

    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] float attackCoolTime = 1f;

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