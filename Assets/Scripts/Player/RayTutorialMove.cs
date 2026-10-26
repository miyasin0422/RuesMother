using UnityEngine;
using UnityEngine.InputSystem;

public class RayTutorialMove : MonoBehaviour
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
        moveInput = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    void Update()
    {
        moveInput = PlayerActionBlocker.IsBlocked
            ? 0f
            : moveAction.ReadValue<float>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            PlayerActionBlocker.IsBlocked ? 0f : moveInput * moveSpeed,
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