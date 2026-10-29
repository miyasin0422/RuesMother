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
        UpdateAimLine();
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
        InitializeHitboxes(arrow);
        ArrowMovement arrowMovement =
            arrow.GetComponent<ArrowMovement>();

        arrowMovement.Shoot(
            aimDirection,
            currentArrowSpeed,
            Parameters.range
        );
    }

    void UpdateAimLine()
    {
        Rigidbody2D arrowRb = arrowPrefab.GetComponent<Rigidbody2D>();

        Vector2 gravity = Physics2D.gravity * arrowRb.gravityScale;
        Vector2 position = attackPoint.position;
        Vector2 startPosition = position;
        Vector2 velocity = aimDirection * currentArrowSpeed;

        float range = Mathf.Max(0f, Parameters.range);
        float step = Time.fixedDeltaTime;
        float speed = velocity.magnitude;
        float gravityStrength = gravity.magnitude;

        var positions = new System.Collections.Generic.List<Vector3>();
        positions.Add(position);

        // 射程に到達するまで探索できる時間の上限
        float maxTime = 0f;

        if (range > 0f)
        {
            if (gravityStrength > 0f)
            {
                maxTime = (
                    speed +
                    Mathf.Sqrt(speed * speed + 2f * gravityStrength * range)
                ) / gravityStrength;
            }
            else if (speed > 0f)
            {
                maxTime = range / speed;
            }
        }

        int maxSteps = Mathf.CeilToInt(maxTime / step) + 2;

        if (range > 0f && (speed > 0f || gravityStrength > 0f))
        {
            for (int i = 0; i < maxSteps; i++)
            {
                // 現在の矢と同じく、重力で速度を更新してから移動
                velocity += gravity * step;
                position += velocity * step;

                positions.Add(position);

                if (Vector2.Distance(startPosition, position) >= range)
                {
                    break;
                }
            }
        }

        // 予測した軌道を、指定した点数で描く
        int pointCount = Mathf.Max(2, linePointCount);

        aimLine.useWorldSpace = true;
        aimLine.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float index =
                (positions.Count - 1) * i / (float)(pointCount - 1);

            int from = Mathf.FloorToInt(index);
            int to = Mathf.Min(from + 1, positions.Count - 1);

            Vector3 point = Vector3.Lerp(
                positions[from],
                positions[to],
                index - from
            );

            aimLine.SetPosition(i, point);
        }
    }
}