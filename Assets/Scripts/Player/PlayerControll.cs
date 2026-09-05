using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControll : MonoBehaviour
{
    Rigidbody2D rb;
    //移動関連
    [SerializeField] InputAction moveAction;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float dropMoveSpeed = 10f;
    float moveInput;
    bool isFacingRight = true;
    //ジャンプ関連
    [SerializeField] InputAction jumpAction;
    [SerializeField] float jumpForce;
    private bool isGround;
    //回避関連
    [SerializeField] InputAction dodgeAction;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeTime;
    bool isDodge;
    bool canDodge = true;
    //しゃがむ関連
    [SerializeField] InputAction crouchAction;
    [SerializeField] GameObject normalVisual;
    [SerializeField] GameObject crouchVisual;
    [SerializeField] BoxCollider2D standCollider;
    [SerializeField] BoxCollider2D crouchCollider;
    [SerializeField] float dropSpeed = 0f;
    float downPressTime = 0f;
    bool isCrouch = false;
    bool isOnewayGround = false;
    bool isDropping = false;
    Collider2D oneWayGroundCollider;
    //こうげき関連
    [SerializeField] InputAction leftAttackAction;
    [SerializeField] InputAction rightAttackAction;
    [SerializeField] GameObject leftgawainPrefab;
    [SerializeField] GameObject rightgawainPrefab;
    [SerializeField] Transform summonPoint;
    [SerializeField] float coolTime = 1f;
    bool canAttack = true; 
    private GewenController leftGewenController;
    private GewenController rightGewenController;
    //回復関連
    [SerializeField] InputAction refreshAction;
    [SerializeField] int hpRefresh;
    [SerializeField] private PlayerDamage playerDamage;
    [SerializeField] GameObject RefreshEffectPoint;
    [SerializeField] GameObject refreshEffect;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //しゃがみ非表示
        crouchVisual.SetActive(false);
        playerDamage = gameObject.GetComponent<PlayerDamage>();
    }
    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dodgeAction.Enable();
        crouchAction.Enable();
        leftAttackAction.Enable();
        rightAttackAction.Enable();
        refreshAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dodgeAction.Disable();
        crouchAction.Disable();
        leftAttackAction.Disable();
        rightAttackAction.Disable();
        refreshAction.Disable();
    }
    void Update()
    {
        //しゃがみ落下処理
        if (isDropping && standCollider.bounds.max.y < oneWayGroundCollider.bounds.min.y)
        {
            Physics2D.IgnoreCollision(standCollider, oneWayGroundCollider, false);
            Physics2D.IgnoreCollision(crouchCollider, oneWayGroundCollider, false);

            isDropping = false;
            oneWayGroundCollider = null;
        }
        //しゃがみ入力
        if (crouchAction.WasPressedThisFrame() && isGround)
        {
            isCrouch = true;
            Crouch();
        }
        //しゃがみカウント
        if (crouchAction.IsPressed() && isCrouch && isOnewayGround)
        {
            downPressTime += Time.deltaTime;
            if (downPressTime >= 0.3f)
            {
                DropGround();
            }
        }
        else
        {
            downPressTime = 0f;
        }
        //しゃがみ解除
        if (crouchAction.WasReleasedThisFrame())
        {
            isCrouch = false;
            StandUp();
        }
        if (isCrouch)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        //方向              
        moveInput = moveAction.ReadValue<float>();

        if (moveInput != 0)
        {
            isFacingRight = moveInput > 0;

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
            transform.localScale = scale;
        }
        //ジャンプ入力
        if (isGround && jumpAction.triggered)
        {
            Jump();
        }
        //回避入力
        if (dodgeAction.triggered && canDodge)
        {
            isDodge = true;
            canDodge = false;
            StartCoroutine(Dodge());
        }
        //こうげき入力

        //左攻撃
        if (leftAttackAction.WasPressedThisFrame() && canAttack)
        {
            leftAttack();
        }

        if (leftAttackAction.IsPressed() && leftGewenController != null)
        {
            UpdateAttackFacing(leftGewenController);
            leftGewenController.AttackHeld();
        }

        if (leftAttackAction.WasReleasedThisFrame() && leftGewenController != null)
        {
            leftGewenController.AttackReleased();
            leftGewenController = null;
        }


        //右攻撃
        if (rightAttackAction.WasPressedThisFrame() && canAttack)
        {
            rightAttack();
        }

        if (rightAttackAction.IsPressed() && rightGewenController != null)
        {
            UpdateAttackFacing(leftGewenController);
            rightGewenController.AttackHeld();
        }

        if (rightAttackAction.WasReleasedThisFrame() && rightGewenController != null)
        {
            rightGewenController.AttackReleased();
            rightGewenController = null;
        }
        //回復入力
        if (isGround && PlayerStatus.refreshItemStock >= 1 && refreshAction.triggered)
        {
            Refresh();
        }
    }
    void FixedUpdate()
    {
        //通常移動
        if (!isDodge && !isCrouch)
        {
            if (IsAttackMovementLocked())
            {
                rb.linearVelocity =
                    new Vector2(0f, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity =
                    new Vector2(
                        moveInput * moveSpeed,
                        rb.linearVelocity.y
                    );
            }
        }
        //空中しゃがみ入力
        if (crouchAction.IsPressed() && !isGround &&!isOnewayGround && !isDropping)
        {
            rb.linearVelocity = new Vector2(moveInput * dropMoveSpeed, -dropSpeed);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }else if (collision.gameObject.CompareTag("OneWayGround"))
        {
            isGround = true;
            isOnewayGround = true;
            oneWayGroundCollider = collision.collider;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
        else if (collision.gameObject.CompareTag("OneWayGround"))
        {
            isGround = false;
            isOnewayGround = false;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    IEnumerator Dodge()
    {
        rb.linearVelocity = new Vector2(dodgeSpeed * moveInput, rb.linearVelocity.y);
        yield return new WaitForSeconds(dodgeTime);
        isDodge = false;
        canDodge = true;
    }
    void DropGround()
    {
        isDropping = true;
        Physics2D.IgnoreCollision(standCollider, oneWayGroundCollider, true);
        Physics2D.IgnoreCollision(crouchCollider, oneWayGroundCollider, true);
    }

    void Crouch()
    {
        normalVisual.SetActive(false);
        crouchVisual.SetActive(true);

        standCollider.enabled = false;
        crouchCollider.enabled = true;
    }
    void StandUp()
    {
        normalVisual.SetActive(true);
        crouchVisual.SetActive(false);

        standCollider.enabled = true;
        crouchCollider.enabled = false;
    }

    void leftAttack()
    {
        canAttack = false;

        GameObject gawain = Instantiate(
            leftgawainPrefab,
            summonPoint.position,
            Quaternion.identity
        );

        Vector3 scale = gawain.transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        gawain.transform.localScale = scale;

        leftGewenController = gawain.GetComponent<GewenController>();

        leftGewenController.AttackPressed();

        StartCoroutine(AttackCoolTime());
    }
    void rightAttack()
    {
        canAttack = false;

        GameObject gawain = Instantiate(
            rightgawainPrefab,
            summonPoint.position,
            Quaternion.identity
        );

        Vector3 scale = gawain.transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        gawain.transform.localScale = scale;

        rightGewenController = gawain.GetComponent<GewenController>();

        rightGewenController.AttackPressed();

        StartCoroutine(AttackCoolTime());
    }

    bool IsAttackMovementLocked()
    {
        bool leftLocked =
            leftGewenController != null
            && leftGewenController.IsMovementLocked;

        bool rightLocked =
            rightGewenController != null
            && rightGewenController.IsMovementLocked;

        return leftLocked || rightLocked;
    }
    void UpdateAttackFacing(GewenController gewenController)
    {
        if (gewenController == null || !gewenController.FollowMouseFacing)
        {
            return;
        }

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // マウスがルーの右か左か
        isFacingRight = mousePosition.x > transform.position.x;

        // ルーを反転
        Vector3 playerScale = transform.localScale;

        playerScale.x =
            Mathf.Abs(playerScale.x) * (isFacingRight ? 1 : -1);

        transform.localScale = playerScale;

        // ルーが反転したのでSummonPointも左右反転している
        // ゲーウェンを新しいSummonPointへ移動
        gewenController.transform.position = summonPoint.position;

        // ゲーウェンも左右反転
        Vector3 gewenScale = gewenController.transform.localScale;

        gewenScale.x =
            Mathf.Abs(gewenScale.x) * (isFacingRight ? 1 : -1);

        gewenController.transform.localScale = gewenScale;
    }
    //回復
    void Refresh()
    {
        PlayerStatus.refreshItemStock -= 1;
        playerDamage.Refresh(hpRefresh);
        Instantiate(refreshEffect, RefreshEffectPoint.transform.position, Quaternion.identity);
    }
    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }

}
