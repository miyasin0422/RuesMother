using UnityEngine;
using System.Collections;

public class RayCompanionController : MonoBehaviour
{
    enum CompanionState
    {
        Following,
        Transforming,
        Weapon
    }

    [SerializeField] Transform followTarget;

    [Header("見た目")]
    [SerializeField] SpriteRenderer raySprite;
    [SerializeField] GameObject lightOrb;

    [Header("追従位置")]
    [SerializeField] float followDistance = 1.5f;
    [SerializeField] float followHeight = 0.8f;

    [Header("追従の速さ")]
    [SerializeField] float smoothTime = 0.2f;
    [SerializeField] float teleportDistance = 10f;

    [Header("変形")]
    [SerializeField] float transformDuration = 0.2f;

    Transform weaponAnchor;
    CompanionState state = CompanionState.Following;

    Vector3 followVelocity;
    Vector3 originalScale;
    Vector3 transformStartPosition;
    float transformElapsed;

    public bool IsFollowing =>
        state == CompanionState.Following;

    public bool IsWeaponReady =>
        state == CompanionState.Weapon;

    GewenController activeWeapon;
    bool attackInProgress;
    bool releaseRequested;

    public bool IsAttacking => attackInProgress;

    public bool IsMovementLocked =>
        activeWeapon != null && activeWeapon.IsMovementLocked;

    public bool FollowMouseFacing =>
        activeWeapon != null && activeWeapon.FollowMouseFacing;


    void Awake()
    {
        originalScale = transform.localScale;

        if (raySprite == null)
        {
            raySprite = GetComponent<SpriteRenderer>();
        }

        UpdateVisuals();
    }

    public void Initialize(Transform target)
    {
        followTarget = target;
        followVelocity = Vector3.zero;
        state = CompanionState.Following;

        if (followTarget != null)
        {
            weaponAnchor = followTarget.Find("WeaponAnchor");

            transform.position = GetFollowPosition();
            UpdateFacing();
        }

        UpdateVisuals();
    }

