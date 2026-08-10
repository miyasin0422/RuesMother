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
    float moveInput;
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
    float downPressTime = 0f;
    bool isOnewayGround = false;
    bool isDropping = false;
    Collider2D oneWayGroundCollider;
    //こうげき関連
    [SerializeField] InputAction attackAction1;
    [SerializeField] InputAction attackAction2;
    [SerializeField] GameObject gawainPrefab;
    [SerializeField] Transform summonPoint;
    [SerializeField] float coolTime = 1f;

    bool isFacingRight = true;
    bool isCrouch = false;
    bool canAttack = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction.Enable();
        jumpAction.Enable();
        dodgeAction.Enable();
        crouchAction.Enable();
        attackAction1.Enable();
        attackAction2.Enable();
        //しゃがみ非表示
        crouchVisual.SetActive(false);
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
            Crouch();
        }
        //しゃがみカウント
        if (crouchAction.IsPressed() && isOnewayGround)
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
            StandUp();
        }
        if (isCrouch)
        {
            return;
        }
        //移動                
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
        if (Mouse.current.leftButton.wasPressedThisFrame && canAttack)
        {
            Attack();
        }
    }
    void FixedUpdate()
    {
        if (!isDodge)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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
        isCrouch = true;
        normalVisual.SetActive(false);
        crouchVisual.SetActive(true);

        standCollider.enabled = false;
        crouchCollider.enabled = true;
    }
    void StandUp()
    {
        isCrouch = false;
        normalVisual.SetActive(true);
        crouchVisual.SetActive(false);

        standCollider.enabled = true;
        crouchCollider.enabled = false;
    }

    void Attack()
    {
        canAttack = false;
        GameObject gawain = Instantiate(gawainPrefab, summonPoint.position, Quaternion.identity);
        Vector3 scale = gawain.transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        gawain.transform.localScale = scale;
        StartCoroutine(AttackCoolTime());
    }
    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(coolTime);
        canAttack = true;
    }

}
