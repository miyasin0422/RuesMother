using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControll : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] PhysicsMaterial2D idleMaterial;

    PhysicsMaterial2D normalMaterial;
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
    [SerializeField] float dodgeInvincibleTime = 0.12f;

    float dodgeInvincibleEndTime;
    Coroutine dodgeRoutine;

    int enemyBodyLayer;
    LayerMask normalStandExclusions;
    LayerMask normalCrouchExclusions;

    public bool IsDashInvulnerable =>
        isActiveAndEnabled &&
        isDodge &&
        Time.time < dodgeInvincibleEndTime;
    //しゃがむ関連
    [SerializeField] InputAction crouchAction;
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
    [SerializeField] float coolTime = 1f;
    bool canAttack = true; 

    RayCompanionController rayCompanion;
    bool activeAttackIsLeft;

    public void SetRayCompanion(RayCompanionController companion)
    {
        rayCompanion = companion;
    }
    //回復関連
    [SerializeField] InputAction refreshAction;
    [SerializeField] int hpRefresh;
    [SerializeField] private PlayerDamage playerDamage;
    [SerializeField] GameObject RefreshEffectPoint;
    [SerializeField] GameObject refreshEffect;
    //ビジュアル
    public bool IsDashing => isDodge;
    public bool IsCrouching => isCrouch;
    public bool IsHoldingWeapon => rayCompanion != null && rayCompanion.HasWeapon;
    public bool IsWalking =>
        !isCrouch &&
        !isDodge &&
        !IsAttackMovementLocked() &&
        Mathf.Abs(moveInput) > 0.01f;

    //敵に触れる
    PlayerEnemyContact enemyContact;

    public bool IsKnockedBack =>
        enemyContact != null && enemyContact.IsKnockedBack;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDamage = GetComponent<PlayerDamage>();

        standCollider.enabled = true;
        crouchCollider.enabled = false;        
        enemyBodyLayer = LayerMask.NameToLayer("Enemy");
        normalStandExclusions = standCollider.excludeLayers;
        normalCrouchExclusions = crouchCollider.excludeLayers;
        enemyContact = GetComponent<PlayerEnemyContact>();

        normalMaterial = rb.sharedMaterial;
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
        if (rayCompanion != null)
        {
            rayCompanion.CancelAttack();
        }
        if (dodgeRoutine != null)
        {
            StopCoroutine(dodgeRoutine);
            dodgeRoutine = null;
        }

        SetDashPassThrough(false);

        isDodge = false;
        canDodge = true;
        dodgeInvincibleEndTime = 0f;

        if (rb != null)
        {
            rb.sharedMaterial = normalMaterial;
        }
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
            UpdateRayAttackInput();

            if (!IsKnockedBack)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

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
        if (isGround && jumpAction.triggered && !IsKnockedBack)
        {
            Jump();
        }
        //回避入力
        if (dodgeAction.triggered && canDodge && !IsKnockedBack)
        {
            dodgeRoutine = StartCoroutine(Dodge());
        }
        //こうげき入力

        UpdateRayAttackInput();
        //回復入力
        if (isGround && PlayerStatus.refreshItemStock >= 1 && refreshAction.triggered)
        {
            Refresh();
        }
    }
    void FixedUpdate()
    {
        bool wantsToStop =
        isCrouch ||
        IsAttackMovementLocked() ||
        Mathf.Abs(moveInput) < 0.01f;

        bool useIdleMaterial =
            isGround &&
            wantsToStop &&
            !isDodge &&
            !IsKnockedBack &&
            !isDropping &&
            rb.linearVelocity.y <= 0.05f;

        PhysicsMaterial2D material =
            useIdleMaterial && idleMaterial != null
            ? idleMaterial
            : normalMaterial;

        if (rb.sharedMaterial != material)
        {
            rb.sharedMaterial = material;
        }
        //敵接触
        if (IsKnockedBack)
        {
            return;
        }
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
        isDodge = true;
        canDodge = false;

        float duration = Mathf.Max(dodgeTime, 0f);

        dodgeInvincibleEndTime = Time.time + Mathf.Clamp(
            dodgeInvincibleTime,
            0f,
            duration
        );

        SetDashPassThrough(true);

        // 横入力がなければ、現在向いている方向へダッシュ
        float direction = Mathf.Abs(moveInput) > 0.01f
            ? Mathf.Sign(moveInput)
            : (isFacingRight ? 1f : -1f);

        rb.linearVelocity = new Vector2(
            dodgeSpeed * direction,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(duration);

        SetDashPassThrough(false);

        isDodge = false;
        canDodge = true;
        dodgeRoutine = null;
    }
    void SetDashPassThrough(bool enabled)
    {
        if (enemyBodyLayer < 0)
        {
            return;
        }

        int enemyMask = 1 << enemyBodyLayer;

        standCollider.excludeLayers = enabled
            ? normalStandExclusions.value | enemyMask
            : normalStandExclusions.value;

        crouchCollider.excludeLayers = enabled
            ? normalCrouchExclusions.value | enemyMask
            : normalCrouchExclusions.value;
    }
    void DropGround()
    {
        isDropping = true;
        Physics2D.IgnoreCollision(standCollider, oneWayGroundCollider, true);
        Physics2D.IgnoreCollision(crouchCollider, oneWayGroundCollider, true);
    }

    void Crouch()
    {
        standCollider.enabled = false;
        crouchCollider.enabled = true;
    }

    void StandUp()
    {
        standCollider.enabled = true;
        crouchCollider.enabled = false;
    }

    void leftAttack()
    {
        BeginRayAttack(true);
    }

    void rightAttack()
    {
        BeginRayAttack(false);
    }

    void BeginRayAttack(bool isLeft)
    {
        if (
            !canAttack ||
            rayCompanion == null ||
            PlayerLoadout.instance == null)
        {
            return;
        }

        GameObject weaponPrefab = isLeft
            ? PlayerLoadout.instance.leftWeapon
            : PlayerLoadout.instance.rightWeapon;

        if (!rayCompanion.TryAttack(weaponPrefab))
        {
            return;
        }

        activeAttackIsLeft = isLeft;
        canAttack = false;

        StartCoroutine(AttackCoolTime());
    }
    void UpdateRayAttackInput()
    {
        if (!isCrouch && canAttack)
        {
            if (leftAttackAction.WasPressedThisFrame())
            {
                leftAttack();
            }
            else if (rightAttackAction.WasPressedThisFrame())
            {
                rightAttack();
            }
        }

        if (rayCompanion == null || !rayCompanion.IsAttacking)
        {
            return;
        }

        UpdateAttackFacing();

        bool held = activeAttackIsLeft
            ? leftAttackAction.IsPressed()
            : rightAttackAction.IsPressed();

        bool released = activeAttackIsLeft
            ? leftAttackAction.WasReleasedThisFrame()
            : rightAttackAction.WasReleasedThisFrame();

        if (held)
        {
            rayCompanion.HoldAttack();
        }

        if (released)
        {
            rayCompanion.ReleaseAttack();
        }
    }
    bool IsAttackMovementLocked()
    {
        return rayCompanion != null &&
               rayCompanion.IsMovementLocked;
    }
    void UpdateAttackFacing()
    {
        if (
            rayCompanion == null ||
            !rayCompanion.FollowMouseFacing ||
            Mouse.current == null ||
            Camera.main == null)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        isFacingRight = mousePosition.x > transform.position.x;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1f : -1f);

        transform.localScale = scale;

        rayCompanion.UpdateWeaponPose();
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