    void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }

        switch (state)
        {
            case CompanionState.Following:
                Follow();
                break;

            case CompanionState.Transforming:
                MoveToWeaponAnchor();
                break;

            case CompanionState.Weapon:
                if (weaponAnchor == null)
                {
                    ReturnToFollow();
                    return;
                }

                transform.position = weaponAnchor.position;
                UpdateWeaponPose();
                break;
        }
    }
    public bool TryAttack(GameObject weaponPrefab)
    {
        if (attackInProgress || weaponPrefab == null)
        {
            return false;
        }

        // 既存の武器Prefabの構成を確認
        if (
            weaponPrefab.GetComponent<GewenController>() == null ||
            weaponPrefab.GetComponent<AttackBehavior>() == null)
        {
            Debug.LogWarning(
                "武器PrefabにGewenControllerとAttackBehaviorが必要です。",
                weaponPrefab
            );

            return false;
        }

        if (!TryBeginTransform())
        {
            return false;
        }

        attackInProgress = true;
        releaseRequested = false;

        StartCoroutine(WeaponAttack(weaponPrefab));

        return true;
    }

    IEnumerator WeaponAttack(GameObject weaponPrefab)
    {
        // 光の玉が手元へ到着するまで待つ
        while (
            followTarget != null &&
            state == CompanionState.Transforming)
        {
            yield return null;
        }

        if (
            followTarget == null ||
            weaponAnchor == null ||
            state != CompanionState.Weapon)
        {
            FinishWeaponAttack();
            yield break;
        }

        GameObject weaponObject = Instantiate(
            weaponPrefab,
            weaponAnchor.position,
            Quaternion.identity
        );

        activeWeapon =
            weaponObject.GetComponent<GewenController>();

        UpdateWeaponPose();
        activeWeapon.AttackPressed();

        // 変形中にボタンを離していた場合も反映する
        if (releaseRequested && activeWeapon != null)
        {
            activeWeapon.AttackReleased();
        }

        // 既存のEndAttack()で武器が消えるまで待つ
        while (
            activeWeapon != null &&
            followTarget != null &&
            state == CompanionState.Weapon)
        {
            yield return null;
        }

        FinishWeaponAttack();
    }

    public void HoldAttack()
    {
        if (!attackInProgress || releaseRequested)
        {
            return;
        }

        if (activeWeapon != null)
        {
            UpdateWeaponPose();
            activeWeapon.AttackHeld();
        }
    }

    public void ReleaseAttack()
    {
        if (!attackInProgress || releaseRequested)
        {
            return;
        }

        releaseRequested = true;

        if (activeWeapon != null)
        {
            UpdateWeaponPose();
            activeWeapon.AttackReleased();
        }
    }

    public void UpdateWeaponPose()
    {
        if (
            activeWeapon == null ||
            weaponAnchor == null ||
            followTarget == null)
        {
            return;
        }

        activeWeapon.transform.position = weaponAnchor.position;

        float facing =
            followTarget.localScale.x >= 0f ? 1f : -1f;

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
        attackInProgress = false;
        releaseRequested = false;

        ReturnToFollow();
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        FinishWeaponAttack();
    }

    void OnDisable()
    {
        CancelAttack();
    }
    void Follow()
    {
        Vector3 targetPosition = GetFollowPosition();

        if (Vector3.Distance(transform.position, targetPosition)
            > teleportDistance)
        {
            transform.position = targetPosition;
            followVelocity = Vector3.zero;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref followVelocity,
                Mathf.Max(smoothTime, 0.01f)
            );
        }

        UpdateFacing();
    }

    public bool TryBeginTransform()
    {
        if (!IsFollowing || weaponAnchor == null)
        {
            return false;
        }

        if (raySprite == null || lightOrb == null)
        {
            Debug.LogWarning(
                "Ray SpriteとLight Orbを設定してください。",
                this
            );

            return false;
        }

        transformStartPosition = transform.position;
        transformElapsed = 0f;
        followVelocity = Vector3.zero;

        state = CompanionState.Transforming;
        UpdateVisuals();

        return true;
    }

    void MoveToWeaponAnchor()
    {
        if (weaponAnchor == null)
        {
            ReturnToFollow();
            return;
        }

        transformElapsed += Time.deltaTime;

        float t = Mathf.Clamp01(
            transformElapsed / Mathf.Max(transformDuration, 0.01f)
        );

        // ルーが移動しても、現在の手元へ向かう
        transform.position = Vector3.Lerp(
            transformStartPosition,
            weaponAnchor.position,
            t
        );

        if (t >= 1f)
        {
            state = CompanionState.Weapon;
            UpdateVisuals();
        }
    }

    public void ReturnToFollow()
    {
        state = CompanionState.Following;
        followVelocity = Vector3.zero;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (raySprite != null)
        {
            raySprite.enabled =
                state == CompanionState.Following;
        }

        if (lightOrb != null)
        {
            lightOrb.SetActive(
                state == CompanionState.Transforming
            );
        }
    }

    Vector3 GetFollowPosition()
    {
        float facing =
            followTarget.localScale.x >= 0f ? 1f : -1f;

        return followTarget.position + new Vector3(
            -facing * followDistance,
            followHeight,
            0f
        );
    }

    void UpdateFacing()
    {
        float facing =
            followTarget.localScale.x >= 0f ? 1f : -1f;

        Vector3 scale = originalScale;
        scale.x = -Mathf.Abs(originalScale.x) * facing;

        transform.localScale = scale;
    }

    [ContextMenu("テスト：光の玉になって手元へ移動")]
    void TestTransform()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (!TryBeginTransform())
        {
            Debug.LogWarning(
                "追従中か、WeaponAnchorがあるか確認してください。",
                this
            );
        }
    }

    [ContextMenu("テスト：レイに戻る")]
    void TestReturn()
    {
        if (Application.isPlaying)
        {
            ReturnToFollow();
        }
    }
}