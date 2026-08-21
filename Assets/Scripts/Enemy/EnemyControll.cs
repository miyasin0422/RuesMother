using System.Collections;
using UnityEngine;

public class EnemyControll : MonoBehaviour
{
    enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    // 索敵・移動
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float patrolDistance = 3f;
    [SerializeField] LayerMask playerLayer;

    // 攻撃
    [SerializeField] EnemyAttack attackPrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] int attackPower;
    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] float attackCoolTime = 1f;
    //HPバー
    [SerializeField] private Transform hpBar;
    Vector2 startPosition;
    int moveDirection = 1;
    EnemyState currentState = EnemyState.Patrol;

    Rigidbody2D rb;
    Vector3 scale;
    protected Transform player;

    bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scale = transform.localScale;
        startPosition = transform.position;
    }

    void Update()
    {
        SearchPlayer();
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }
        //移動方向に合わせて反転
        scale.x = Mathf.Abs(transform.localScale.x) * moveDirection;
        transform.localScale = scale;
        //HPバーだけ反転しない
        Vector3 hpScale = hpBar.localScale;
        hpScale.x = Mathf.Abs(hpScale.x) * moveDirection;
        hpBar.localScale = hpScale;
    }

    // 索敵処理
    void SearchPlayer()
    {
        Collider2D foundPlayer = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (foundPlayer != null)
        {
            player = foundPlayer.transform;

            if (currentState == EnemyState.Patrol)
            {
                Debug.Log("巡回→追跡");
                currentState = EnemyState.Chase;
            }
        }
        else
        {
            player = null;

            if (currentState == EnemyState.Chase || currentState == EnemyState.Attack)
            {
                Debug.Log("→巡回");
                currentState = EnemyState.Patrol;
            }
        }
    }

    // 巡回状態
    void Patrol()
    {
        if (transform.position.x >= startPosition.x + patrolDistance)
        {
            moveDirection = -1;
        }
        else if (transform.position.x <= startPosition.x - patrolDistance)
        {
            moveDirection = 1;
        }

        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    // 追跡状態
    void Chase()
    {
        if (player == null)
        {
            return;
        }

        if (player.position.x > transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Debug.Log("追跡→攻撃");
            currentState = EnemyState.Attack;
            return;
        }

        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    // 攻撃状態
    void Attack()
    {
        if (player == null)
        {
            return;
        }

        if (player.position.x > transform.position.x)
        {
            moveDirection = 1;
        }
        else
        {
            moveDirection = -1;
        }
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            Debug.Log("攻撃→追跡");
            currentState = EnemyState.Chase;
            return;
        }

        if (!isAttacking)
        {
            Debug.Log("攻撃");
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        // 予備動作
        Debug.Log("予備動作");
        yield return new WaitForSeconds(attackDelay);

        // 攻撃生成
        Debug.Log("攻撃判定");
        EnemyAttack attack = Instantiate(attackPrefab, attackPoint.position, attackPoint.rotation);
        attack.Initialize(attackPower, player.position);

        // 硬直
        Debug.Log("硬直");
        yield return new WaitForSeconds(attackCoolTime);

        isAttacking = false;
    }

}