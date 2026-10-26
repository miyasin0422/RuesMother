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
    Transform weaponAnchor;
    GewenController activeWeapon;
    bool weaponAttackInProgress;
    bool attackReleased;
    public bool IsHoldingWeapon => activeWeapon != null;

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

        weaponAnchor = transform.Find("WeaponAnchor");
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
        if (dodgeRoutine != null)
        {
            StopCoroutine(dodgeRoutine);
            dodgeRoutine = null;
        }

        SetDashPassThrough(false);

        isDodge = false;
        canDodge = true;
        dodgeInvincibleEndTime = 0f;

        StopAllCoroutines();
        FinishWeaponAttack();
        canAttack = true;
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
        if (PlayerActionBlocker.IsBlocked)
        {
            moveInput = 0f;
            downPressTime = 0f;

            // チャージ中の弓などは、発射せずに片付ける
            if (activeWeapon != null)
            {
                FinishWeaponAttack();
            }

            if (dodgeRoutine != null)
            {
                StopCoroutine(dodgeRoutine);
                dodgeRoutine = null;
            }

            if (isDodge)
            {
                SetDashPassThrough(false);
                isDodge = false;
                canDodge = true;
                dodgeInvincibleEndTime = 0f;
            }

            if (isCrouch)
            {
                isCrouch = false;
                StandUp();
            }

            return;
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
        if (PlayerActionBlocker.IsBlocked)
        {
            if (!IsKnockedBack)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            return;
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
            PlayerActionBlocker.IsBlocked ||
            PlayerActionBlocker.IsPointerOverUI())
        {
            return;
        }
        if (
            !canAttack ||
            weaponAttackInProgress ||
            Gene.PlayerLoadout.instance == null)
        {
            return;
        }

        Gene.EquippedWeaponData weaponData = isLeft
            ? Gene.PlayerLoadout.instance.LeftWeaponData
            : Gene.PlayerLoadout.instance.RightWeaponData;

        if (weaponData == null)
        {
            return;
        }

        GameObject prefab = weaponData.weaponPrefab;

        if (prefab == null)
        {
            Debug.LogWarning(
                $"武器「{weaponData.weaponName}」にPrefabがありません。"
            );
            return;
        }

        if (weaponAnchor == null)
        {
            Debug.LogWarning(
                "ルーの子オブジェクトにWeaponAnchorが必要です。",
                this
            );
            return;
        }

        if (
            prefab.GetComponent<GewenController>() == null ||
            prefab.GetComponent<AttackBehavior>() == null)
        {
            Debug.LogWarning(
                "武器PrefabにGewenControllerとAttackBehaviorが必要です。",
                prefab
            );
            return;
        }

        GameObject weaponObject = Instantiate(
            prefab,
            weaponAnchor.position,
            Quaternion.identity
        );

        activeWeapon = weaponObject.GetComponent<GewenController>();
        AttackBehavior behavior = weaponObject.GetComponent<AttackBehavior>();
        behavior.Initialize(weaponData.param);

        activeAttackIsLeft = isLeft;
        attackReleased = false;
        weaponAttackInProgress = true;
        canAttack = false;

        // 武器はルー側ですぐに攻撃を開始する
        UpdateWeaponPose();
        activeWeapon.AttackPressed();

        // レイがいる場合だけ演出を開始。成功や完了は待たない
        if (rayCompanion != null && rayCompanion.isActiveAndEnabled)
        {
            rayCompanion.TryBeginTransform();
        }

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

        if (activeWeapon == null)
        {
            return;
        }

        UpdateAttackFacing();

        if (attackReleased)
        {
            return;
        }

        bool held = activeAttackIsLeft
            ? leftAttackAction.IsPressed()
            : rightAttackAction.IsPressed();

        bool released = activeAttackIsLeft
            ? leftAttackAction.WasReleasedThisFrame()
            : rightAttackAction.WasReleasedThisFrame();

        if (held)
        {
            activeWeapon.AttackHeld();
        }

        if (released && activeWeapon != null)
        {
            attackReleased = true;
            activeWeapon.AttackReleased();
        }
    }

    bool IsAttackMovementLocked()
    {
        return activeWeapon != null &&
               activeWeapon.IsMovementLocked;
    }

    void UpdateAttackFacing()
    {
        if (
            activeWeapon == null ||
            !activeWeapon.FollowMouseFacing ||
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

        UpdateWeaponPose();
    }

    void LateUpdate()
    {
        if (!weaponAttackInProgress)
        {
            return;
        }

        // 既存のEndAttackで武器が消えたら演出も終了
        if (activeWeapon == null)
        {
            FinishWeaponAttack();
            return;
        }

        UpdateWeaponPose();
    }

    void UpdateWeaponPose()
    {
        if (activeWeapon == null || weaponAnchor == null)
        {
            return;
        }

        activeWeapon.transform.position = weaponAnchor.position;

        float facing = transform.localScale.x >= 0f ? 1f : -1f;

        Vector3 scale = activeWeapon.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        activeWeapon.transform.localScale = scale;
    }

    void FinishWeaponAttack()
    {
        if (activeWeapon != null)
        {
            Destroy(activeWeapon.gameObject);
        }

        activeWeapon = null;
        weaponAttackInProgress = false;
        attackReleased = false;

        if (rayCompanion != null)
        {
            rayCompanion.ReturnToFollow();
        }
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
