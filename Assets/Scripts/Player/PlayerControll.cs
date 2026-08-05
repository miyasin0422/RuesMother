using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControll : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private float moveInput;
    //ジャンプ関連
    [SerializeField]
    private InputAction jumpAction;
    [SerializeField]
    private bool isGround;
    [SerializeField]
    private float jumpForce;
    //回避関連
    [SerializeField]
    private InputAction dodgeAction;
    [SerializeField]
    private float dodgeSpeed;
    [SerializeField]
    private bool isDodge;
    [SerializeField]
    private bool canDodge = true;
    [SerializeField]
    private float dodgeTime;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpAction.Enable();
        dodgeAction.Enable();
    }

    void Update()
    {
        moveInput = 0f;
        //左入力
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput -= 1f;
        }

        //右入力
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput += 1f;
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
}
