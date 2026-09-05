using UnityEngine;
using UnityEngine.InputSystem;

public class BowAttackBehavior : AttackBehavior
{
    public override bool IsMovementLocked => isAiming;
    public override bool FollowMouseFacing => isAiming;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform attackPoint;

    // チャージ
    [SerializeField] float minArrowSpeed = 5f;
    [SerializeField] float maxArrowSpeed = 20f;
    [SerializeField] float maxChargeTime = 1f;

    // 補助線
    [SerializeField] LineRenderer aimLine;
    [SerializeField] int linePointCount = 20;
    [SerializeField] float lineTime = 1f;

    private Vector2 aimDirection;
    private float chargeTime;
    private float currentArrowSpeed;
    private bool isAiming;

    public bool IsAiming => isAiming;

    public override void AttackPressed()
    {
        isAiming = true;

        chargeTime = 0f;
        currentArrowSpeed = minArrowSpeed;

        aimLine.enabled = true;

        UpdateAim();
    }

    public override void AttackHeld()
    {
        if (!isAiming)
        {
            return;
        }

        // チャージ
        chargeTime += Time.deltaTime;
        chargeTime = Mathf.Min(chargeTime, maxChargeTime);

        float chargeRate = chargeTime / maxChargeTime;

        currentArrowSpeed =
            Mathf.Lerp(minArrowSpeed, maxArrowSpeed, chargeRate);

        UpdateAim();
        UpdateAimLine();
    }

    public override void AttackReleased()
    {
        if (!isAiming)
        {
            return;
        }

        ShootArrow();

        aimLine.enabled = false;
        isAiming = false;

        GetComponent<GewenController>().EndAttack();
    }

    void UpdateAim()
    {
        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        mousePosition.z = 0f;

        aimDirection =
            ((Vector2)mousePosition - (Vector2)attackPoint.position).normalized;
    }

    void ShootArrow()
    {
        GameObject arrow = Instantiate(
            arrowPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        ArrowMovement arrowMovement =
            arrow.GetComponent<ArrowMovement>();

        arrowMovement.Shoot(
            aimDirection,
            currentArrowSpeed
        );
    }

    void UpdateAimLine()
    {
        Rigidbody2D arrowRb =
            arrowPrefab.GetComponent<Rigidbody2D>();

        Vector2 gravity =
            Physics2D.gravity * arrowRb.gravityScale;

        Vector2 startPosition = attackPoint.position;

        Vector2 startVelocity =
            aimDirection * currentArrowSpeed;

        aimLine.positionCount = linePointCount;

        for (int i = 0; i < linePointCount; i++)
        {
            float t =
                lineTime * i / (linePointCount - 1);

            Vector2 position =
                startPosition
                + startVelocity * t
                + 0.5f * gravity * t * t;

            aimLine.SetPosition(i, position);
        }
    }
}