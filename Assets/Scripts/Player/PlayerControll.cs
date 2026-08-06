using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControll : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;
    //ジャンプ関連
    [SerializeField] private InputAction jumpAction;
    [SerializeField] private bool isGround;
    [SerializeField] private float jumpForce;
    //回避関連
    [SerializeField] private InputAction dodgeAction;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private bool isDodge;
    [SerializeField] private bool canDodge = true;
    [SerializeField] private float dodgeTime;
    //しゃがむ関連
    [SerializeField] GameObject normalVisual;
    [SerializeField] GameObject crouchVisual;
    [SerializeField] BoxCollider2D normalCollider;
    [SerializeField] BoxCollider2D crouchCollider;
    //こうげき関連
    [SerializeField] GameObject gawainPrefab;
    [SerializeField] Transform summonPoint;
    [SerializeField] float coolTime = 1f;

    bool isFacingRight = true;
    bool isCrouch = false;
    bool canAttack = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction.Enable();
        dodgeAction.Enable();
        //しゃがみ非表示
        crouchVisual.SetActive(false);
    }

    void Update()
    {
        moveInput = 0f;
         //しゃがみ入力
        if (Keyboard.current.sKey.wasPressedThisFrame && isGround)
        {
            Crouch();
        }
        if (Keyboard.current.sKey.wasReleasedThisFrame)
        {
            StandUp();
        }
        if (isCrouch)
        {
            return;
        }
        //左入力
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput -= 1f;
            isFacingRight = false; 
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        //右入力
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput += 1f;
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = false;
        }
    }

    void Crouch()
    {
        isCrouch = true;
        normalVisual.SetActive(false);
        crouchVisual.SetActive(true);

        normalCollider.enabled = false;
        crouchCollider.enabled = true;
    }
    void StandUp()
    {
        isCrouch = false;
        normalVisual.SetActive(true);
        crouchVisual.SetActive(false);

        normalCollider.enabled = true;
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
