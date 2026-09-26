using UnityEngine;
using UnityEngine.InputSystem;

public class RayMovement : MonoBehaviour
{
    [SerializeField] InputAction moveAction;
    [SerializeField] float moveSpeed = 5f;

    Rigidbody2D rb;
    float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<float>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed,
            rb.linearVelocity.y
        );

        // 左右反転
        if (moveInput != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x) * Mathf.Sign(moveInput);
            transform.localScale = scale;
        }
    }
}